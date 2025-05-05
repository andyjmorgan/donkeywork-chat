import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';
import { Menu } from 'primereact/menu';
import { Skeleton } from 'primereact/skeleton';
import { Toast } from 'primereact/toast';
import { formatDistanceToNow, parseISO } from 'date-fns';
import { AgentDialog } from '../features/agents/components/AgentDialog';
import { agentService } from '../services/api';
import '../styles/components/ConversationList.css';

// Define Agent interface
interface Agent {
  id: string;
  name: string;
  description: string;
  tags: string[];
  createdAt: string;
  updatedAt: string;
  executionCount?: number;
}

const Agents: React.FC = () => {
  const navigate = useNavigate();
  const [agents, setAgents] = useState<Agent[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [dialogVisible, setDialogVisible] = useState<boolean>(false);
  const [editAgent, setEditAgent] = useState<Agent | undefined>();
  const toast = useRef<Toast>(null);

  // Fetch agents
  const fetchAgents = async () => {
    setLoading(true);
    try {
      // Use the real agent service API
      const response = await agentService.getAgents();
      console.log('API Response:', response);
      
      // The API response has 'agents' property, not 'items'
      // Convert AgentModel to Agent interface
      const agentList: Agent[] = (response.agents || []).map(agent => ({
        id: agent.id || '',
        name: agent.name,
        description: agent.description,
        tags: agent.tags,
        createdAt: agent.createdAt || new Date().toISOString(),
        updatedAt: agent.updatedAt || new Date().toISOString(),
        executionCount: agent.executionCount
      }));
      
      setAgents(agentList);
      console.log('Set agents to:', agentList);
    } catch (error) {
      console.error('Error fetching agents:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load agents' 
      });
    } finally {
      setLoading(false);
    }
  };

  // Load agents on component mount
  useEffect(() => {
    fetchAgents();
  }, []);

  // Utility to truncate text if too long
  const truncateText = (text: string, maxLength: number) => {
    if (!text) return '';
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text;
  };

  // Handle delete confirmation
  const confirmDelete = (agent: Agent) => {
    const truncatedName = truncateText(agent.name, 50);
    
    confirmDialog({
      message: `Are you sure you want to delete "${truncatedName}"?`,
      header: 'Delete Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptClassName: 'p-button-danger',
      accept: () => deleteAgent(agent)
    });
  };

  // Delete agent
  const deleteAgent = async (agent: Agent) => {
    try {
      // Use the real API to delete the agent
      await agentService.deleteAgent(agent.id);
      
      // If successful, update the UI
      const updatedAgents = agents.filter(a => a.id !== agent.id);
      setAgents(updatedAgents);
      toast.current?.show({ 
        severity: 'success', 
        summary: 'Success', 
        detail: 'Agent deleted successfully' 
      });
    } catch (error) {
      console.error('Error deleting agent:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to delete agent' 
      });
    }
  };

  // Format timestamp to relative time
  const formatTimestamp = (timestamp: Date | string) => {
    const dateObj = timestamp instanceof Date ? timestamp : parseISO(timestamp.toString());
    return formatDistanceToNow(dateObj, { addSuffix: true });
  };
  
  // Template for timestamp column
  const timestampTemplate = (rowData: Agent) => {
    return <span className="text-sm">{formatTimestamp(rowData.updatedAt)}</span>;
  };

  // Handle edit button click - navigate to the agent editor
  const handleEdit = (agent: Agent) => {
    // Navigate to the agent editor with the agent ID using React Router
    navigate(`/agents/edit/${agent.id}`);
  };

  // Handle duplicate button click
  const handleDuplicate = (agent: Agent) => {
    const duplicatedAgent: Agent = {
      id: '',  // Empty string instead of undefined for id
      name: `${agent.name} (Copy)`,
      description: agent.description,
      tags: agent.tags,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      executionCount: 0
    };
    setEditAgent(duplicatedAgent);
    setDialogVisible(true);
  };

  // Handle new agent button click - navigate to create new agent
  const handleNewAgent = () => {
    // Navigate to the agent editor to create a new agent using React Router
    navigate('/agents/new');
  };

  // Template for actions column using a kebab menu
  const actionsTemplate = (rowData: Agent) => {
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
        <AgentDialog 
          visible={dialogVisible}
          onHide={() => setDialogVisible(false)}
          onSave={fetchAgents}
          editAgent={editAgent}
        />
      )}
      
      <Card className="mb-3 conversation-summary-card">
        <div className="flex flex-column md:flex-row justify-content-between">
          <div className="mb-3 md:mb-0">
            <h2 className="m-0 mb-2 text-xl">Agents Library</h2>
            <p className="m-0 text-sm text-color-secondary">
              Create, manage, and configure AI agents for various workflows
              {!loading && agents.length > 0 && ` (${agents.length} total)`}
            </p>
          </div>
          <div className="flex align-items-center">
            <Button 
              label="Create New Agent" 
              icon="pi pi-plus" 
              className="p-button-primary" 
              style={{ height: "2.5rem", padding: "0.75rem 1.25rem" }}
              onClick={handleNewAgent}
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
            <Column body={skeletonTemplate} style={{ width: '20%' }} />
            <Column body={skeletonTemplate} style={{ width: '50%' }} />
            <Column body={() => <Skeleton width="5rem" height="1.2rem" />} style={{ width: '20%' }} />
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
                <h2 className="m-0 text-lg">All Agents</h2>
              </div>
            }
            value={agents} 
            paginator 
            rows={5} 
            rowsPerPageOptions={[5, 10, 25]} 
            globalFilter={undefined}
            emptyMessage="No agents found"
            className="p-datatable-sm p-datatable-responsive-stack"
            sortField="updatedAt"
            sortOrder={-1}
            responsiveLayout="stack"
            breakpoint="960px"
            dataKey="id"
            stripedRows
            onRowDoubleClick={(e) => handleEdit(e.data as Agent)}
            rowClassName={() => "cursor-pointer"}
          >
            <Column 
              field="name" 
              header="Name" 
              sortable 
              headerClassName="w-3 column-title"
              bodyClassName="font-medium"
              style={{ width: '20%' }}
            />
            <Column 
              field="description" 
              header="Description" 
              headerClassName="w-6 column-description"
              bodyClassName="text-color-secondary"
              style={{ width: '50%' }}
            />
            <Column 
              field="updatedAt" 
              header="Last Updated" 
              body={timestampTemplate}
              sortable 
              headerClassName="w-2 column-timestamp"
              style={{ width: '20%' }}
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

export default Agents;