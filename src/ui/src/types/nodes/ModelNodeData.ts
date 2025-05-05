import { BaseNodeData } from './BaseNodeData';
import { NodeTypeEnum } from './NodeTypeEnum';

export interface ModelNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.MODEL;
  allowedTools: string[];
  providerType: string;
  modelName: string;
  streaming: boolean;
  systemPromptIds: string[];
  modelParameters: Record<string, string>;
  dynamicTools?: boolean;
}