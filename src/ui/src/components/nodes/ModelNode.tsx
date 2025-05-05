import { memo, useState, useCallback, useMemo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { ModelNodeData, NodeTypeEnum } from '../../types/nodes';
import BaseNode from './BaseNode';
import ModelConfigDialog from './ModelConfigDialog';
import { nodeEvents } from '../../utils/nodeEvents';
import { ValidationResult } from '../../utils/nodeValidation';

const ModelNode = ({ id, data }: NodeProps) => {
  const [configDialogVisible, setConfigDialogVisible] = useState(false);
  
  // Use useMemo to create a stable reference to nodeData
  const nodeData: ModelNodeData = useMemo(() => ({
    ...data,
    nodeType: NodeTypeEnum.MODEL,
    immutable: false
  }), [data]);
  
  // Calculate validation result directly instead of in state to avoid render cycles
  const validationResult: ValidationResult = useMemo(() => {
    const errors = [];
    
    if (!nodeData.providerType || !nodeData.modelName) {
      errors.push({
        severity: 'error',
        message: 'Model requires a provider and model selection'
      });
    }
    
    return {
      valid: errors.length === 0,
      errors
    };
  }, [nodeData.providerType, nodeData.modelName]);
  
  // Handle node configuration updates
  const handleNodeUpdate = useCallback((updatedData: ModelNodeData) => {
    // Emit update event to the AgentBuilder component
    nodeEvents.emitUpdate(id, updatedData);
  }, [id]);
  
  // Get provider icon based on providerType - using useMemo to optimize
  const providerIcon = useMemo(() => {
    if (!nodeData.providerType) return null;
    
    switch (nodeData.providerType.toLowerCase()) {
      case 'anthropic':
        return '/images/providers/anthropic.png';
      case 'openai':
        return '/images/providers/openai.png';
      case 'gemini':
        return '/images/providers/gemini.png';
      default:
        return null;
    }
  }, [nodeData.providerType]);
  
  // Open configuration dialog on node click - using useCallback to optimize
  const handleNodeClick = useCallback((e: React.MouseEvent) => {
    // Don't open config if clicking on title (for renaming)
    const isNodeTitle = (e.target as HTMLElement).closest('.node-title');
    if (!isNodeTitle) {
      setConfigDialogVisible(true);
    }
  }, []);
  
  // The has prompts flag
  const hasPrompts = !!(nodeData.systemPromptIds && nodeData.systemPromptIds.length > 0);
  
  // The has tools flag combines either having specific tools or dynamic tools enabled
  const hasTools = !!(
    (nodeData.allowedTools && nodeData.allowedTools.length > 0) || 
    nodeData.dynamicTools
  );
  
  // The count of tools (only used if specific tools are selected)
  const toolCount = nodeData.allowedTools?.length || 0;
  
  // The count of prompts
  const promptCount = nodeData.systemPromptIds?.length || 0;
  
  // Whether tools are in dynamic mode
  const isDynamicTools = !!nodeData.dynamicTools;

  return (
    <>
      <BaseNode 
        id={id}
        type="modelNode"
        data={nodeData} 
        selected={false}
        isConnectable={true}
        dragHandle={undefined}
        zIndex={0}
        xPos={0}
        yPos={0}
        dragging={false}
        className={!validationResult.valid ? 'invalid-node' : ''}
      >
        <Handle type="target" position={Position.Top} id="in" />
        <div className="node-header">
          <span 
            className="node-title"
            title={!data.immutable ? "Click to rename" : undefined}
          >
            {nodeData.label}
          </span>
          <div className="node-controls">
            {!validationResult.valid && (
              <span 
                className="validation-indicator" 
                title={validationResult.errors.map(err => err.message).join('\n')}
              >
                <i className="pi pi-exclamation-triangle" style={{ fontSize: '0.7rem', color: '#f59e0b' }}></i>
              </span>
            )}
            {nodeData.providerType && providerIcon && (
              <img 
                src={providerIcon} 
                alt={nodeData.providerType} 
                className="provider-icon" 
                title={nodeData.providerType}
              />
            )}
          </div>
        </div>
        
        <div className="node-content" onClick={handleNodeClick}>
          {nodeData.providerType && nodeData.modelName ? (
            <div className="model-info">
              <div className="model-name" title={nodeData.modelName}>
                {nodeData.modelName.length > 25 
                  ? nodeData.modelName.substring(0, 22) + '...' 
                  : nodeData.modelName}
              </div>
              
              {/* Add a container for metadata that will be placed at the bottom right */}
              <div className="model-metadata-container">
                <div className="model-metadata">
                  {/* Show prompt icon if system prompts are configured */}
                  {hasPrompts && (
                    <div className="metadata-item" title={`${promptCount} prompts configured`}>
                      <i className="pi pi-envelope" style={{ fontSize: '0.8rem' }}></i>
                    </div>
                  )}
                  
                  {/* Show tools icon if tools are configured */}
                  {hasTools && (
                    <div className="metadata-item" 
                      title={isDynamicTools 
                        ? "Dynamic tools enabled" 
                        : `${toolCount} tools configured`}
                    >
                      {isDynamicTools ? (
                        <i className="pi pi-wrench" style={{ color: 'var(--primary-color)', fontSize: '0.8rem' }}></i>
                      ) : (
                        <i className="pi pi-wrench" style={{ fontSize: '0.8rem' }}></i>
                      )}
                    </div>
                  )}
                  
                  {/* Show streaming icon if streaming is enabled */}
                  {nodeData.streaming && (
                    <div className="metadata-item" title="Streaming enabled">
                      <i className="pi pi-bolt" style={{ fontSize: '0.8rem' }}></i>
                    </div>
                  )}
                </div>
              </div>
            </div>
          ) : (
            <div className="empty-model-config">
              Click to configure model
            </div>
          )}
          
          {!validationResult.valid && (
            <div className="validation-errors">
              <small className="error-hint">{validationResult.errors.length} validation error(s)</small>
            </div>
          )}
        </div>
        
        <Handle type="source" position={Position.Bottom} id="out" />
      </BaseNode>
      
      <ModelConfigDialog
        visible={configDialogVisible}
        onHide={() => setConfigDialogVisible(false)}
        nodeData={nodeData}
        onUpdate={handleNodeUpdate}
      />
    </>
  );
};

export default memo(ModelNode);