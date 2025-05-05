import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { InputNodeData, NodeTypeEnum } from '../../types/nodes';
import BaseNode from './BaseNode';

const InputNode = ({ data }: NodeProps) => {
  // Ensure data has the correct node type and immutable property
  const nodeData: InputNodeData = {
    ...data,
    nodeType: NodeTypeEnum.INPUT,
    immutable: true
  };

  return (
    <BaseNode data={nodeData}>
      <div className="node-header">
        <span className="node-title">{nodeData.label}</span>
      </div>
      <Handle type="source" position={Position.Bottom} id="out" />
    </BaseNode>
  );
};

export default memo(InputNode);