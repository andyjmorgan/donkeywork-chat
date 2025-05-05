import { NodeTypeEnum } from './NodeTypeEnum';
import { BaseNodeData, createNodeTypeGuard } from './BaseNodeData';

/**
 * Data interface for Input nodes
 */
export interface InputNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.INPUT;
  immutable: true;
}

/**
 * Type guard to check if a node is an InputNode
 */
export const isInputNode = createNodeTypeGuard<InputNodeData>(NodeTypeEnum.INPUT);

