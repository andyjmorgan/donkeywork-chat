import { NodeTypeEnum } from './NodeTypeEnum';
import { BaseNodeData, createNodeTypeGuard } from './BaseNodeData';

/**
 * Condition expression interface for Conditional nodes
 */
export interface ConditionalCondition {
  id: string;
  expression: string;
}

/**
 * Data interface for Conditional nodes
 */
export interface ConditionalNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.CONDITIONAL;
  immutable: false;
  conditions: ConditionalCondition[];
}

/**
 * Type guard to check if a node is a ConditionalNode
 */
export const isConditionalNode = createNodeTypeGuard<ConditionalNodeData>(NodeTypeEnum.CONDITIONAL);

