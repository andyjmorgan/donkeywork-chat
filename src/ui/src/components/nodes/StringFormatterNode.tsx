import { useState, useCallback, useEffect } from 'react';
import { Handle, Position, NodeProps, useReactFlow } from 'reactflow';
import { StringFormatterNodeData, NodeTypeEnum, ValidationResult } from '../../types/nodes';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { Message } from 'primereact/message';
import TemplateEditor from '../TemplateEditor';
import { nodeEvents } from '../../utils/nodeEvents';
import { validateStringFormatter } from '../../utils/nodeValidation';
import BaseNode from './BaseNode';
import './nodeStyles.css';

const StringFormatterNode = ({ id, data, selected }: NodeProps) => {
  // State for editing modal
  const [modalVisible, setModalVisible] = useState(false);
  const [editingTemplate, setEditingTemplate] = useState('');
  const [validationResult, setValidationResult] = useState<ValidationResult>({ valid: true, errors: [] });
  
  // Get the ReactFlow instance for accessing edges and nodes
  const reactFlowInstance = useReactFlow();
  
  // Get template from data
  const template = data.template || '';
  
  // Load template when modal opens
  useEffect(() => {
    if (modalVisible) {
      setEditingTemplate(template);
    }
  }, [modalVisible, template]);
  
  // Helper to get connected input nodes
  const getConnectedInputNodes = useCallback(() => {
    const { getEdges, getNode } = reactFlowInstance;
    
    // Find all input nodes connected to this formatter node
    return getEdges()
      .filter(edge => edge.target === id)
      .map(edge => getNode(edge.source))
      .filter(Boolean)
      .map(node => node.data.label);
  }, [id, reactFlowInstance]);
  
  // Validate this node
  const validate = useCallback((): ValidationResult => {
    const node = {
      id,
      data: data as StringFormatterNodeData,
      type: 'stringFormatterNode'
    };
    return validateStringFormatter(node as any);
  }, [id, data]);
  
  // Run validation when node data changes
  useEffect(() => {
    const result = validate();
    setValidationResult(result);
  }, [validate, data]);
  
  // Open the editor modal
  const handleOpenEditor = useCallback((event: React.MouseEvent) => {
    event.stopPropagation();
    setModalVisible(true);
  }, []);
  
  // Save changes to the node
  const handleSaveChanges = useCallback(() => {
    // Emit an event to update the node data
    nodeEvents.emitUpdate(id, {
      ...data,
      template: editingTemplate
    });
    
    // Close the modal
    setModalVisible(false);
  }, [id, data, editingTemplate]);
  
  return (
    <>
      <BaseNode 
        id={id}
        type="stringFormatterNode"
        data={{...data, nodeType: NodeTypeEnum.STRING_FORMATTER}} 
        selected={selected || false}
        isConnectable={true}
        dragHandle={undefined}
        zIndex={0}
        xPos={0}
        yPos={0}
        dragging={false}
        className={!validationResult.valid ? 'invalid-node' : ''}>
        <Handle type="target" position={Position.Top} id="in" />
        <div className="node-header">
          <span 
            className="node-title" 
            title={!data.immutable ? "Click to rename" : undefined}
          >{data.label}</span>
          <div className="node-controls">
            {!validationResult.valid && (
              <span 
                className="validation-indicator" 
                title={validationResult.errors.map(err => err.message).join('\n')}
              >
                <i className="pi pi-exclamation-triangle" style={{ fontSize: '0.7rem', color: '#f59e0b' }}></i>
              </span>
            )}
          </div>
        </div>
        <div className="node-content" title="Click to edit template" onClick={handleOpenEditor}>
          <div className="template-preview">
            {template ? 
              (template.length > 20 ? 
                template.substring(0, 20) + '...' : 
                template
              ) : 
              <span className="placeholder-text">Click to add template...</span>
            }
          </div>
          {!validationResult.valid && (
            <div className="validation-errors">
              <small className="error-hint">{validationResult.errors.length} validation error(s)</small>
            </div>
          )}
        </div>
        <Handle type="source" position={Position.Bottom} id="out" />
      </BaseNode>
      
      {/* Template Editor Modal */}
      <Dialog 
        header={`Edit Template for ${data.label}`}
        visible={modalVisible} 
        onHide={() => setModalVisible(false)}
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
              onClick={() => setModalVisible(false)} 
            />
            <Button 
              label="Save" 
              icon="pi pi-check" 
              className="p-button-text" 
              onClick={handleSaveChanges}
            />
          </div>
        }
      >
        <div className="field" style={{ marginTop: '1rem' }}>
          <label htmlFor="template-editor" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold' }}>
            Template Editor
          </label>
          
          {/* Show validation errors if there are any */}
          {!validationResult.valid && (
            <div className="validation-error-list" style={{ marginBottom: '0.75rem', display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
              {validationResult.errors.map((error, index) => (
                <Message 
                  key={index}
                  severity={error.severity === 'error' ? 'error' : 'warn'}
                  text={error.message}
                />
              ))}
            </div>
          )}
          
          <small style={{ display: 'block', marginBottom: '0.75rem', color: 'var(--text-color-secondary)' }}>
            Use Scriban template syntax <code>&#123;&#123; nodeName.property &#125;&#125;</code> to reference other node outputs.
            <br />
            Type <code>space</code> after opening braces to see node suggestions, 
            or <code>.</code> after a node name to see property suggestions.
          </small>
          
          <div className="template-editor-wrapper" style={{ marginBottom: '16px' }}>
            <TemplateEditor 
              value={editingTemplate}
              onChange={setEditingTemplate}
              connectedNodes={getConnectedInputNodes()}
            />
          </div>
          
          <div className="help-section">
            <h4>Available Variables</h4>
            <p>You can use variables from connected <strong>input</strong> nodes like:</p>
            {(() => {
              // Find all input nodes connected to this formatter
              const connectedInputs = getConnectedInputNodes();
                
              if (connectedInputs.length === 0) {
                return (
                  <Message
                    severity="warn"
                    text="No connected input nodes found. Connect nodes to this formatter to use their values."
                    style={{ margin: '0.5rem 0' }}
                  />
                );
              }
              
              return (
                <ul>
                  {connectedInputs.map(nodeName => (
                    <li key={nodeName}>
                      <code>&#123;&#123; {nodeName}.Text &#125;&#125;</code> - Text from {nodeName}
                    </li>
                  ))}
                </ul>
              );
            })()}
            
            <p><strong>Note:</strong> You can only reference nodes that are directly connected as inputs to this string formatter.</p>
            
            {(() => {
              // Find a connected input node to use in the example
              const connectedInput = getConnectedInputNodes()[0]; // Get the first connected input
                
              return connectedInput ? (
                <p>Example: <code>Hello &#123;&#123; {connectedInput}.Text &#125;&#125;!</code></p>
              ) : (
                <p>Example: <code>Hello &#123;&#123; NodeLabel.Text &#125;&#125;!</code> (replace NodeLabel with an actual connected node)</p>
              );
            })()}
          </div>
        </div>
      </Dialog>
    </>
  );
};

export default StringFormatterNode;