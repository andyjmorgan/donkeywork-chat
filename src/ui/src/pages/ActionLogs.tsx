import React, { useState, useEffect, useRef } from 'react';
import { Card } from 'primereact/card';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Skeleton } from 'primereact/skeleton';
import { Toast } from 'primereact/toast';
import { Button } from 'primereact/button';
import { formatDistanceToNow, parseISO } from 'date-fns';
import { Tag } from 'primereact/tag';
import { useNavigate } from 'react-router-dom';
import { actionExecutionService } from '../services/api/actionExecutionService';
import { actionService } from '../services/api/actionService';
import { ActionExecutionItem, ActionExecutionStatus } from '../models/api/ActionExecution';
import { ActionItem, UpsertActionModel } from '../models/api/Action';
import { ActionDialog } from '../features/actions/components';
import '../styles/components/ConversationList.css';
import '../styles/components/ActionLogs.css';

const ActionLogs: React.FC = () => {
  const [executions, setExecutions] = useState<ActionExecutionItem[]>([]);
  const [actions, setActions] = useState<{ [key: string]: ActionItem }>({});
  const [loading, setLoading] = useState<boolean>(true);
  const [dialogVisible, setDialogVisible] = useState<boolean>(false);
  const [editAction, setEditAction] = useState<UpsertActionModel & { id?: string }>();
  const toast = useRef<Toast>(null);
  const navigate = useNavigate();

  // Fetch all actions to get icons and details
  const fetchActions = async () => {
    try {
      const result = await actionService.getActions(0, 100);
      const actionsMap: { [key: string]: ActionItem } = {};
      
      result.actions.forEach(action => {
        actionsMap[action.id] = action;
      });
      
      setActions(actionsMap);
      return actionsMap;
    } catch (error) {
      console.error('Error fetching actions:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load actions' 
      });
      return {};
    }
  };

  // Fetch action executions
  const fetchExecutions = async (actionsMap: { [key: string]: ActionItem } = {}) => {
    setLoading(true);
    try {
      const result = await actionExecutionService.getActionExecutions(0, 100);
      
      // Convert date strings to Date objects
      const executionItems: ActionExecutionItem[] = result.actions.map(execution => ({
        ...execution,
        createdAt: execution.createdAt instanceof Date ? execution.createdAt : parseISO(execution.createdAt as string),
        endTime: execution.endTime instanceof Date ? execution.endTime : parseISO(execution.endTime as string)
      }));
      
      setExecutions(executionItems);
    } catch (error) {
      console.error('Error fetching action executions:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load action executions' 
      });
    } finally {
      setLoading(false);
    }
  };

  // Load actions and executions on component mount
  useEffect(() => {
    const loadData = async () => {
      const actionsMap = await fetchActions();
      await fetchExecutions(actionsMap);
    };
    
    loadData();
    
    // No automatic refresh
  }, []);

  // Format timestamp to exact date and time
  const formatTimestamp = (timestamp: Date | string) => {
    const dateObj = timestamp instanceof Date ? timestamp : parseISO(timestamp as string);
    
    // Format date as YYYY-MM-DD HH:MM:SS
    return dateObj.toISOString().replace('T', ' ').substring(0, 19);
  };
  
  // Template for timestamp column
  const timestampTemplate = (rowData: ActionExecutionItem) => {
    return <span className="text-sm">{formatTimestamp(rowData.createdAt)}</span>;
  };
  
  // Template for completion timestamp column
  const completionTimestampTemplate = (rowData: ActionExecutionItem) => {
    if (rowData.executionStatus === ActionExecutionStatus.Pending || 
        rowData.executionStatus === ActionExecutionStatus.InProgress) {
      return <span className="text-sm">In progress</span>;
    }
    return <span className="text-sm">{formatTimestamp(rowData.endTime)}</span>;
  };

  // Template for duration column
  const durationTemplate = (rowData: ActionExecutionItem) => {
    if (rowData.executionStatus === ActionExecutionStatus.Pending || 
        rowData.executionStatus === ActionExecutionStatus.InProgress) {
      return <span className="text-sm">In progress</span>;
    }
    
    const startDate = rowData.createdAt instanceof Date ? rowData.createdAt : parseISO(rowData.createdAt as string);
    const endDate = rowData.endTime instanceof Date ? rowData.endTime : parseISO(rowData.endTime as string);
    
    // Calculate duration in seconds
    const durationMs = endDate.getTime() - startDate.getTime();
    const durationSec = Math.round(durationMs / 1000);
    
    if (durationSec < 60) {
      return <span className="text-sm">{durationSec}s</span>;
    } else {
      const minutes = Math.floor(durationSec / 60);
      const seconds = durationSec % 60;
      return <span className="text-sm">{minutes}m {seconds}s</span>;
    }
  };

  // Template for status column
  const statusTemplate = (rowData: ActionExecutionItem) => {
    let severity;
    
    switch (rowData.executionStatus) {
      case ActionExecutionStatus.Completed:
        severity = 'success';
        break;
      case ActionExecutionStatus.Failed:
        severity = 'danger';
        break;
      case ActionExecutionStatus.Canceled:
        severity = 'warning';
        break;
      case ActionExecutionStatus.InProgress:
        severity = 'info';
        break;
      case ActionExecutionStatus.Pending:
      default:
        severity = 'secondary';
        break;
    }
    
    return <Tag value={rowData.executionStatus} severity={severity} />;
  };
  
  // Template for action icon
  const iconTemplate = (rowData: ActionExecutionItem) => {
    const action = actions[rowData.actionId];
    
    if (!action || !action.icon) {
      return <i className="pi pi-bolt text-lg" style={{ color: 'var(--primary-color)' }} />;
    }
    
    // Check if the icon is a URL
    if (action.icon.startsWith('http')) {
      return <img src={action.icon} alt={action.name} className="action-icon" style={{ width: '24px', height: '24px', objectFit: 'contain' }} />;
    }
    
    // Assume it's a PrimeReact icon class
    return <i className={`${action.icon} text-lg`} style={{ color: 'var(--primary-color)' }} />;
  };
  
  // Template for action name (clickable) with icon
  const actionNameTemplate = (rowData: ActionExecutionItem) => {
    const handleClick = (e: React.MouseEvent) => {
      e.preventDefault();
      
      const action = actions[rowData.actionId];
      if (action) {
        // Prepare action for editing
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
      }
    };
    
    const action = actions[rowData.actionId];
    const displayName = action ? action.name : rowData.actionName;
    
    // Generate icon based on action
    let icon;
    if (action && action.icon) {
      if (action.icon.startsWith('http')) {
        icon = <img src={action.icon} alt={action.name} className="action-icon mr-2" style={{ width: '18px', height: '18px', objectFit: 'contain', verticalAlign: 'text-bottom' }} />;
      } else {
        icon = <i className={`${action.icon} mr-2`} style={{ color: 'var(--primary-color)', fontSize: '16px' }}></i>;
      }
    } else {
      icon = <i className="pi pi-bolt mr-2" style={{ color: 'var(--primary-color)', fontSize: '16px' }}></i>;
    }
    
    return (
      <a 
        href="#"
        onClick={handleClick}
        className="cursor-pointer text-primary hover:underline flex align-items-center"
        title="View action details"
      >
        {icon}
        <span>{displayName}</span>
      </a>
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
          onHide={() => {
            setDialogVisible(false);
            fetchActions(); // Refresh the actions data when dialog is closed
          }}
          onSave={() => fetchActions()}
          editAction={editAction}
        />
      )}
      
      <Card className="mb-3 conversation-summary-card">
        <div className="flex flex-column md:flex-row justify-content-between">
          <div className="mb-3 md:mb-0">
            <h2 className="m-0 mb-2 text-xl">Action Logs</h2>
            <p className="m-0 text-sm text-color-secondary">
              View history and status of all action executions
              {!loading && executions.length > 0 && ` (${executions.length} total)`}
            </p>
          </div>
          <div className="flex align-items-center gap-2">
            <Button 
              label="Refresh" 
              icon="pi pi-refresh" 
              className="p-button-outlined mr-2" 
              style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
              onClick={async () => {
                setLoading(true);
                await fetchActions();
                await fetchExecutions();
                setLoading(false);
              }}
            />
            <Button 
              label="View Actions" 
              icon="pi pi-external-link" 
              className="p-button-outlined" 
              style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
              onClick={() => navigate('/actions')}
            />
          </div>
        </div>
      </Card>
      
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
            <Column body={() => (
              <div className="flex align-items-center gap-2">
                <Skeleton shape="circle" size="1.5rem" />
                <Skeleton width="70%" height="1.2rem" />
              </div>
            )} style={{ width: '25%' }} />
            <Column body={() => <Skeleton width="5rem" height="1.2rem" />} style={{ width: '15%' }} />
            <Column body={() => <Skeleton width="10rem" height="1.2rem" />} style={{ width: '20%' }} />
            <Column body={() => <Skeleton width="10rem" height="1.2rem" />} style={{ width: '20%' }} />
            <Column body={() => <Skeleton width="5rem" height="1.2rem" />} style={{ width: '15%' }} />
          </DataTable>
        </Card>
      ) : (
        <Card>
          <DataTable
            header={
              <div className="flex justify-content-between align-items-center">
                <h2 className="m-0 text-lg">Execution History</h2>
              </div>
            }
            value={executions} 
            paginator 
            rows={10} 
            rowsPerPageOptions={[10, 25, 50]} 
            globalFilter={undefined}
            emptyMessage="No action executions found"
            className="p-datatable-sm p-datatable-responsive-stack"
            sortField="createdAt"
            sortOrder={-1}
            responsiveLayout="stack"
            breakpoint="960px"
            dataKey="id"
            stripedRows
            rowClassName={(data) => {
              if (data.executionStatus === ActionExecutionStatus.Failed) {
                return 'execution-failed';
              }
              return '';
            }}
          >
            <Column 
              field="actionName" 
              header="Action Name" 
              body={actionNameTemplate}
              sortable 
              headerClassName="w-3 column-title"
              bodyClassName="font-medium"
              style={{ width: '25%' }}
            />
            <Column 
              field="executionStatus" 
              header="Status" 
              body={statusTemplate} 
              sortable 
              headerClassName="w-2 column-status"
              style={{ width: '15%' }}
            />
            <Column 
              field="createdAt" 
              header="Time" 
              body={timestampTemplate}
              sortable 
              headerClassName="w-3 column-timestamp"
              style={{ width: '20%' }}
            />
            <Column 
              field="endTime" 
              header="End Time" 
              body={completionTimestampTemplate}
              sortable 
              headerClassName="w-3 column-timestamp"
              style={{ width: '20%' }}
            />
            <Column 
              field="duration" 
              header="Duration" 
              body={durationTemplate}
              headerClassName="w-2 column-duration"
              style={{ width: '15%' }}
            />
          </DataTable>
        </Card>
      )}
    </div>
  );
};

export default ActionLogs;