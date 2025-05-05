import { useState, useCallback, useRef, useEffect } from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { InputText } from 'primereact/inputtext';
import { Message } from 'primereact/message';
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
import { nodeTypes as importedNodeTypes } from '../nodes';
import { nodeEvents } from '../../utils/nodeEvents';
import { validateAgent, ValidationError } from '../../utils/nodeValidation';
import { NodeTypeEnum } from '../../types/nodes';
import 'reactflow/dist/style.css';
import '../nodes/nodeStyles.css';
import './AgentBuilder.css';

// Define nodeTypes outside of the component to avoid React Flow warning
const nodeTypes = importedNodeTypes;

interface AgentBuilderProps {
  initialNodes?: Node[];
  initialEdges?: Edge[];
  onSave?: (agentJson: string) => void;
}

interface AgentData {
  nodes: any[];
  edges: any[];
}

export const AgentBuilder = ({ 
  initialNodes, 
  initialEdges, 
  onSave 
}: AgentBuilderProps) => {
  // Default initial nodes if none provided
  const defaultInitialNodes: Node[] = [
    {
      id: '1',
      type: 'inputNode',
      data: { 
        label: 'Input',
        nodeType: NodeTypeEnum.INPUT,
        immutable: true
      },
      position: { x: 250, y: 100 },
    },
    {
      id: '2',
      type: 'outputNode',
      data: { 
        label: 'Output',
        nodeType: NodeTypeEnum.OUTPUT,
        immutable: true 
      },
      position: { x: 250, y: 200 },
    },
  ];

  const defaultInitialEdges = [
    { id: 'e1-2', source: '1', target: '2' }
  ];

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes || defaultInitialNodes);
  const [edges, setEdges, onEdgesChangeOriginal] = useEdgesState(initialEdges || defaultInitialEdges);
  const [nodeId, setNodeId] = useState(initialNodes?.length ? initialNodes.length + 1 : 3); // Start from after last node
  const [reactFlowInstance, setReactFlowInstance] = useState<any>(null);
  const reactFlowWrapper = useRef<HTMLDivElement>(null);
  const [selectedElements, setSelectedElements] = useState<{nodes: Node[], edges: Edge[]}>({nodes: [], edges: []});
  
  // State for node label editing modal
  const [labelModalVisible, setLabelModalVisible] = useState(false);
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [editingLabel, setEditingLabel] = useState('');
  const [labelError, setLabelError] = useState<string | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  
  // State for JSON dialog
  const [jsonDialogVisible, setJsonDialogVisible] = useState(false);
  const [agentJson, setAgentJson] = useState('');
  
  // State for validation dialog
  const [validationDialogVisible, setValidationDialogVisible] = useState(false);
  const [validationErrors, setValidationErrors] = useState<ValidationError[]>([]);
  
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
      
      setEdges((eds) => addEdge(params, eds));
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
  
  // Open the label editing modal
  const openLabelEditor = useCallback((nodeId: string) => {
    const node = nodes.find(n => n.id === nodeId);
    console.log('openLabelEditor called for node:', nodeId, 'node found:', !!node);
    if (node && !node.data.immutable) {
      // Delay longer to ensure all other events have completed
      console.log('Setting timeout to open label editor for node:', nodeId);
      setTimeout(() => {
        console.log('Timeout fired, opening label editor for node:', nodeId);
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
      console.log('Node title clicked, opening label editor for node:', node.id);
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
        id: `${nodeId}`,
        type,
        position,
        data: nodeData,
      };
      
      setNodes((nds) => nds.concat(newNode));
      setNodeId((id) => id + 1);
    },
    [nodeId, reactFlowInstance, setNodes, nodes, getUniqueLabel]
  );
  
  const saveAgent = useCallback(() => {
    // Use our validation utility to validate the agent
    const validationResult = validateAgent(nodes, edges);
    
    if (validationResult.valid) {
      // Create a clean version of the agent data with only essential information
      const cleanedNodes = nodes.map(node => {
        const baseNode = {
          id: node.id,
          type: node.type,
          label: node.data.label,
          nodeType: node.data.nodeType,
          immutable: node.data.immutable,
          position: node.position
        };
        
        // Add template data for string formatter nodes
        if (node.type === 'stringFormatterNode') {
          return {
            ...baseNode,
            template: node.data.template || ''
          };
        }
        
        // Add conditions data for conditional nodes
        if (node.type === 'conditionalNode') {
          return {
            ...baseNode,
            conditions: node.data.conditions || []
          };
        }
        
        return baseNode;
      });
      
      const cleanedEdges = edges.map(edge => ({
        id: edge.id,
        source: edge.source,
        target: edge.target,
        sourceHandle: edge.sourceHandle,
        targetHandle: edge.targetHandle
      }));
      
      const agentData: AgentData = { 
        nodes: cleanedNodes, 
        edges: cleanedEdges 
      };
      
      // Format the JSON with indentation
      const formattedJson = JSON.stringify(agentData, null, 2);
      
      // If onSave callback is provided, call it with the agent JSON
      if (onSave) {
        onSave(formattedJson);
      } else {
        // Otherwise, set the JSON data and show the dialog
        setAgentJson(formattedJson);
        setJsonDialogVisible(true);
      }
    } else {
      // Show validation errors in a dialog
      setValidationErrors(validationResult.errors);
      setValidationDialogVisible(true);
    }
  }, [nodes, edges, onSave]);
  
  // Create a custom header component with title, palette and save button
  const renderCardHeader = () => {
    return (
      <div className="agent-card-header">
        <div className="agent-title">Agent Builder</div>
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
            label="Save Agent" 
            icon="pi pi-save" 
            className="p-button-sm p-button-success" 
            onClick={saveAgent} 
          />
        </div>
      </div>
    );
  };

  return (
    <div className="agent-builder-container">
      <Card header={renderCardHeader()} className="agent-card">
        <div className="flow-container" ref={reactFlowWrapper}>
          <ReactFlowProvider>
            <ReactFlow
              nodes={nodes}
              edges={edges}
              nodeTypes={nodeTypes}
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
      
      {/* Agent JSON Modal */}
      <Dialog 
        header="Agent Configuration" 
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
                alert('JSON copied to clipboard!');
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
          <pre 
            style={{ 
              backgroundColor: '#f8f9fa', 
              padding: '1rem', 
              borderRadius: '4px',
              maxHeight: '400px',
              overflow: 'auto',
              fontSize: '14px',
              fontFamily: 'monospace'
            }}
          >
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
        footer={
          <Button 
            label="Close" 
            icon="pi pi-times" 
            className="p-button-text" 
            onClick={() => setValidationDialogVisible(false)} 
          />
        }
      >
        <div className="validation-error-list">
          <p style={{ color: '#f44336', marginBottom: '1rem' }}>Please fix the following issues before saving the agent:</p>
          {validationErrors.map((error, index) => (
            <div 
              key={index} 
              style={{ 
                padding: '0.75rem', 
                marginBottom: '0.5rem', 
                backgroundColor: error.severity === 'error' ? '#ffebee' : '#fff8e1',
                borderLeft: `4px solid ${error.severity === 'error' ? '#f44336' : '#ffc107'}`,
                borderRadius: '4px'
              }}
            >
              <i className={`pi pi-${error.severity === 'error' ? 'times-circle' : 'exclamation-triangle'}`} style={{ marginRight: '0.5rem', color: error.severity === 'error' ? '#f44336' : '#ffc107' }}></i>
              {error.message}
            </div>
          ))}
        </div>
      </Dialog>
    </div>
  );
};

export default AgentBuilder;