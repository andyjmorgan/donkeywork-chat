import { BaseNodeData } from './BaseNodeData';
import { NodeTypeEnum } from './NodeTypeEnum';

export interface StringFormatterNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.STRING_FORMATTER;
  template: string;
}