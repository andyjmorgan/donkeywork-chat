import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { OutputNodeData, NodeTypeEnum } from '../../types/nodes';
import BaseNode from './BaseNode';

const OutputNode = ({ id, data }: NodeProps) => {
  // Ensure data has the correct node type and immutable property
  const nodeData: OutputNodeData = {
    ...data,
    nodeType: NodeTypeEnum.OUTPUT,
    immutable: true
  };

  return (
    <BaseNode 
      id={id}
      type="outputNode"
      data={nodeData}
      selected={false}
      isConnectable={true}
      dragHandle={undefined}
      zIndex={0}
      xPos={0}
      yPos={0}
      dragging={false}
    >
      <Handle type="target" position={Position.Top} id="in" />
      <div className="node-header">
        <span className="node-title">{nodeData.label}</span>
      </div>
    </BaseNode>
  );
};

export default memo(OutputNode);