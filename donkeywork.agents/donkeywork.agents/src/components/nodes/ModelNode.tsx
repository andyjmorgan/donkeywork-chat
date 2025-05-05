import { memo } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { ModelNodeData, NodeTypeEnum } from '../../types/nodes';
import BaseNode from './BaseNode';

const ModelNode = ({ data }: NodeProps) => {
  const nodeData: ModelNodeData = {
    ...data,
    nodeType: NodeTypeEnum.MODEL,
    immutable: false
  };

  return (
    <BaseNode data={nodeData}>
      <Handle type="target" position={Position.Top} id="in" />
      <div className="node-header">
        <span 
          className="node-title"
          title={!data.immutable ? "Click to rename" : undefined}
        >
          {nodeData.label}
        </span>
      </div>
      <Handle type="source" position={Position.Bottom} id="out" />
    </BaseNode>
  );
};

export default memo(ModelNode);