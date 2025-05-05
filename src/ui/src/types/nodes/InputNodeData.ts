import { BaseNodeData } from './BaseNodeData';
import { NodeTypeEnum } from './NodeTypeEnum';

export interface InputNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.INPUT;
}