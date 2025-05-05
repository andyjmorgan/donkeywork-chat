import { BaseNodeData } from './BaseNodeData';
import { NodeTypeEnum } from './NodeTypeEnum';

export interface OutputNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.OUTPUT;
}