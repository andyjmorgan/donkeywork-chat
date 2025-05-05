import { NodeTypeEnum } from './NodeTypeEnum';
import { BaseNodeData, createNodeTypeGuard } from './BaseNodeData';

/**
 * Data interface for Model nodes
 */
export interface ModelNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.MODEL;
  immutable: boolean;
}

/**
 * Type guard to check if a node is a ModelNode
 */
export const isModelNode = createNodeTypeGuard<ModelNodeData>(NodeTypeEnum.MODEL);

