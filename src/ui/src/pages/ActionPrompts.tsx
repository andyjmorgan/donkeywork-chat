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
import { actionPromptService } from '../services/api/actionPromptService';
import { ActionPromptItem, UpsertActionPromptModel, ActionPromptMessageModel } from '../models/api/Prompt';
import { ActionPromptDialog } from '../features/prompts';
import '../styles/components/ConversationList.css';

const ActionPrompts: React.FC = () => {
  const [actionPrompts, setActionPrompts] = useState<ActionPromptItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [dialogVisible, setDialogVisible] = useState<boolean>(false);
  const [editPrompt, setEditPrompt] = useState<UpsertActionPromptModel & { id?: string }>();
  const toast = useRef<Toast>(null);

  // Fetch action prompts
  const fetchActionPrompts = async () => {
    setLoading(true);
    try {
      const result = await actionPromptService.getActionPrompts(0, 100);
      
      // Convert API model to UI model and handle content format compatibility
      const promptItems: ActionPromptItem[] = (result.prompts || result.items || []).map(prompt => {
        // Process messages to handle possible object content
        const processedMessages = (prompt.messages || []).map(msg => {
          // If content is an object with text property, normalize it
          if (msg.content && typeof msg.content === 'object' && 'text' in msg.content) {
            return {
              ...msg,
              content: msg.content.text || ''
            };
          }
          // If content is an array, extract text content
          if (Array.isArray(msg.content)) {
            const textContent = msg.content.find(c => c.type === 'text');
            return {
              ...msg,
              content: textContent && 'text' in textContent ? textContent.text : ''
            };
          }
          return msg;
        });
        
        // Process variables to support new record-based format
        let processedVariables;
        
        if (Array.isArray(prompt.variables)) {
          // Handle array format (old)
          processedVariables = (prompt.variables || []).map(variable => {
            if (variable.isRequired !== undefined) {
              return {
                ...variable,
                required: Boolean(variable.isRequired),
                // Keep isRequired for backward compatibility
                isRequired: Boolean(variable.isRequired)
              };
            }
            return variable;
          });
        } else if (typeof prompt.variables === 'object' && prompt.variables !== null) {
          // Handle record format (new)
          processedVariables = Object.entries(prompt.variables).map(([name, props]: [string, any]) => ({
            name,
            description: props.description || '',
            defaultValue: props.defaultValue || '',
            required: props.isRequired !== undefined ? Boolean(props.isRequired) : Boolean(props.required),
          }));
        } else {
          processedVariables = [];
        }
        
        return {
          id: prompt.id,
          name: prompt.name,
          description: prompt.description,
          variables: processedVariables,
          messages: processedMessages as ActionPromptMessageModel[],
          usageCount: prompt.usageCount || 0,
          timestamp: prompt.updatedAt ? parseISO(prompt.updatedAt) : new Date(),
        };
      });
      
      setActionPrompts(promptItems);
    } catch (error) {
      console.error('Error fetching action prompts:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load action prompts' 
      });
    } finally {
      setLoading(false);
    }
  };

  // Load action prompts on component mount
  useEffect(() => {
    fetchActionPrompts();
  }, []);

  // Utility to truncate text if too long
  const truncateText = (text: string, maxLength: number) => {
    if (!text) return '';
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text;
  };

  // Handle delete confirmation
  const confirmDelete = (prompt: ActionPromptItem) => {
    // Truncate name if it's too long
    const truncatedName = truncateText(prompt.name, 50);
    
    confirmDialog({
      message: `Are you sure you want to delete "${truncatedName}"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: () => deleteActionPrompt(prompt)
    });
  };

  // Delete action prompt
  const deleteActionPrompt = async (prompt: ActionPromptItem) => {
    try {
      await actionPromptService.deleteActionPrompt(prompt.id);
      // If successful, update the UI
      const updatedPrompts = actionPrompts.filter(p => p.id !== prompt.id);
      setActionPrompts(updatedPrompts);
      toast.current?.show({ 
        severity: 'success', 
        summary: 'Success', 
        detail: 'Action prompt deleted successfully' 
      });
    } catch (error) {
      console.error('Error deleting action prompt:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to delete action prompt' 
      });
    }
  };

  // Format timestamp to relative time
  const formatTimestamp = (timestamp: Date) => {
    return formatDistanceToNow(timestamp, { addSuffix: true });
  };
  
  // Template for timestamp column
  const timestampTemplate = (rowData: ActionPromptItem) => {
    return <span className="text-sm">{formatTimestamp(rowData.timestamp)}</span>;
  };

  // Template for usage column
  const usageTemplate = (rowData: ActionPromptItem) => {
    return (
      <span className="flex align-items-center gap-2">
        <i className="pi pi-sync" style={{ fontSize: '0.8rem' }} />
        <span>{rowData.usageCount} {rowData.usageCount === 1 ? 'use' : 'uses'}</span>
      </span>
    );
  };

  // Template for variables column
  const variablesTemplate = (rowData: ActionPromptItem) => {
    // Count variables correctly whether they're in an array or object
    let count = 0;
    if (Array.isArray(rowData.variables)) {
      count = rowData.variables.length;
    } else if (rowData.variables && typeof rowData.variables === 'object') {
      count = Object.keys(rowData.variables).length;
    }
    
    return (
      <span className="flex align-items-center gap-2">
        <i className="pi pi-list" style={{ fontSize: '0.8rem' }} />
        <span>{count}</span>
      </span>
    );
  };

  // Template for messages column
  const messagesTemplate = (rowData: ActionPromptItem) => {
    return (
      <span className="flex align-items-center gap-2">
        <i className="pi pi-comments" style={{ fontSize: '0.8rem' }} />
        <span>{rowData.messages?.length || 0}</span>
      </span>
    );
  };

  // Handle edit button click
  const handleEdit = (prompt: ActionPromptItem) => {
    // Convert array variables to object format for editing
    let variablesObject: Record<string, any> = {};
    
    if (Array.isArray(prompt.variables)) {
      // Convert array to object format
      prompt.variables.forEach(variable => {
        if (variable.name) {
          variablesObject[variable.name] = {
            description: variable.description || '',
            defaultValue: variable.defaultValue || '',
            required: variable.required !== undefined ? variable.required : Boolean(variable.isRequired)
          };
        }
      });
    } else if (prompt.variables && typeof prompt.variables === 'object') {
      // Already in object format
      variablesObject = prompt.variables;
    }
    
    setEditPrompt({
      id: prompt.id,
      name: prompt.name,
      description: prompt.description,
      variables: variablesObject,
      messages: Array.isArray(prompt.messages) ? prompt.messages.map((msg: any) => ({
        role: msg && msg.role ? (typeof msg.role === 'string' ? msg.role : String(msg.role)) : 'User',
        content: msg && msg.content ? (typeof msg.content === 'string' ? msg.content : 
          (typeof msg.content === 'object' && msg.content && 'text' in msg.content) ? 
            msg.content.text || '' : String(msg.content)) : ''
      })) : []
    });
    setDialogVisible(true);
  };

  // Handle duplicate button click
  const handleDuplicate = (prompt: ActionPromptItem) => {
    // Convert array variables to object format for editing (same as in handleEdit)
    let variablesObject: Record<string, any> = {};
    
    if (Array.isArray(prompt.variables)) {
      // Convert array to object format
      prompt.variables.forEach(variable => {
        if (variable.name) {
          variablesObject[variable.name] = {
            description: variable.description || '',
            defaultValue: variable.defaultValue || '',
            required: variable.required !== undefined ? variable.required : Boolean(variable.isRequired)
          };
        }
      });
    } else if (prompt.variables && typeof prompt.variables === 'object') {
      // Already in object format
      variablesObject = prompt.variables;
    }
    
    setEditPrompt({
      name: `${prompt.name} (Copy)`,
      description: prompt.description,
      variables: variablesObject,
      messages: Array.isArray(prompt.messages) ? prompt.messages.map((msg: any) => ({
        role: msg && msg.role ? (typeof msg.role === 'string' ? msg.role : String(msg.role)) : 'User',
        content: msg && msg.content ? (typeof msg.content === 'string' ? msg.content : 
          (typeof msg.content === 'object' && msg.content && 'text' in msg.content) ? 
            msg.content.text || '' : String(msg.content)) : ''
      })) : []
    });
    setDialogVisible(true);
  };

  // Handle new action prompt button click
  const handleNewPrompt = () => {
    setEditPrompt(undefined);
    setDialogVisible(true);
  };

  // Template for actions column using a kebab menu
  const actionsTemplate = (rowData: ActionPromptItem) => {
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
      {dialogVisible && (
        <ActionPromptDialog 
          visible={dialogVisible}
          onHide={() => setDialogVisible(false)}
          onSave={fetchActionPrompts}
          editPrompt={editPrompt}
        />
      )}
      
      <Card className="mb-3 conversation-summary-card">
        <div className="flex flex-column md:flex-row justify-content-between">
          <div className="mb-3 md:mb-0">
            <h2 className="m-0 mb-2 text-xl">Action Prompt Library</h2>
            <p className="m-0 text-sm text-color-secondary">
              Create, manage, and use AI action prompts for various workflows
              {!loading && actionPrompts.length > 0 && ` (${actionPrompts.length} total)`}
            </p>
          </div>
          <div className="flex align-items-center">
            <Button 
              label="Create New Action Prompt" 
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
            <Column body={skeletonTemplate} style={{ width: '35%' }} />
            <Column body={() => <Skeleton width="3rem" height="1.2rem" />} style={{ width: '10%' }} />
            <Column body={() => <Skeleton width="3rem" height="1.2rem" />} style={{ width: '10%' }} />
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
                <h2 className="m-0 text-lg">All Action Prompts</h2>
              </div>
            }
            value={actionPrompts} 
            paginator 
            rows={5} 
            rowsPerPageOptions={[5, 10, 25]} 
            globalFilter={undefined}
            emptyMessage="No action prompts found"
            className="p-datatable-sm p-datatable-responsive-stack"
            sortField="timestamp"
            sortOrder={-1}
            responsiveLayout="stack"
            breakpoint="960px"
            dataKey="id"
            stripedRows
            onRowDoubleClick={(e) => handleEdit(e.data as ActionPromptItem)}
            rowClassName={() => "cursor-pointer"}
          >
            <Column 
              field="name" 
              header="Name" 
              sortable 
              headerClassName="w-2 column-title"
              bodyClassName="font-medium"
            />
            <Column 
              field="description" 
              header="Description" 
              headerClassName="w-4 column-description"
              bodyClassName="text-color-secondary"
            />
            <Column 
              field="variables" 
              header="Variables" 
              body={variablesTemplate} 
              sortable 
              headerClassName="w-1 column-variables"
            />
            <Column 
              field="messages" 
              header="Messages" 
              body={messagesTemplate} 
              sortable 
              headerClassName="w-1 column-messages"
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

export default ActionPrompts;