import { NodeTypeEnum } from './NodeTypeEnum';
import { BaseNodeData, createNodeTypeGuard } from './BaseNodeData';

/**
 * Data interface for Output nodes
 */
export interface OutputNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.OUTPUT;
  immutable: true;
}

/**
 * Type guard to check if a node is an OutputNode
 */
export const isOutputNode = createNodeTypeGuard<OutputNodeData>(NodeTypeEnum.OUTPUT);

