import { NodeTypeEnum } from './NodeTypeEnum';

/**
 * Base interface for all node data types
 */
export interface BaseNodeData {
  label: string;
  nodeType: NodeTypeEnum;
  immutable: boolean;
}

/**
 * Generic type guard factory for node types
 * Creates a type guard function for a specific node type
 */
export function createNodeTypeGuard<T extends BaseNodeData>(nodeType: NodeTypeEnum) {
  return (data: BaseNodeData): data is T => {
    return data.nodeType === nodeType;
  };
}