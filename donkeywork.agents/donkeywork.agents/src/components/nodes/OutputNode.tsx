import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { OutputNodeData, NodeTypeEnum } from '../../types/nodes';
import BaseNode from './BaseNode';

const OutputNode = ({ data }: NodeProps) => {
  // Ensure data has the correct node type and immutable property
  const nodeData: OutputNodeData = {
    ...data,
    nodeType: NodeTypeEnum.OUTPUT,
    immutable: true
  };

  return (
    <BaseNode data={nodeData}>
      <Handle type="target" position={Position.Top} id="in" />
      <div className="node-header">
        <span className="node-title">{nodeData.label}</span>
      </div>
    </BaseNode>
  );
};

export default memo(OutputNode);