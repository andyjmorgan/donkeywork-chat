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
import { promptService } from '../services/api/promptService';
import { PromptItem, UpsertPromptModel } from '../models/api/Prompt';
import { PromptDialog } from '../features/prompts';
import '../styles/components/ConversationList.css';

// This comment is just to indicate that we're using the PromptItem type
// from the imported models

const Prompts: React.FC = () => {
  const [prompts, setPrompts] = useState<PromptItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [dialogVisible, setDialogVisible] = useState<boolean>(false);
  const [editPrompt, setEditPrompt] = useState<UpsertPromptModel & { id?: string }>();
  const toast = useRef<Toast>(null);

  // Fetch prompts
  const fetchPrompts = async () => {
    setLoading(true);
    try {
      const result = await promptService.getPrompts(0, 100);
      
      // Convert API model to UI model
      const promptItems: PromptItem[] = (result.prompts || result.items || []).map(prompt => ({
        id: prompt.id,
        name: prompt.name,
        description: prompt.description,
        content: prompt.content,
        usageCount: prompt.usageCount || 0,
        timestamp: prompt.updatedAt ? parseISO(prompt.updatedAt) : new Date(),
      }));
      
      setPrompts(promptItems);
    } catch (error) {
      console.error('Error fetching prompts:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load prompts' 
      });
    } finally {
      setLoading(false);
    }
  };

  // Load prompts on component mount
  useEffect(() => {
    fetchPrompts();
  }, []);

  // Utility to truncate text if too long
  const truncateText = (text: string, maxLength: number) => {
    if (!text) return '';
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text;
  };

  // Handle delete confirmation
  const confirmDelete = (prompt: PromptItem) => {
    // Truncate name if it's too long
    const truncatedName = truncateText(prompt.name, 50);
    
    confirmDialog({
      message: `Are you sure you want to delete "${truncatedName}"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: () => deletePrompt(prompt)
    });
  };

  // Delete prompt
  const deletePrompt = async (prompt: PromptItem) => {
    try {
      await promptService.deletePrompt(prompt.id);
      // If successful, update the UI
      const updatedPrompts = prompts.filter(p => p.id !== prompt.id);
      setPrompts(updatedPrompts);
    } catch (error) {
      console.error('Error deleting prompt:', error);
      // Show error message if needed
    }
  };

  // Format timestamp to relative time
  const formatTimestamp = (timestamp: Date) => {
    return formatDistanceToNow(timestamp, { addSuffix: true });
  };
  
  // Template for timestamp column
  const timestampTemplate = (rowData: PromptItem) => {
    return <span className="text-sm">{formatTimestamp(rowData.timestamp)}</span>;
  };

  // Template for usage column
  const usageTemplate = (rowData: PromptItem) => {
    return (
      <span className="flex align-items-center gap-2">
        <i className="pi pi-sync" style={{ fontSize: '0.8rem' }} />
        <span>{rowData.usageCount} {rowData.usageCount === 1 ? 'use' : 'uses'}</span>
      </span>
    );
  };

  // Handle edit button click
  const handleEdit = (prompt: PromptItem) => {
    setEditPrompt({
      id: prompt.id,
      name: prompt.name,
      description: prompt.description,
      content: prompt.content
    });
    setDialogVisible(true);
  };

  // Handle duplicate button click
  const handleDuplicate = (prompt: PromptItem) => {
    setEditPrompt({
      name: `${prompt.name} (Copy)`,
      description: prompt.description,
      content: prompt.content
    });
    setDialogVisible(true);
  };

  // Handle new prompt button click
  const handleNewPrompt = () => {
    setEditPrompt(undefined);
    setDialogVisible(true);
  };

  // Template for actions column using a kebab menu
  const actionsTemplate = (rowData: PromptItem) => {
    const menuRef = useRef<any>(null);
    
    const menuItems = [
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
      <PromptDialog 
        visible={dialogVisible}
        onHide={() => setDialogVisible(false)}
        onSave={fetchPrompts}
        editPrompt={editPrompt}
      />
      
      <Card className="mb-3 conversation-summary-card">
          <div className="flex flex-column md:flex-row justify-content-between">
            <div className="mb-3 md:mb-0">
              <h2 className="m-0 mb-2 text-xl">Prompt Library</h2>
              <p className="m-0 text-sm text-color-secondary">
                Create, manage, and use AI prompts for various tasks
                {!loading && prompts.length > 0 && ` (${prompts.length} total)`}
              </p>
            </div>
            <div className="flex align-items-center">
              <Button 
                label="Create New Prompt" 
                icon="pi pi-plus" 
                className="p-button-primary" 
                style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
                onClick={handleNewPrompt}
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
              <Column body={skeletonTemplate} style={{ width: '25%' }} />
              <Column body={skeletonTemplate} style={{ width: '40%' }} />
              <Column body={() => <Skeleton width="3rem" height="1.2rem" />} style={{ width: '15%' }} />
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
                  <h2 className="m-0 text-lg">All Prompts</h2>
                </div>
              }
              value={prompts} 
              paginator 
              rows={5} 
              rowsPerPageOptions={[5, 10, 25]} 
              globalFilter={undefined}
              emptyMessage="No prompts found"
              className="p-datatable-sm p-datatable-responsive-stack"
              sortField="timestamp"
              sortOrder={-1}
              responsiveLayout="stack"
              breakpoint="960px"
              dataKey="id"
              stripedRows
              onRowDoubleClick={(e) => handleEdit(e.data as PromptItem)}
              rowClassName={() => "cursor-pointer"}
            >
              <Column 
                field="name" 
                header="Name" 
                sortable 
                headerClassName="w-3 column-title"
                bodyClassName="font-medium"
              />
              <Column 
                field="description" 
                header="Description" 
                headerClassName="w-5 column-description"
                bodyClassName="text-color-secondary"
              />
              <Column 
                field="usageCount" 
                header="Usage" 
                body={usageTemplate} 
                sortable 
                headerClassName="w-1 column-usage"
              />
              <Column 
                field="timestamp" 
                header="Last Updated" 
                body={timestampTemplate}
                sortable 
                headerClassName="w-2 column-timestamp"
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

export default Prompts;