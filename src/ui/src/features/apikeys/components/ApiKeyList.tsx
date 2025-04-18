import React, { useState, useEffect, useRef } from 'react';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { Card } from 'primereact/card';
import { Skeleton } from 'primereact/skeleton';
import { Toast } from 'primereact/toast';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Menu } from 'primereact/menu';
import { parseISO, formatDistanceToNow } from 'date-fns';

import { apiKeyService } from '../../../services/api';
import { ApiKeyItem } from '../../../models/ui/apikey/ApiKeyListTypes';
import { ApiKeyModel } from '../../../models/api/ApiKey';
import ApiKeyDialog from './ApiKeyDialog';

const ApiKeyList: React.FC = () => {
  const [apiKeys, setApiKeys] = useState<ApiKeyItem[]>([]);
  const [globalFilter, setGlobalFilter] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(true);
  const [dialogVisible, setDialogVisible] = useState<boolean>(false);
  const [selectedApiKey, setSelectedApiKey] = useState<ApiKeyModel | undefined>(undefined);
  const [isEditMode, setIsEditMode] = useState<boolean>(false);
  
  const toast = React.useRef<Toast>(null);

  // Fetch API keys
  const fetchApiKeys = async () => {
    setLoading(true);
    try {
      const result = await apiKeyService.getApiKeys(0, 1000);
      
      // Convert API model to UI model with additional fields
      const apiKeyItems: ApiKeyItem[] = (result.apiKeys || []).map(key => ({
        ...key,
        timestamp: parseISO(key.createdAt),
      }));
      
      setApiKeys(apiKeyItems);
    } catch (error) {
      console.error('Error fetching API keys:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load API keys'
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchApiKeys();
  }, []);

  // Handle viewing an API key
  const handleViewKey = async (apiKey: ApiKeyItem) => {
    try {
      const fullApiKey = await apiKeyService.getApiKey(apiKey.id);
      setSelectedApiKey(fullApiKey);
      setIsEditMode(false);
      setDialogVisible(true);
    } catch (error) {
      console.error('Error fetching API key details:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load API key details'
      });
    }
  };
  
  // Handle editing an API key
  const handleEditKey = async (apiKey: ApiKeyItem) => {
    try {
      const fullApiKey = await apiKeyService.getApiKey(apiKey.id);
      setSelectedApiKey(fullApiKey);
      setIsEditMode(true);
      setDialogVisible(true);
    } catch (error) {
      console.error('Error fetching API key details for editing:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load API key details'
      });
    }
  };

  // Create a new API key
  const createNewApiKey = () => {
    setSelectedApiKey(undefined);
    setIsEditMode(false);
    setDialogVisible(true);
  };

  // Format timestamp to relative time
  const formatTimestamp = (timestamp: Date) => {
    return formatDistanceToNow(timestamp, { addSuffix: true });
  };

  // Render column templates
  const nameTemplate = (rowData: ApiKeyItem) => {
    return (
      <div className="flex align-items-center gap-2">
        <span className="font-semibold">{rowData.name}</span>
        {!rowData.isEnabled && (
          <span className="p-tag p-tag-danger">Disabled</span>
        )}
      </div>
    );
  };

  const keyTemplate = (rowData: ApiKeyItem) => {
    return (
      <div className="text-sm font-mono">
        {rowData.apiKey}
      </div>
    );
  };

  const descriptionTemplate = (rowData: ApiKeyItem) => {
    return (
      <div className="text-sm text-color-secondary">
        {rowData.description || <em>No description</em>}
      </div>
    );
  };

  const timestampTemplate = (rowData: ApiKeyItem) => {
    return <span className="text-sm">{formatTimestamp(rowData.timestamp)}</span>;
  };

  const statusTemplate = (rowData: ApiKeyItem) => {
    return rowData.isEnabled ? 
      <span className="p-tag p-tag-success">Enabled</span> : 
      <span className="p-tag p-tag-danger">Disabled</span>;
  };

  // Confirm delete dialog
  const confirmDelete = (apiKey: ApiKeyItem) => {
    confirmDialog({
      message: `Are you sure you want to delete the API key "${apiKey.name}"?`,
      header: 'Delete API Key',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: () => deleteApiKey(apiKey.id)
    });
  };
  
  // Delete API key
  const deleteApiKey = async (id: string) => {
    try {
      await apiKeyService.deleteApiKey(id);
      toast.current?.show({
        severity: 'success',
        summary: 'Success',
        detail: 'API key deleted successfully'
      });
      // Refresh the list
      fetchApiKeys();
    } catch (error) {
      console.error('Error deleting API key:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to delete API key'
      });
    }
  };

  // Action menu reference - one per row is created dynamically
  
  const actionsTemplate = (rowData: ApiKeyItem) => {
    // Create a ref for this specific row's menu
    const itemMenuRef = useRef<Menu>(null);
    
    // Menu items for the dropdown
    const menuItems = [
      {
        label: 'View',
        icon: 'pi pi-eye',
        command: () => handleViewKey(rowData)
      },
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => handleEditKey(rowData)
      },
      {
        separator: true
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        className: 'text-red-500',
        command: () => confirmDelete(rowData)
      }
    ];
    
    return (
      <div className="flex justify-content-center">
        <Menu model={menuItems} popup ref={itemMenuRef} />
        <Button 
          icon="pi pi-ellipsis-v" 
          className="p-button-rounded p-button-text" 
          onClick={(e) => itemMenuRef.current?.toggle(e)}
          aria-label="Options"
          tooltip="Actions"
        />
      </div>
    );
  };

  const header = (
    <div className="flex justify-content-between align-items-center">
      <h2 className="m-0 text-lg">Your API Keys</h2>
      <div className="p-input-icon-left">
        <i className="pi pi-search" />
        <InputText 
          value={globalFilter} 
          onChange={(e) => setGlobalFilter(e.target.value)} 
          placeholder="Search" 
          className="p-inputtext-sm"
        />
      </div>
    </div>
  );

  // Generate skeleton rows for the table when loading
  const skeletonTemplate = () => {
    return (
      <div className="flex align-items-center gap-2">
        <Skeleton width="70%" height="1.2rem" />
      </div>
    );
  };
  
  // Generate array of empty objects for skeleton loading
  const skeletonArray = Array(5).fill({});

  return (
    <div className="api-key-list-container">
      <Toast ref={toast} />
      <ConfirmDialog />
      
      {/* Summary Card */}
      <Card className="mb-3 api-key-summary-card">
        <div className="flex flex-column md:flex-row justify-content-between">
          <div className="mb-3 md:mb-0">
            <h2 className="m-0 mb-2 text-xl">API Keys</h2>
            <p className="m-0 text-sm text-color-secondary">
              Create and manage API keys for automated access to the DonkeyWork MCP server. The DonkeyWork MCP server will allow you to re-use tooling and prompts here in your native applications!
          </p>
          </div>
          <div className="flex align-items-center">
            <Button 
              label="New Key"
              icon="pi pi-plus" 
              className="p-button-primary" 
              style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
              onClick={createNewApiKey}
            />
          </div>
        </div>
      </Card>
      
      {/* API Key Dialog */}
      <ApiKeyDialog 
        visible={dialogVisible}
        onHide={() => setDialogVisible(false)}
        apiKey={selectedApiKey}
        onSave={fetchApiKeys}
        editMode={isEditMode}
      />
      
      {/* Data Table */}
      {loading ? (
        <Card>
          <div className="mb-3">
            <Skeleton width="25%" height="1.5rem" className="mb-3" />
            <div className="flex justify-content-between mb-3">
              <Skeleton width="15%" height="1.2rem" />
              <Skeleton width="15%" height="1.2rem" />
            </div>
          </div>
          <DataTable 
            value={skeletonArray} 
            className="p-datatable-sm"
          >
            <Column body={skeletonTemplate} style={{ width: '25%' }} />
            <Column body={skeletonTemplate} style={{ width: '25%' }} />
            <Column body={skeletonTemplate} style={{ width: '20%' }} />
            <Column body={skeletonTemplate} style={{ width: '15%' }} />
            <Column body={() => (
              <div className="flex gap-2 justify-content-end">
                <Skeleton shape="circle" size="2rem" />
              </div>
            )} style={{ width: '15%' }} />
          </DataTable>
        </Card>
      ) : (
        <DataTable 
          value={apiKeys} 
          paginator 
          rows={10} 
          rowsPerPageOptions={[5, 10, 25]} 
          globalFilter={globalFilter}
          header={header}
          emptyMessage="No API keys found"
          stripedRows
          className="p-datatable-sm p-datatable-responsive-stack"
          sortField="timestamp"
          sortOrder={-1}
          responsiveLayout="stack"
          breakpoint="960px"
          onRowDoubleClick={(e) => handleViewKey(e.data as ApiKeyItem)}
          rowClassName={() => "cursor-pointer"}
        >
          <Column field="name" header="Name" body={nameTemplate} sortable headerClassName="w-2" />
          <Column field="description" header="Description" body={descriptionTemplate} headerClassName="w-3" />
          <Column field="apiKey" header="API Key" body={keyTemplate} headerClassName="w-3" />
          <Column field="timestamp" header="Created" body={timestampTemplate} sortable headerClassName="w-2" />
          <Column field="isEnabled" header="Status" body={statusTemplate} sortable headerClassName="w-1" />
          <Column header="Actions" body={actionsTemplate} headerClassName="w-3" headerStyle={{ textAlign: 'center' }} />
        </DataTable>
      )}
    </div>
  );
};

export default ApiKeyList;