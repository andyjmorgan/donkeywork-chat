import { useState, useCallback, useEffect } from 'react';
import { Handle, Position, useReactFlow, useUpdateNodeInternals } from 'reactflow';
import { NodeTypeEnum, ConditionalCondition, ValidationResult } from '../../types/nodes';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { InputText } from 'primereact/inputtext';
import { nodeEvents } from '../../utils/nodeEvents';
import { validateConditional } from '../../utils/nodeValidation';
import BaseNode from './BaseNode';
import './nodeStyles.css';

type ConditionalNodeProps = {
  id: string;
  data: {
    label: string;
    nodeType: NodeTypeEnum;
    conditions: ConditionalCondition[];
    immutable?: boolean;
  };
  selected: boolean;
};

function ConditionalNode({ id, data, selected }: ConditionalNodeProps) {
  // State for editing modal
  const [modalVisible, setModalVisible] = useState(false);
  const [editingConditions, setEditingConditions] = useState<ConditionalCondition[]>([]);
  const [validationResult, setValidationResult] = useState<ValidationResult>({ valid: true, errors: [] });
  
  // Get the ReactFlow instance for accessing edges
  const reactFlowInstance = useReactFlow();
  const updateNodeInternals = useUpdateNodeInternals();
  
  // Get array of conditions, defaulting to an empty array if not defined
  const conditions = data.conditions || [];
  
  // Helper to get connected input nodes
  const getConnectedInputNodes = useCallback(() => {
    const { getEdges, getNode } = reactFlowInstance;
    
    // Find all input nodes connected to this conditional node
    return getEdges()
      .filter(edge => edge.target === id)
      .map(edge => getNode(edge.source))
      .filter(Boolean)
      .map(node => ({
        id: node.id,
        label: node.data.label
      }));
  }, [id, reactFlowInstance]);
  
  // Validate this node
  const validate = useCallback((): ValidationResult => {
    const node = {
      id,
      data: {
        ...data,
        nodeType: NodeTypeEnum.CONDITIONAL
      },
      type: 'conditionalNode'
    };
    return validateConditional(node as any);
  }, [id, data]);
  
  // Load conditions when modal opens
  useEffect(() => {
    if (modalVisible) {
      setEditingConditions([...conditions]);
    }
  }, [modalVisible, conditions]);
  
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
      conditions: editingConditions
    });
    
    // Update node internals to ensure handles are correctly positioned
    updateNodeInternals(id);
    
    // Close the modal
    setModalVisible(false);
  }, [id, data, editingConditions, updateNodeInternals]);
  
  // Run validation when node data changes
  useEffect(() => {
    if (validate) {
      const result = validate();
      setValidationResult(result);
    }
  }, [validate, data]);
  
  return (
    <>
      <BaseNode 
        data={{...data, nodeType: NodeTypeEnum.CONDITIONAL}} 
        className={!validationResult.valid ? 'invalid-node' : ''}
      >
        {/* Input connection point */}
        <Handle type="target" position={Position.Top} />
        
        <div className="node-header">
          <div 
            className="node-title"
            title={!data.immutable ? "Click to rename" : undefined}
          >{data.label}</div>
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
        
        <div className="node-content" onClick={handleOpenEditor}>
          <div className="condition-list">
            {conditions.length > 0 ? (
              conditions.map((cond, index) => (
                <div key={cond.id} className="condition-item">
                  <div className="condition-expression">
                    <span className="condition-index">{index + 1}:</span>
                    <code>{cond.expression || <span className="placeholder-text">Click to add condition...</span>}</code>
                  </div>
                </div>
              ))
            ) : (
              <div className="condition-empty">
                No conditions defined. Click to add.
              </div>
            )}
          </div>
          
          {!validationResult.valid && (
            <div className="validation-errors">
              <small className="error-hint">{validationResult.errors.length} validation error(s)</small>
            </div>
          )}
        </div>
        
        {/* All output handles at the bottom */}
        <div className="output-handles">
          {conditions.map((cond, index) => (
            <div key={cond.id} className="output-handle-container">
              <div className="output-handle-label">{index + 1}</div>
              <Handle
                type="source"
                position={Position.Bottom}
                id={`condition-${cond.id}`}
              />
            </div>
          ))}
          
          {/* Default handle */}
          <div className="output-handle-container">
            <div className="output-handle-label">Default</div>
            <Handle
              type="source"
              position={Position.Bottom}
              id="default"
            />
          </div>
        </div>
      </BaseNode>
      
      {/* Conditional Node Editing Modal */}
      <Dialog 
        header={`Edit Conditions for ${data.label}`}
        visible={modalVisible} 
        onHide={() => setModalVisible(false)}
        style={{ width: '50rem' }}
        modal
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
          {/* Show validation errors if there are any */}
          {!validationResult.valid && (
            <div className="validation-error-list" style={{ marginBottom: '1rem' }}>
              {validationResult.errors.map((error, index) => (
                <div 
                  key={index}
                  style={{ 
                    padding: '0.5rem', 
                    marginBottom: '0.25rem', 
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
          )}
          
          <div className="conditions-section">
            <div className="conditions-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
              <div>
                <h4 style={{ margin: 0 }}>Condition Expressions</h4>
                <small style={{ display: 'block', color: '#6c757d' }}>
                  Define expressions that will be evaluated in order. The first true condition determines which output path is taken.
                </small>
              </div>
              <Button 
                icon="pi pi-plus" 
                label="Add Condition"
                className="p-button-sm" 
                onClick={() => {
                  setEditingConditions([
                    ...editingConditions, 
                    { id: crypto.randomUUID(), expression: '' }
                  ]);
                }} 
              />
            </div>
            
            <div className="conditions-list" style={{ marginBottom: '1.5rem' }}>
              {editingConditions.length > 0 ? (
                editingConditions.map((condition, index) => (
                  <div key={condition.id} className="condition-row" style={{ 
                    display: 'flex', 
                    gap: '0.5rem', 
                    marginBottom: '0.5rem',
                    alignItems: 'center' 
                  }}>
                    <div className="condition-number" style={{ 
                      width: '24px', 
                      height: '24px', 
                      backgroundColor: '#6366F1', 
                      color: 'white',
                      borderRadius: '50%',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontWeight: 'bold'
                    }}>
                      {index + 1}
                    </div>
                    <InputText
                      value={condition.expression}
                      onChange={(e) => {
                        const updatedConditions = [...editingConditions];
                        updatedConditions[index].expression = e.target.value;
                        setEditingConditions(updatedConditions);
                      }}
                      placeholder="Enter expression (e.g. {{ Input.Value > 10 }})"
                      style={{ flex: 1, fontFamily: 'monospace' }}
                    />
                    <Button 
                      icon="pi pi-trash" 
                      className="p-button-text p-button-danger" 
                      onClick={() => {
                        const updatedConditions = [...editingConditions];
                        updatedConditions.splice(index, 1);
                        setEditingConditions(updatedConditions);
                      }} 
                    />
                    <Button 
                      icon="pi pi-arrow-up" 
                      className="p-button-text" 
                      disabled={index === 0}
                      onClick={() => {
                        if (index > 0) {
                          const updatedConditions = [...editingConditions];
                          const temp = updatedConditions[index];
                          updatedConditions[index] = updatedConditions[index - 1];
                          updatedConditions[index - 1] = temp;
                          setEditingConditions(updatedConditions);
                        }
                      }} 
                    />
                    <Button 
                      icon="pi pi-arrow-down" 
                      className="p-button-text" 
                      disabled={index === editingConditions.length - 1}
                      onClick={() => {
                        if (index < editingConditions.length - 1) {
                          const updatedConditions = [...editingConditions];
                          const temp = updatedConditions[index];
                          updatedConditions[index] = updatedConditions[index + 1];
                          updatedConditions[index + 1] = temp;
                          setEditingConditions(updatedConditions);
                        }
                      }} 
                    />
                  </div>
                ))
              ) : (
                <div className="empty-conditions" style={{ 
                  padding: '1rem', 
                  backgroundColor: '#EEF2FF', 
                  borderRadius: '4px',
                  textAlign: 'center',
                  color: '#6B7280',
                  border: '1px dashed #C7D2FE'
                }}>
                  No conditions defined. Add a condition to create a new output path.
                </div>
              )}
            </div>
            
            <div className="default-path-info" style={{ 
              padding: '0.75rem', 
              backgroundColor: '#EEF2FF', 
              borderRadius: '4px',
              border: '1px solid #C7D2FE',
              marginBottom: '1rem'
            }}>
              <h5 style={{ margin: '0 0 0.5rem 0', color: '#4F46E5' }}>Default Path</h5>
              <p style={{ margin: 0, fontSize: '0.9rem' }}>
                If none of the conditions evaluate to true, the flow will continue through the "Default" output path.
              </p>
            </div>
          </div>
          
          <div className="help-section">
            <h4>Available Variables</h4>
            <p>You can use variables from connected <strong>input</strong> nodes like:</p>
            {(() => {
              // Find all input nodes connected to this conditional node
              const connectedInputs = getConnectedInputNodes();
                
              if (connectedInputs.length === 0) {
                return (
                  <div style={{
                    padding: '0.75rem', 
                    margin: '0.5rem 0',
                    backgroundColor: '#fff8e1',
                    borderLeft: '4px solid #ffc107',
                    borderRadius: '4px'
                  }}>
                    <p style={{ margin: 0 }}><i className="pi pi-exclamation-triangle" style={{marginRight: '0.5rem', color: '#ffc107'}}></i>
                    No connected input nodes found. Connect nodes to this conditional block to use their values.</p>
                  </div>
                );
              }
              
              return (
                <ul>
                  {connectedInputs.map(node => (
                    <li key={node.id}>
                      <code>&#123;&#123; {node.label}.Value &#125;&#125;</code> - Value from {node.label}
                    </li>
                  ))}
                </ul>
              );
            })()}
            
            <p><strong>Examples:</strong></p>
            {(() => {
              const connectedInputs = getConnectedInputNodes();
              const firstNode = connectedInputs.length > 0 ? connectedInputs[0].label : "Input";
              const secondNode = connectedInputs.length > 1 ? connectedInputs[1].label : "Model";
              
              return (
                <ul>
                  <li><code>&#123;&#123; {firstNode}.Value &gt; 10 &#125;&#125;</code> - Value greater than 10</li>
                  <li><code>&#123;&#123; {firstNode}.Text | string.contains "error" &#125;&#125;</code> - Text contains "error"</li>
                  {connectedInputs.length > 1 ? (
                    <li><code>&#123;&#123; {secondNode}.Score &lt; 0.5 &amp;&amp; {firstNode}.Priority == "high" &#125;&#125;</code> - Multiple conditions</li>
                  ) : (
                    <li><code>&#123;&#123; Model.Score &lt; 0.5 &amp;&amp; {firstNode}.Priority == "high" &#125;&#125;</code> - Multiple conditions</li>
                  )}
                </ul>
              );
            })()}
          </div>
        </div>
      </Dialog>
    </>
  );
}

export default ConditionalNode;