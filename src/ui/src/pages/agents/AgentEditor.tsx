import React, { useState, useEffect } from 'react';
import { AgentBuilder } from '../../components/agent/AgentBuilder';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { useNavigate, useParams } from 'react-router-dom';
import { Toast } from 'primereact/toast';
import { useRef } from 'react';
import { agentService } from '../../services/api';
import '../../components/agent/agent-editor.css';

interface AgentEditorProps {
  agentId?: string;
}

const AgentEditor: React.FC = () => {
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [agent, setAgent] = useState<any>(null);
  
  // Get agentId from URL if we're editing
  const { agentId } = useParams<{ agentId: string }>();
  
  // If agentId is provided, fetch the agent data
  useEffect(() => {
    const loadAgent = async () => {
      if (!agentId) {
        return;
      }
      
      try {
        setLoading(true);
        
        // Fetch the agent data from the API
        const agentData = await agentService.getAgent(agentId);
        
        // Map the API agent model to ReactFlow nodes and edges
        const flowNodes = agentData.nodes.map(node => {
          // Create a deep copy of the metadata to avoid reference issues
          const metadata = JSON.parse(JSON.stringify(node.metadata || {}));
          
          // Fix the issue where Output nodes have nodeType of "input" in metadata
          if (node.nodeType.toLowerCase() === 'output' && metadata.nodeType === 'input') {
            metadata.nodeType = 'output';
          }
          
          // Create the data object with proper fields
          const data = {
            ...metadata,
            nodeType: node.nodeType.toLowerCase(),
            label: node.label,
            immutable: metadata.immutable !== undefined ? metadata.immutable : 
                     (node.nodeType === 'Input' || node.nodeType === 'Output')
          };
          
          return {
            id: node.id,
            type: getNodeTypeFromNodeType(node.nodeType),
            data: data,
            position: node.position,
          };
        });
        
        const flowEdges = agentData.nodeEdges.map(edge => ({
          id: edge.id || crypto.randomUUID(),
          source: edge.sourceNodeId,
          target: edge.targetNodeId,
          sourceHandle: edge.sourceNodeHandle || undefined,
          targetHandle: edge.targetNodeHandle || undefined,
        }));
        
        // Store the full agent data including the properly converted nodes and edges
        setAgent({
          ...agentData,
          name: agentData.name,
          description: agentData.description,
          tags: agentData.tags || [],
          id: agentData.id,
          nodes: flowNodes,
          edges: flowEdges
        });
      } catch (error) {
        console.error('Error loading agent:', error);
        toast.current?.show({
          severity: 'error',
          summary: 'Error',
          detail: `Failed to load agent: ${error instanceof Error ? error.message : 'Unknown error'}`
        });
      } finally {
        setLoading(false);
      }
    };
    
    loadAgent();
  }, [agentId]);
  
  // Helper function to convert from AgentNodeType to ReactFlow node type
  const getNodeTypeFromNodeType = (nodeType: string): string => {
    // Convert to lowercase for case-insensitive matching
    const normalizedType = nodeType.toLowerCase();
    
    switch (normalizedType) {
      case 'input':
        return 'inputNode';
      case 'output':
        return 'outputNode';
      case 'model':
        return 'modelNode';
      case 'stringformatter':
        return 'stringFormatterNode';
      case 'conditional':
        return 'conditionalNode';
      default:
        // Fallback for alternative formats
        if (normalizedType.includes('input')) return 'inputNode';
        if (normalizedType.includes('output')) return 'outputNode';
        if (normalizedType.includes('model')) return 'modelNode';
        if (normalizedType.includes('string') || normalizedType.includes('format')) return 'stringFormatterNode';
        if (normalizedType.includes('condition')) return 'conditionalNode';
        
        return 'modelNode'; // Default to model node if all else fails
    }
  };
  
  const handleSave = async (agentJson: string) => {
    try {
      // Parse the JSON to verify it's valid
      const parsedAgent = JSON.parse(agentJson);
      
      // Show success message
      toast.current?.show({
        severity: 'success',
        summary: 'Success',
        detail: 'Agent saved successfully'
      });
      
      // If this was a new agent, navigate to the edit page with the new ID
      if (!agentId && parsedAgent.id) {
        navigate(`/agents/edit/${parsedAgent.id}`, { replace: true });
      }
    } catch (error) {
      console.error('Error parsing agent JSON:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to parse agent JSON' 
      });
    }
  };

  return (
    <div className="agent-editor-container">
      <Toast ref={toast} position="top-right" />
      
      {loading ? (
        <Card>
          <div className="flex justify-content-center align-items-center" style={{ height: '400px' }}>
            <i className="pi pi-spin pi-spinner" style={{ fontSize: '2rem' }}></i>
            <span className="ml-2">Loading agent...</span>
          </div>
        </Card>
      ) : (
        <AgentBuilder
          initialNodes={agent?.nodes}
          initialEdges={agent?.edges}
          onSave={handleSave}
          agentId={agentId}
        />
      )}
    </div>
  );
};

export default AgentEditor;