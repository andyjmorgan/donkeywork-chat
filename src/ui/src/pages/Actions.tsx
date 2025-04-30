import React, { useState, useEffect, useRef } from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Menu } from 'primereact/menu';
import { Skeleton } from 'primereact/skeleton';
import { Toast } from 'primereact/toast';
import { formatDistanceToNow, parseISO } from 'date-fns';
import { actionService } from '../services/api/actionService';
import { actionExecutionService } from '../services/api/actionExecutionService';
import { ActionItem, UpsertActionModel } from '../models/api/Action';
import { ActionDialog } from '../features/actions/components/ActionDialog';
import { ActionExecuteDialog } from '../features/actions/components/ActionExecuteDialog';
import '../styles/components/ConversationList.css';

const Actions: React.FC = () => {
  const [actions, setActions] = useState<ActionItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [dialogVisible, setDialogVisible] = useState<boolean>(false);
  const [executeDialogVisible, setExecuteDialogVisible] = useState<boolean>(false);
  const [editAction, setEditAction] = useState<UpsertActionModel & { id?: string }>();
  const [selectedAction, setSelectedAction] = useState<ActionItem | null>(null);
  const toast = useRef<Toast>(null);

  // Fetch actions
  const fetchActions = async () => {
    setLoading(true);
    try {
      const result = await actionService.getActions(0, 100);
      
      // Convert date strings to Date objects
      const actionItems: ActionItem[] = result.actions.map(action => ({
        ...action,
        createdAt: action.createdAt instanceof Date ? action.createdAt : parseISO(action.createdAt as string),
        updatedAt: action.updatedAt instanceof Date ? action.updatedAt : parseISO(action.updatedAt as string)
      }));
      
      setActions(actionItems);
    } catch (error) {
      console.error('Error fetching actions:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load actions' 
      });
    } finally {
      setLoading(false);
    }
  };

  // Load actions on component mount
  useEffect(() => {
    const loadActionsAndCheckForSelectedAction = async () => {
      await fetchActions();
      
      // Check if there's a selected action ID in sessionStorage
      const selectedActionId = sessionStorage.getItem('selectedActionId');
      if (selectedActionId) {
        // Find the action and open the edit dialog
        const found = actions.find(action => action.id === selectedActionId);
        if (found) {
          handleEdit(found);
        }
        // Clear the selected action ID from sessionStorage
        sessionStorage.removeItem('selectedActionId');
      }
    };
    
    loadActionsAndCheckForSelectedAction();
  }, []);

  // Utility to truncate text if too long
  const truncateText = (text: string, maxLength: number) => {
    if (!text) return '';
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text;
  };

  // Handle delete confirmation
  const confirmDelete = (action: ActionItem) => {
    // Truncate name if it's too long
    const truncatedName = truncateText(action.name, 50);
    
    confirmDialog({
      message: `Are you sure you want to delete "${truncatedName}"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: () => deleteAction(action)
    });
  };

  // Delete action
  const deleteAction = async (action: ActionItem) => {
    try {
      await actionService.deleteAction(action.id);
      // If successful, update the UI
      const updatedActions = actions.filter(a => a.id !== action.id);
      setActions(updatedActions);
      toast.current?.show({ 
        severity: 'success', 
        summary: 'Success', 
        detail: 'Action deleted successfully' 
      });
    } catch (error) {
      console.error('Error deleting action:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to delete action' 
      });
    }
  };

  // Format timestamp to relative time
  const formatTimestamp = (timestamp: Date | string) => {
    const dateObj = timestamp instanceof Date ? timestamp : parseISO(timestamp);
    return formatDistanceToNow(dateObj, { addSuffix: true });
  };
  
  // Template for timestamp column
  const timestampTemplate = (rowData: ActionItem) => {
    return <span className="text-sm">{formatTimestamp(rowData.updatedAt)}</span>;
  };


  // Template for action icon
  const iconTemplate = (rowData: ActionItem) => {
    if (!rowData.icon) {
      return <i className="pi pi-bolt text-lg" style={{ color: 'var(--primary-color)' }} />;
    }
    
    // Check if the icon is a URL
    if (rowData.icon.startsWith('http')) {
      return <img src={rowData.icon} alt={rowData.name} className="action-icon" style={{ width: '24px', height: '24px', objectFit: 'contain' }} />;
    }
    
    // Assume it's a PrimeReact icon class
    return <i className={`${rowData.icon} text-lg`} style={{ color: 'var(--primary-color)' }} />;
  };

  // Template for model configuration
  const modelConfigTemplate = (rowData: ActionItem) => {
    // Check for backend model configuration format
    if (rowData.actionModelConfiguration) {
      const providerType = rowData.actionModelConfiguration.providerType;
      const modelName = rowData.actionModelConfiguration.modelName;
      
      if (providerType || modelName) {
        return (
          <div className="flex flex-column">
            {providerType && <span className="text-sm font-medium">{providerType}</span>}
            {modelName && <span className="text-sm text-color-secondary">{modelName}</span>}
          </div>
        );
      }
    }
    
    // Fallback for old format or missing data
    return <span className="text-color-secondary">Default</span>;
  };

  // Handle edit button click
  const handleEdit = (action: ActionItem) => {
    // When editing, just pass the backend model configuration directly
    const modelConfig = action.actionModelConfiguration || {
      providerType: '',
      modelName: '',
      streaming: true,
      metadata: {}
    };
    
    setEditAction({
      id: action.id,
      name: action.name,
      description: action.description,
      icon: action.icon,
      systemPromptIds: action.systemPromptIds,
      userPromptIds: action.userPromptIds,
      allowedTools: action.allowedTools || action.toolProviderApplicationTypes || [],
      actionModelConfiguration: modelConfig
    });
    setDialogVisible(true);
  };

  // Handle duplicate button click
  const handleDuplicate = (action: ActionItem) => {
    // When duplicating, just pass the backend model configuration directly
    const modelConfig = action.actionModelConfiguration || {
      providerType: '',
      modelName: '',
      streaming: true,
      metadata: {}
    };
    
    setEditAction({
      name: `${action.name} (Copy)`,
      description: action.description,
      icon: action.icon,
      systemPromptIds: action.systemPromptIds,
      userPromptIds: action.userPromptIds,
      allowedTools: action.allowedTools || action.toolProviderApplicationTypes || [],
      actionModelConfiguration: modelConfig
    });
    setDialogVisible(true);
  };

  // Handle new action button click
  const handleNewAction = () => {
    setEditAction(undefined);
    setDialogVisible(true);
  };

  // Handle action execution
  const handleExecute = (action: ActionItem) => {
    setSelectedAction(action);
    setExecuteDialogVisible(true);
  };

  // Template for actions column using a kebab menu
  const actionsTemplate = (rowData: ActionItem) => {
    const menuRef = useRef<any>(null);
    
    const menuItems = [
      {
        label: 'Execute',
        icon: 'pi pi-play',
        className: 'p-success',
        command: () => handleExecute(rowData)
      },
      {
        separator: true
      },
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => handleEdit(rowData)
      },
      {
        label: 'Duplicate',
        icon: 'pi pi-copy',
        command: () => handleDuplicate(rowData)
      },
      {
        separator: true
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        className: 'p-error',
        command: () => confirmDelete(rowData)
      }
    ];
    
    return (
      <div className="flex justify-content-center">
        <Button
          icon="pi pi-ellipsis-v"
          className="p-button-rounded p-button-text p-button-plain action-menu-button"
          onClick={(e) => {
            e.stopPropagation();
            menuRef.current.toggle(e);
          }}
          aria-label="Options"
        />
        <Menu
          model={menuItems}
          popup
          ref={menuRef}
          popupAlignment="right"
        />
      </div>
    );
  };

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
    <div className="conversation-list-container">
      <Toast ref={toast} position="top-right" />
      {dialogVisible && (
        <ActionDialog 
          visible={dialogVisible}
          onHide={() => setDialogVisible(false)}
          onSave={fetchActions}
          editAction={editAction}
        />
      )}
      
      <ActionExecuteDialog
        visible={executeDialogVisible}
        onHide={() => setExecuteDialogVisible(false)}
        action={selectedAction}
      />
      
      <Card className="mb-3 conversation-summary-card">
        <div className="flex flex-column md:flex-row justify-content-between">
          <div className="mb-3 md:mb-0">
            <h2 className="m-0 mb-2 text-xl">Actions Library</h2>
            <p className="m-0 text-sm text-color-secondary">
              Create, manage, and configure AI actions for various workflows
              {!loading && actions.length > 0 && ` (${actions.length} total)`}
            </p>
          </div>
          <div className="flex align-items-center">
            <Button 
              label="Create New Action" 
              icon="pi pi-plus" 
              className="p-button-primary" 
              style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
              onClick={handleNewAction}
            />
          </div>
        </div>
      </Card>
      
      <ConfirmDialog />
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
            <Column body={() => <Skeleton shape="circle" size="1.5rem" />} style={{ width: '5%' }} />
            <Column body={skeletonTemplate} style={{ width: '15%' }} />
            <Column body={skeletonTemplate} style={{ width: '40%' }} />
            <Column body={() => <Skeleton width="5rem" height="1.2rem" />} style={{ width: '15%' }} />
            <Column body={() => <Skeleton width="5rem" height="1.2rem" />} style={{ width: '10%' }} />
            <Column body={() => (
              <div className="flex justify-content-center">
                <Skeleton shape="circle" size="2rem" />
              </div>
            )} style={{ width: '10%' }} />
          </DataTable>
        </Card>
      ) : (
        <Card>
          <DataTable
            header={
              <div className="flex justify-content-between align-items-center">
                <h2 className="m-0 text-lg">All Actions</h2>
              </div>
            }
            value={actions} 
            paginator 
            rows={5} 
            rowsPerPageOptions={[5, 10, 25]} 
            globalFilter={undefined}
            emptyMessage="No actions found"
            className="p-datatable-sm p-datatable-responsive-stack"
            sortField="updatedAt"
            sortOrder={-1}
            responsiveLayout="stack"
            breakpoint="960px"
            dataKey="id"
            stripedRows
            onRowDoubleClick={(e) => handleEdit(e.data as ActionItem)}
            rowClassName={() => "cursor-pointer"}
          >
            <Column 
              field="icon" 
              header="Icon" 
              body={iconTemplate}
              headerClassName="w-1 column-icon"
              style={{ width: '5%' }}
            />
            <Column 
              field="name" 
              header="Name" 
              sortable 
              headerClassName="w-3 column-title"
              bodyClassName="font-medium"
              style={{ width: '15%' }}
            />
            <Column 
              field="description" 
              header="Description" 
              headerClassName="w-6 column-description"
              bodyClassName="text-color-secondary"
              style={{ width: '40%' }}
            />
            <Column 
              field="actionModelConfiguration" 
              header="Model" 
              body={modelConfigTemplate} 
              sortable 
              headerClassName="w-2 column-model"
              style={{ width: '15%' }}
            />
            <Column 
              field="updatedAt" 
              header="Last Updated" 
              body={timestampTemplate}
              sortable 
              headerClassName="w-2 column-timestamp"
              style={{ width: '10%' }}
            />
            <Column 
              header="Actions"
              body={actionsTemplate} 
              headerClassName="w-2 column-actions text-center"
              bodyClassName="text-center"
              style={{ width: '10%' }}
              frozen
              alignFrozen="right"
            />
          </DataTable>
        </Card>
      )}
    </div>
  );
};

export default Actions;