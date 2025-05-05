import { memo } from 'react';
import { NodeProps } from 'reactflow';
import { BaseNodeData } from '../../types/nodes';

// This is the base component that all node types will extend
export const BaseNode = memo(({ 
  id, 
  type,
  data, 
  children, 
  className = '',
  selected = false,
  dragHandle,
  zIndex,
  isConnectable = true,
  xPos = 0,
  yPos = 0,
  ...rest
}: NodeProps & { 
  children: React.ReactNode;
  className?: string;
}) => {
  const nodeData = data as BaseNodeData;
  
  return (
    <div className={`custom-node ${nodeData.nodeType}-node ${className}`}>
      {children}
    </div>
  );
});

export default BaseNode;