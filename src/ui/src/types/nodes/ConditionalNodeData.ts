import { BaseNodeData } from './BaseNodeData';
import { NodeTypeEnum } from './NodeTypeEnum';

export interface ConditionalCondition {
  id: string;
  expression: string;
}

export interface ConditionalNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.CONDITIONAL;
  conditions: ConditionalCondition[];
}