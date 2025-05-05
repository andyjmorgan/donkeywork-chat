import { NodeTypeEnum } from './NodeTypeEnum';
import { BaseNodeData, createNodeTypeGuard } from './BaseNodeData';

/**
 * Data interface for String Formatter nodes
 */
export interface StringFormatterNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.STRING_FORMATTER;
  immutable: false;
  template: string;
}

/**
 * Type guard to check if a node is a StringFormatterNode
 */
export const isStringFormatterNode = createNodeTypeGuard<StringFormatterNodeData>(NodeTypeEnum.STRING_FORMATTER);

