import { NodeTypeEnum } from './NodeTypeEnum';

export interface BaseNodeData {
  label: string;
  nodeType: NodeTypeEnum;
  immutable?: boolean;
  description?: string;
}