import { useState, useCallback, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Chips } from 'primereact/chips';
import { Message } from 'primereact/message';
import { Toast } from 'primereact/toast';
import { AgentModel, AgentNodeEdgeModel, AgentNode, AgentNodePosition, agentService } from '../../services/api/agentService';
import { BaseNodeData } from '../../types/nodes';
import ReactFlow, { 
  Background, 
  useNodesState,
  useEdgesState,
  addEdge,
  Connection,
  Edge,
  ReactFlowProvider,
  Node,
  BackgroundVariant
} from 'reactflow';
import { nodeTypes } from '../nodes';
import { nodeEvents } from '../../utils/nodeEvents';
import { validateAgent, ValidationError } from '../../utils/nodeValidation';
import { NodeTypeEnum } from '../../types/nodes';
import 'reactflow/dist/style.css';
import '../nodes/nodeStyles.css';
import './AgentBuilder.css';
import './dialog-styles.css';

interface AgentBuilderProps {
  initialNodes?: Node[];
  initialEdges?: Edge[];
  onSave?: (agentJson: string) => void;
  agentId?: string; // Optional agent ID for editing existing agents
}

export const AgentBuilder = ({ 
  initialNodes, 
  initialEdges, 
  onSave,
  agentId: initialAgentId
}: AgentBuilderProps) => {
  const navigate = useNavigate();
  // Default initial nodes if none provided
  const defaultInitialNodes: Node[] = [
    {
      id: crypto.randomUUID(), // Use UUID
      type: 'inputNode',
      data: { 
        label: 'Input',
        nodeType: NodeTypeEnum.INPUT,
        immutable: true
      },
      position: { x: 250, y: 100 },
    },
    {
      id: crypto.randomUUID(), // Use UUID
      type: 'outputNode',
      data: { 
        label: 'Output',
        nodeType: NodeTypeEnum.OUTPUT,
        immutable: true 
      },
      position: { x: 250, y: 200 },
    },
  ];

  // We need to store the node IDs to properly create edges
  const inputNodeId = defaultInitialNodes[0].id;
  const outputNodeId = defaultInitialNodes[1].id;
  
  const defaultInitialEdges = [
    { id: crypto.randomUUID(), source: inputNodeId, target: outputNodeId }
  ];

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes || defaultInitialNodes);
  const [edges, setEdges, onEdgesChangeOriginal] = useEdgesState(initialEdges || defaultInitialEdges);
  // Remove nodeId state as we're using UUIDs now
  const [reactFlowInstance, setReactFlowInstance] = useState<any>(null);
  const reactFlowWrapper = useRef<HTMLDivElement>(null);
  const [selectedElements, setSelectedElements] = useState<{nodes: Node[], edges: Edge[]}>({nodes: [], edges: []});
  
  // Remove attribution
  const proOptions = { hideAttribution: true };
  
  // State for node label editing modal
  const [labelModalVisible, setLabelModalVisible] = useState(false);
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [editingLabel, setEditingLabel] = useState('');
  const [labelError, setLabelError] = useState<string | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  
  // State for JSON dialog
  const [jsonDialogVisible, setJsonDialogVisible] = useState(false);
  const [agentJson, setAgentJson] = useState('');
  
  // State for import JSON dialog
  const [importJsonDialogVisible, setImportJsonDialogVisible] = useState(false);
  const [importJsonText, setImportJsonText] = useState('');
  
  // State for validation dialog
  const [validationDialogVisible, setValidationDialogVisible] = useState(false);
  const [validationErrors, setValidationErrors] = useState<ValidationError[]>([]);
  
  // State for agent configuration
  // Generate a unique agent name with a timestamp suffix if no initial agent ID
  const [agentName, setAgentName] = useState(() => {
    if (initialNodes && initialNodes.length > 0) {
      // Check if a name was provided with the initial data
      if (initialNodes[0].data.agentName) {
        return initialNodes[0].data.agentName;
      }
    }
    
    // Generate a unique name for new agents
    if (!initialAgentId) {
      const timestamp = new Date().toISOString().slice(0, 10).replace(/-/g, '');
      return `Agent_${timestamp}_${Math.floor(Math.random() * 1000)}`;
    }
    return 'MyAgent';
  });
  const [agentDescription, setAgentDescription] = useState('');
  const [agentTags, setAgentTags] = useState<string[]>([]);
  const [agentConfigDialogVisible, setAgentConfigDialogVisible] = useState(false);
  const [agentNameError, setAgentNameError] = useState<string | null>(null);
  // Only use the initialAgentId if it's provided - don't generate a random one for new agents
  const [agentId, setAgentId] = useState<string | undefined>(initialAgentId);
  const [isSaving, setIsSaving] = useState(false);
  
  // Toast reference for notifications
  const toast = useRef<Toast>(null);
  
  // Subscribe to node update events
  useEffect(() => {
    // Subscribe to node update events
    const unsubscribe = nodeEvents.subscribe((nodeId, newData) => {
      // Update the node with new data
      setNodes(nodes.map(node => {
        if (node.id === nodeId) {
          return {
            ...node,
            data: {
              ...node.data,
              ...newData
            }
          };
        }
        return node;
      }));
    });
    
    // Clean up on unmount
    return () => {
      unsubscribe();
    };
  }, [nodes, setNodes]);
  
  // Load agent data if initialAgentId is provided (existing agent)
  useEffect(() => {
    if (initialAgentId) {
      // If we have an existing agent, try to fetch its details from the API
      const loadAgentProperties = async () => {
        try {
          const agent = await agentService.getAgent(initialAgentId);
          // Update agent properties
          setAgentName(agent.name);
          setAgentDescription(agent.description);
          setAgentTags(agent.tags || []);
        } catch (error) {
          console.error('Failed to load agent properties:', error);
        }
      };
      
      loadAgentProperties();
    }
  }, [initialAgentId, initialNodes, initialEdges]);
  
  const onSelectionChange = useCallback((params: { nodes?: Node[], edges?: Edge[] }) => {
    const newNodes = params.nodes || [];
    const newEdges = params.edges || [];
    
    if (JSON.stringify(selectedElements.nodes) !== JSON.stringify(newNodes) ||
        JSON.stringify(selectedElements.edges) !== JSON.stringify(newEdges)) {
      setSelectedElements({ nodes: newNodes, edges: newEdges });
    }
  }, [selectedElements]);

  const onEdgesChange = useCallback((changes: any[]) => {
    const filteredChanges = changes.filter((change: any) => {
      if (change.type === 'remove') {
        // If an edge is explicitly selected, allow the deletion
        if (selectedElements.edges.length > 0) {
          return true;
        }
        
        // Check if an immutable node is selected
        const hasImmutableNodeSelected = selectedElements.nodes.some(
          selectedNode => selectedNode.data.immutable === true
        );
        
        // Prevent edge deletion if an immutable node is selected but no edge is explicitly selected
        if (hasImmutableNodeSelected) {
          return false;
        }
      }
      
      // Allow all other change types
      return true;
    });
    
    onEdgesChangeOriginal(filteredChanges);
  }, [selectedElements, onEdgesChangeOriginal]);
  
  const onConnect = useCallback(
    (params: Connection | Edge) => {
      if (!params.source || !params.target) {
        return;
      }
      
      // Create a new connection with a valid UUID instead of letting ReactFlow generate an ID
      const connection = {
        ...params,
        id: crypto.randomUUID() // Ensure a valid UUID is used for the edge ID
      };
      
      setEdges((eds) => addEdge(connection, eds));
    },
    [setEdges]
  );
  
  const onInit = useCallback((reactFlowInstance: any) => {
    setReactFlowInstance(reactFlowInstance);
  }, []);
  
  const onDragOver = useCallback((event: React.DragEvent) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  }, []);
  
  // Track when drag starts and ends to avoid opening modal during drag operations
  const onNodeDragStart = useCallback(() => {
    setIsDragging(true);
  }, []);
  
  const onNodeDragStop = useCallback(() => {
    // Use setTimeout to ensure click events are processed after drag state is updated
    setTimeout(() => {
      setIsDragging(false);
    }, 100);
  }, []);
  
  // Handle node click to open the label editing modal
  // Function to validate node labels
  const validateLabel = useCallback((label: string): string | null => {
    // Validate not empty
    if (!label.trim()) {
      return 'Label cannot be empty';
    }
    
    // Validate the label format
    const labelPattern = /^[a-zA-Z0-9_-]+$/;
    if (!labelPattern.test(label)) {
      return 'Label must only contain alphanumeric characters, underscores, and hyphens';
    }
    
    // Prevent using reserved names "Input" and "Output" for model nodes
    if (label === 'Input' || label === 'Output') {
      return `"${label}" is a reserved name. Please choose a different label`;
    }
    
    // Check for duplicate labels across all nodes
    if (selectedNodeId) {
      const isDuplicate = nodes.some(node => 
        node.id !== selectedNodeId && 
        node.data.label === label
      );
      
      if (isDuplicate) {
        return 'This label is already in use. Please choose a unique label';
      }
    }
    
    return null; // No errors
  }, [selectedNodeId, nodes]);
  
  // Function to validate agent name
  const validateAgentName = useCallback((name: string): string | null => {
    // Validate not empty
    if (!name.trim()) {
      return 'Agent name cannot be empty';
    }
    
    // Validate the name format - same rules as node labels
    const namePattern = /^[a-zA-Z0-9_-]+$/;
    if (!namePattern.test(name)) {
      return 'Agent name must only contain alphanumeric characters, underscores, and hyphens';
    }
    
    return null; // No errors
  }, []);
  
  // Open the label editing modal
  const openLabelEditor = useCallback((nodeId: string) => {
    const node = nodes.find(n => n.id === nodeId);
    if (node && !node.data.immutable) {
      // Delay longer to ensure all other events have completed
      setTimeout(() => {
        setSelectedNodeId(nodeId);
        setEditingLabel(node.data.label);
        setLabelError(null); // Reset error state
        setLabelModalVisible(true);
      }, 100);
    }
  }, [nodes]);
  
  const onNodeClick = useCallback((event: React.MouseEvent, node: Node) => {
    // Don't handle click if we're dragging or if node is immutable
    if (isDragging || node.data.immutable === true) {
      return;
    }
    
    // For all node types, check if the click was on the node title
    const isNodeTitle = (event.target as HTMLElement).closest('.node-title');
    if (isNodeTitle) {
      // Stop event propagation to prevent other handlers from firing
      event.stopPropagation();
      // Open the label editor
      openLabelEditor(node.id);
      return;
    }
    
    // Handle other node-specific clicks
    if (node.type === 'stringFormatterNode' || node.type === 'conditionalNode') {
      // These nodes have their own click handlers for their content
      return;
    }
    
  }, [isDragging, openLabelEditor]);
  
  // Handle input change with validation
  const handleLabelChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setEditingLabel(newValue);
    setLabelError(validateLabel(newValue));
  }, [validateLabel]);
  
  // Handle updating the node label
  const updateNodeLabel = useCallback(() => {
    if (!selectedNodeId) {
      return;
    }
    
    // Validate the label one more time before saving
    const error = validateLabel(editingLabel);
    if (error) {
      setLabelError(error);
      return;
    }
    
    setNodes(nodes.map(node => {
      if (node.id === selectedNodeId) {
        return {
          ...node,
          data: {
            ...node.data,
            label: editingLabel
          }
        };
      }
      return node;
    }));
    
    // Close the modal
    setLabelModalVisible(false);
  }, [selectedNodeId, editingLabel, nodes, setNodes, validateLabel]);
  
  // Helper function to validate if a node label follows the pattern and is unique
  const isValidNodeLabel = useCallback((label: string, existingNodes: Node[]): boolean => {
    // Check if label follows the pattern: alphanumeric, underscores, and hyphens only
    const labelPattern = /^[a-zA-Z0-9_-]+$/;
    if (!labelPattern.test(label)) {
      return false;
    }
    
    // Prevent model nodes from using reserved names "Input" and "Output"
    if (label === 'Input' || label === 'Output') {
      return false;
    }
    
    // Check if label is unique among all nodes
    return !existingNodes.some(node => node.data.label === label);
  }, []);
  
  // Helper function to generate a unique valid label
  const getUniqueLabel = useCallback((baseLabel: string, existingNodes: Node[]): string => {
    // For model nodes, avoid using reserved names
    if (baseLabel === 'Input' || baseLabel === 'Output') {
      baseLabel = 'Model'; // Try to use simple 'Model' first
    }
    
    // If there are invalid characters, sanitize them
    let sanitizedLabel = baseLabel.replace(/[^a-zA-Z0-9_-]/g, '_');
    
    // If the sanitized label is already valid and unique, return it
    if (isValidNodeLabel(sanitizedLabel, existingNodes)) {
      return sanitizedLabel;
    }
    
    // Check if the label already has a numeric suffix
    const match = sanitizedLabel.match(/^(.+)_(\d+)$/);
    let basePart, counter;
    
    if (match) {
      // If it has a suffix, extract the base part and start counter from that number
      basePart = match[1];
      counter = parseInt(match[2], 10) + 1;
    } else {
      // Otherwise, use the whole label as base and start counter from 1
      basePart = sanitizedLabel;
      counter = 1;
    }
    
    // Find the next available unique label
    let uniqueLabel = `${basePart}_${counter}`;
    while (!isValidNodeLabel(uniqueLabel, existingNodes)) {
      counter++;
      uniqueLabel = `${basePart}_${counter}`;
    }
    
    return uniqueLabel;
  }, [isValidNodeLabel]);

  const onDrop = useCallback(
    (event: React.DragEvent) => {
      event.preventDefault();
      
      const reactFlowBounds = reactFlowWrapper.current?.getBoundingClientRect();
      if (!reactFlowBounds) return;
      
      const type = event.dataTransfer.getData('application/reactflow');
      
      // Check if the dropped element is valid
      if (!type) {
        return;
      }
      
      if (!reactFlowInstance) return;
      
      // Prevent adding multiple input or output nodes
      if (type === 'inputNode') {
        const existingInputNode = nodes.find(node => node.type === 'inputNode');
        if (existingInputNode) {
          alert('Only one Input node is allowed in the pipeline.');
          return;
        }
      }
      
      if (type === 'outputNode') {
        const existingOutputNode = nodes.find(node => node.type === 'outputNode');
        if (existingOutputNode) {
          alert('Only one Output node is allowed in the pipeline.');
          return;
        }
      }
      
      const position = reactFlowInstance.project({
        x: event.clientX - reactFlowBounds.left,
        y: event.clientY - reactFlowBounds.top,
      });
      
      // Generate node data based on type
      let nodeData = {};
      let baseLabel = '';
      
      switch (type) {
        case 'inputNode':
          baseLabel = 'Input';
          nodeData = {
            label: baseLabel,
            nodeType: NodeTypeEnum.INPUT,
            immutable: true,
            description: 'Input node'
          };
          break;
        case 'modelNode':
          // Try to use simple name "Model" first
          baseLabel = 'Model';
          
          // If "Model" exists, find the next available name with an incrementing number
          if (nodes.some(node => node.data.label === 'Model')) {
            let index = 1;
            while (nodes.some(node => node.data.label === `Model_${index}`)) {
              index++;
            }
            baseLabel = `Model_${index}`;
          }
          
          nodeData = {
            label: baseLabel,
            nodeType: NodeTypeEnum.MODEL,
            immutable: false,
            description: 'Processing node'
          };
          break;
        case 'outputNode':
          baseLabel = 'Output';
          nodeData = {
            label: baseLabel,
            nodeType: NodeTypeEnum.OUTPUT,
            immutable: true,
            description: 'Output node'
          };
          break;
        case 'stringFormatterNode':
          // Try to use simple name "Format" first
          baseLabel = 'Format';
          
          // If "Format" exists, find the next available name with an incrementing number
          if (nodes.some(node => node.data.label === 'Format')) {
            let index = 1;
            while (nodes.some(node => node.data.label === `Format_${index}`)) {
              index++;
            }
            baseLabel = `Format_${index}`;
          }
          
          nodeData = {
            label: baseLabel,
            nodeType: NodeTypeEnum.STRING_FORMATTER,
            immutable: false,
            description: 'String formatter node',
            template: ''  // Empty template by default
          };
          break;
        case 'conditionalNode':
          // Try to use simple name "Condition" first
          baseLabel = 'Condition';
          
          // If "Condition" exists, find the next available name with an incrementing number
          if (nodes.some(node => node.data.label === 'Condition')) {
            let index = 1;
            while (nodes.some(node => node.data.label === `Condition_${index}`)) {
              index++;
            }
            baseLabel = `Condition_${index}`;
          }
          
          nodeData = {
            label: baseLabel,
            nodeType: NodeTypeEnum.CONDITIONAL,
            immutable: false,
            description: 'Conditional block node',
            conditions: [
              { id: crypto.randomUUID(), expression: '' }  // Single empty condition by default
            ]
          };
          break;
        default:
          console.error('Unknown node type:', type);
          return;
      }
      
      // For non-immutable nodes, ensure the label is valid and unique
      // For input and output nodes, we don't need to validate as they're fixed
      if (type === 'modelNode' || type === 'stringFormatterNode' || type === 'conditionalNode') {
        const validLabel = getUniqueLabel(baseLabel, nodes);
        nodeData = {
          ...nodeData,
          label: validLabel
        };
      }
      
      const newNode = {
        id: crypto.randomUUID(), // Use UUID instead of incrementing numbers
        type,
        position,
        data: nodeData,
      };
      
      setNodes((nds) => nds.concat(newNode));
    },
    [reactFlowInstance, setNodes, nodes, getUniqueLabel]
  );
  

  // Function to import agent from JSON
  const importFromJson = useCallback(() => {
    try {
      // Parse the JSON
      const importedAgent = JSON.parse(importJsonText);
      
      if (!importedAgent || !importedAgent.nodes || !Array.isArray(importedAgent.nodes)) {
        throw new Error('Invalid agent JSON format');
      }
      
      // Convert the imported AgentNodes back to ReactFlow nodes
      const importedNodes = importedAgent.nodes.map((agentNode: AgentNode) => ({
        id: agentNode.id,
        type: getNodeTypeFromEnum(agentNode.nodeType),
        data: {
          ...agentNode.metadata,
          // Ensure data.label is always set from the agentNode.label for consistency
          label: agentNode.label
        },
        position: agentNode.position,
      }));
      
      // Convert the imported edges back to ReactFlow edges
      const importedEdges = importedAgent.nodeEdges.map((edge: AgentNodeEdgeModel) => ({
        id: edge.id || crypto.randomUUID(),
        source: edge.sourceNodeId,
        target: edge.targetNodeId,
        sourceHandle: edge.sourceNodeHandle || undefined,
        targetHandle: edge.targetNodeHandle || undefined,
      }));
      
      // Update the agent metadata
      setAgentId(importedAgent.id);
      setAgentName(importedAgent.name || 'Imported Agent');
      setAgentDescription(importedAgent.description || '');
      setAgentTags(importedAgent.tags || []);
      
      // Set the nodes and edges
      setNodes(importedNodes);
      setEdges(importedEdges);
      
      // Close the dialog
      setImportJsonDialogVisible(false);
      setImportJsonText('');
      
      toast.current?.show({
        severity: 'success',
        summary: 'Success',
        detail: 'Agent imported successfully!'
      });
    } catch (error) {
      console.error('Error importing agent:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: `Failed to import agent: ${error instanceof Error ? error.message : 'Invalid JSON format'}`
      });
    }
  }, [importJsonText, setNodes, setEdges]);
  
  // Helper function to convert NodeTypeEnum to ReactFlow node type
  const getNodeTypeFromEnum = useCallback((nodeType: NodeTypeEnum) => {
    switch (nodeType) {
      case NodeTypeEnum.INPUT:
        return 'inputNode';
      case NodeTypeEnum.OUTPUT:
        return 'outputNode';
      case NodeTypeEnum.MODEL:
        return 'modelNode';
      case NodeTypeEnum.STRING_FORMATTER:
        return 'stringFormatterNode';
      case NodeTypeEnum.CONDITIONAL:
        return 'conditionalNode';
      default:
        return 'modelNode'; // Default to model node if unknown
    }
  }, []);

  const saveAgent = useCallback(async () => {
    try {
      setIsSaving(true);
      
      // First validate the agent
      const validationResult = validateAgent(nodes, edges);
      if (!validationResult.valid && validationResult.errors.length > 0) {
        // Show validation errors
        setValidationErrors(validationResult.errors);
        setValidationDialogVisible(true);
        setIsSaving(false);
        return;
      }

      // Get raw nodes for saving
      const rawNodes = nodes.map(node => ({
        id: node.id,
        type: node.type,
        data: node.data,
        position: node.position
      }));
      
      // Get all edges
      const rawEdges = edges.map(edge => ({
        id: edge.id,
        source: edge.source,
        target: edge.target,
        sourceHandle: edge.sourceHandle,
        targetHandle: edge.targetHandle
      }));
      
      // Create a temporary object containing all data (including the original format for compatibility)
      const agentData = {
        id: agentId, 
        name: agentName,
        description: agentDescription,
        tags: agentTags,
        nodes: rawNodes,
        nodeEdges: rawEdges,
        agent: {
          name: agentName,
          description: agentDescription
        }
      };
      
      // Convert raw nodes to AgentNode array
      const agentNodes = rawNodes.map(node => {
        // Create a copy of the node data to modify for metadata
        const metadata = {...node.data};
        
        // Ensure proper format of allowedTools for model nodes
        if (metadata.nodeType === 'model' && Array.isArray(metadata.allowedTools)) {
          // Convert tool IDs to proper format if needed
          metadata.allowedTools = metadata.allowedTools.map(toolId => {
            if (typeof toolId === 'string') {
              // Handle special cases for DateTime and Delay
              if (toolId.toLowerCase() === 'datetime') return 'DateTime';
              if (toolId.toLowerCase() === 'delay') return 'Delay';
              
              // For other values, use the original format
              return toolId;
            }
            return toolId;
          });
        }
        
        return {
          id: node.id,
          label: node.data.label, // Include the label field
          nodeType: node.data.nodeType,
          position: { 
            x: node.position.x,
            y: node.position.y
          },
          metadata: metadata // Use the modified node.data as metadata
        };
      });
      
      // Convert to the AgentModel format
      const agentModel: AgentModel = {
        id: agentId || crypto.randomUUID(),
        name: agentName,
        description: agentDescription,
        tags: agentTags || [],
        // Pass the array of AgentNode objects
        nodes: agentNodes,
        nodeEdges: rawEdges.map(edge => ({
          id: edge.id,
          sourceNodeId: edge.source,
          targetNodeId: edge.target,
          sourceNodeHandle: edge.sourceHandle || null,
          targetNodeHandle: edge.targetHandle || null
        }))
      };
      
      // Call the API to save the agent
      try {
        const savedAgent = await agentService.saveAgent(agentModel);
        
        // Update the agentId if this was a new agent
        if (!agentId) {
          setAgentId(savedAgent.id);
        }
        
        // If onSave callback was provided, call it with the JSON string of the new model
        if (onSave) {
          onSave(JSON.stringify(savedAgent));
        }
        
        // Show success message
        toast.current?.show({
          severity: 'success',
          summary: 'Success',
          detail: `Agent '${agentName}' saved successfully`
        });
      } catch (error) {
        console.error('API error saving agent:', error);
        toast.current?.show({
          severity: 'error',
          summary: 'Error',
          detail: `Failed to save agent: ${error instanceof Error ? error.message : 'Unknown error'}`
        });
      }
    } catch (error) {
      console.error('Error saving agent:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to save agent'
      });
    } finally {
      setIsSaving(false);
    }
  }, [nodes, edges, agentName, agentDescription, agentTags, agentId, onSave, setAgentId]);
  
  // Create a custom header component with title, palette and save button
  const renderCardHeader = () => {
    return (
      <div className="agent-card-header">
        <div className="agent-title">
          <span className="agent-name">{agentName}</span>
          <i 
            className="pi pi-cog agent-config-icon" 
            style={{ marginLeft: '8px', cursor: 'pointer' }}
            onClick={() => setAgentConfigDialogVisible(true)}
            title="Configure agent"
          ></i>
        </div>
        <div className="agent-toolbar">
          <div className="node-palette">
            <div 
              className="node-item model-node-item"
              draggable
              onDragStart={(event) => {
                event.dataTransfer.setData('application/reactflow', 'modelNode');
              }}
            >
              <i className="pi pi-cog" style={{ marginRight: '8px' }}></i>
              <span>Model</span>
            </div>
            
            <div 
              className="node-item string-formatter-node-item"
              draggable
              onDragStart={(event) => {
                event.dataTransfer.setData('application/reactflow', 'stringFormatterNode');
              }}
            >
              <i className="pi pi-pencil" style={{ marginRight: '8px' }}></i>
              <span>Format</span>
            </div>
            
            <div 
              className="node-item conditional-node-item"
              draggable
              onDragStart={(event) => {
                event.dataTransfer.setData('application/reactflow', 'conditionalNode');
              }}
            >
              <i className="pi pi-code" style={{ marginRight: '8px' }}></i>
              <span>Condition</span>
            </div>
          </div>
          <Button 
            label="Import JSON" 
            icon="pi pi-upload" 
            className="p-button-sm p-button-info" 
            onClick={() => setImportJsonDialogVisible(true)}
          />
          <Button 
            label="Export JSON" 
            icon="pi pi-download" 
            className="p-button-sm p-button-info" 
            onClick={() => {
              // Generate the agent JSON and show it in the dialog
              try {
                // Create the raw nodes and edges
                const rawNodes = nodes.map(node => ({
                  id: node.id,
                  type: node.type,
                  data: node.data,
                  position: node.position
                }));
                
                const rawEdges = edges.map(edge => ({
                  id: edge.id,
                  source: edge.source,
                  target: edge.target,
                  sourceHandle: edge.sourceHandle,
                  targetHandle: edge.targetHandle
                }));
                
                // Convert to agent nodes
                const agentNodes = rawNodes.map(node => {
                  // Create a copy of the node data to modify for metadata
                  const metadata = {...node.data};
                  
                  // Ensure proper format of allowedTools for model nodes
                  if (metadata.nodeType === 'model' && Array.isArray(metadata.allowedTools)) {
                    // Convert tool IDs to proper format if needed
                    metadata.allowedTools = metadata.allowedTools.map(toolId => {
                      if (typeof toolId === 'string') {
                        // Handle special cases for DateTime and Delay
                        if (toolId.toLowerCase() === 'datetime') return 'DateTime';
                        if (toolId.toLowerCase() === 'delay') return 'Delay';
                        
                        // For other values, use the original format
                        return toolId;
                      }
                      return toolId;
                    });
                  }
                  
                  return {
                    id: node.id,
                    label: node.data.label, // Include the label field
                    nodeType: node.data.nodeType,
                    position: { 
                      x: node.position.x,
                      y: node.position.y
                    },
                    metadata: metadata // Use the modified node.data as metadata
                  };
                });
                
                // Create the agent model
                const agentModel: AgentModel = {
                  id: agentId || crypto.randomUUID(),
                  name: agentName,
                  description: agentDescription,
                  tags: agentTags || [],
                  nodes: agentNodes,
                  nodeEdges: rawEdges.map(edge => ({
                    id: edge.id,
                    sourceNodeId: edge.source,
                    targetNodeId: edge.target,
                    sourceNodeHandle: edge.sourceHandle || null,
                    targetNodeHandle: edge.targetHandle || null
                  }))
                };
                
                // Set the JSON for display
                setAgentJson(JSON.stringify(agentModel, null, 2));
                setJsonDialogVisible(true);
              } catch (error) {
                console.error('Error generating agent JSON:', error);
                toast.current?.show({
                  severity: 'error',
                  summary: 'Error',
                  detail: 'Failed to generate agent JSON'
                });
              }
            }}
          />
          <Button 
            label="Exit" 
            icon="pi pi-times" 
            className="p-button-sm p-button-secondary" 
            onClick={() => {
              // Handle exit - navigate back to the agents list without page reload
              navigate('/agents');
            }} 
          />
          <Button 
            label="Save Agent" 
            icon="pi pi-save" 
            className="p-button-sm p-button-success" 
            onClick={saveAgent}
            loading={isSaving}
            disabled={isSaving}
          />
        </div>
      </div>
    );
  };

  return (
    <div className="agent-builder-container">
      <Toast ref={toast} />
      <Card header={renderCardHeader()} className="agent-card">
        <div className="flow-container" ref={reactFlowWrapper}>
          <ReactFlowProvider>
            <div style={{ width: '100%', height: '100%' }}>
              <ReactFlow
              nodes={nodes}
              edges={edges}
              nodeTypes={nodeTypes}
              proOptions={proOptions}
              onNodesChange={(changes) => {
                // Filter out any attempts to remove nodes that cannot be deleted
                const filteredChanges = changes.filter((change: any) => {
                  // If it's a remove operation
                  if (change.type === 'remove') {
                    // Find the node that is being removed
                    const nodeToRemove = nodes.find(node => node.id === change.id);
                    // Only allow removal if immutable is false
                    return nodeToRemove && nodeToRemove.data.immutable !== true;
                  }
                  // Allow all other change types
                  return true;
                });
                onNodesChange(filteredChanges);
              }}
              onEdgesChange={onEdgesChange}
              onSelectionChange={onSelectionChange}
              onConnect={onConnect}
              onInit={onInit}
              onDrop={onDrop}
              onDragOver={onDragOver}
              onNodeClick={onNodeClick}
              onNodeDoubleClick={(event, node) => {
                // Double-click is no longer used for renaming
                // It's kept for potential future functionality
              }}
              onNodeDragStart={onNodeDragStart}
              onNodeDragStop={onNodeDragStop}
              fitView
              snapToGrid
              snapGrid={[15, 15]}
              nodesDraggable={true}
              nodesConnectable={true}
              elementsSelectable={true}
            >
              <Background variant={"dots" as BackgroundVariant} gap={12} size={1} />
            </ReactFlow>
            </div>
          </ReactFlowProvider>
        </div>
      </Card>
      
      {/* Label Editing Modal */}
      <Dialog 
        header="Edit Node Label" 
        visible={labelModalVisible} 
        onHide={() => setLabelModalVisible(false)}
        style={{ width: '30rem' }}
        modal
        maximizable
        className="fullscreen-dialog"
        footer={
          <div>
            <Button 
              label="Cancel" 
              icon="pi pi-times" 
              className="p-button-text" 
              onClick={() => setLabelModalVisible(false)} 
            />
            <Button 
              label="Save" 
              icon="pi pi-check" 
              className="p-button-text" 
              onClick={updateNodeLabel}
              disabled={!!labelError} 
            />
          </div>
        }
      >
        <div className="field" style={{ marginTop: '1rem' }}>
          <label htmlFor="node-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold' }}>
            Node Label
          </label>
          <small id="node-label-help" style={{ display: 'block', marginBottom: '0.75rem', color: '#6c757d' }}>
            Labels must contain only alphanumeric characters, underscores, and hyphens (a-zA-Z0-9_-).
          </small>
          <InputText
            id="node-label"
            value={editingLabel}
            onChange={handleLabelChange}
            className={labelError ? 'p-invalid' : ''}
            style={{ width: '100%' }}
            aria-describedby="node-label-help"
          />
          {labelError && (
            <small style={{ display: 'block', marginTop: '0.5rem', color: '#f44336' }}>
              {labelError}
            </small>
          )}
        </div>
      </Dialog>
      
      {/* Agent JSON Export Modal */}
      <Dialog 
        header="Agent JSON Export" 
        visible={jsonDialogVisible} 
        onHide={() => setJsonDialogVisible(false)}
        style={{ width: '50rem' }}
        modal
        maximizable
        footer={
          <div>
            <Button 
              label="Copy to Clipboard" 
              icon="pi pi-copy" 
              className="p-button-text" 
              onClick={() => {
                navigator.clipboard.writeText(agentJson);
                toast.current?.show({
                  severity: 'success',
                  summary: 'Success',
                  detail: 'JSON copied to clipboard!'
                });
              }} 
            />
            <Button 
              label="Close" 
              icon="pi pi-times" 
              className="p-button-text" 
              onClick={() => setJsonDialogVisible(false)} 
            />
          </div>
        }
      >
        <div className="field" style={{ marginTop: '1rem' }}>
          <pre className="agent-json-display">
            {agentJson}
          </pre>
        </div>
      </Dialog>
      
      {/* Validation Errors Dialog */}
      <Dialog 
        header="Agent Validation Errors" 
        visible={validationDialogVisible} 
        onHide={() => setValidationDialogVisible(false)}
        style={{ width: '40rem' }}
        modal
        maximizable
        className="fullscreen-dialog"
        footer={
          <Button 
            label="Close" 
            icon="pi pi-times" 
            className="p-button-text" 
            onClick={() => setValidationDialogVisible(false)} 
          />
        }
      >
        <div className="validation-error-list" style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
          <p style={{ color: 'var(--red-600, #f44336)', marginBottom: '1rem' }}>Please fix the following issues before saving the agent:</p>
          {validationErrors.map((error, index) => (
            <Message 
              key={index}
              severity={error.severity === 'error' ? 'error' : 'warn'} 
              text={error.message}
            />
          ))}
        </div>
      </Dialog>
      
      {/* Agent Configuration Dialog */}
      <Dialog
        header="Configure Agent"
        visible={agentConfigDialogVisible}
        onHide={() => setAgentConfigDialogVisible(false)}
        style={{ width: '35rem' }}
        modal
        maximizable
        className="fullscreen-dialog"
        footer={
          <div>
            <Button
              label="Cancel"
              icon="pi pi-times"
              className="p-button-text"
              onClick={() => setAgentConfigDialogVisible(false)}
            />
            <Button
              label="Save"
              icon="pi pi-check"
              className="p-button-text"
              onClick={() => {
                // Validate agent name
                const error = validateAgentName(agentName);
                if (error) {
                  setAgentNameError(error);
                  return;
                }
                setAgentConfigDialogVisible(false);
              }}
              disabled={!!agentNameError}
            />
          </div>
        }
      >
        <div className="p-fluid">
          <div className="field">
            <label htmlFor="agent-name" className="font-bold">Agent Name</label>
            <small style={{ display: 'block', marginBottom: '0.5rem', color: 'var(--text-color-secondary)' }}>
              Names must contain only alphanumeric characters, underscores, and hyphens (a-zA-Z0-9_-).
            </small>
            <InputText
              id="agent-name"
              value={agentName}
              onChange={(e) => {
                setAgentName(e.target.value);
                setAgentNameError(validateAgentName(e.target.value));
              }}
              className={agentNameError ? 'p-invalid' : ''}
              aria-describedby="agent-name-help"
              autoFocus
            />
            {agentNameError && (
              <small id="agent-name-help" className="p-error" style={{ display: 'block', marginTop: '0.5rem' }}>
                {agentNameError}
              </small>
            )}
          </div>
          
          <div className="field">
            <label htmlFor="agent-description" className="font-bold">Description</label>
            <InputTextarea
              id="agent-description"
              value={agentDescription}
              onChange={(e) => setAgentDescription(e.target.value)}
              rows={3}
              autoResize
              placeholder="Describe the agent's purpose and functionality"
            />
          </div>
          
          <div className="field">
            <label htmlFor="agent-tags" className="font-bold">Tags</label>
            <Chips
              id="agent-tags"
              value={agentTags}
              onChange={(e) => setAgentTags(e.value || [])}
              separator=","
              placeholder="Add tags and press enter"
              tooltip="Add tags to categorize your agent"
              tooltipOptions={{ position: 'top' }}
            />
          </div>
        </div>
      </Dialog>
      
      {/* Import JSON Dialog */}
      <Dialog
        header="Import Agent from JSON"
        visible={importJsonDialogVisible}
        onHide={() => setImportJsonDialogVisible(false)}
        style={{ width: '50rem' }}
        modal
        maximizable
        className="fullscreen-dialog"
        footer={
          <div>
            <Button
              label="Cancel"
              icon="pi pi-times"
              className="p-button-text"
              onClick={() => setImportJsonDialogVisible(false)}
            />
            <Button
              label="Import"
              icon="pi pi-check"
              className="p-button-text"
              onClick={importFromJson}
              disabled={!importJsonText.trim()}
            />
          </div>
        }
      >
        <div className="p-fluid">
          <div className="field">
            <label htmlFor="import-json" className="font-bold">Paste Agent JSON</label>
            <small style={{ display: 'block', marginBottom: '0.5rem', color: 'var(--text-color-secondary)' }}>
              Paste the JSON representation of an agent to import it into the builder.
            </small>
            <InputTextarea
              id="import-json"
              value={importJsonText}
              onChange={(e) => setImportJsonText(e.target.value)}
              rows={15}
              autoResize
              placeholder='{"id":"...","name":"...","description":"...","nodes":[...],"nodeEdges":[...]}'
              className="monospace-font"
              style={{ fontFamily: 'monospace' }}
            />
          </div>
        </div>
      </Dialog>
    </div>
  );
};

export default AgentBuilder;