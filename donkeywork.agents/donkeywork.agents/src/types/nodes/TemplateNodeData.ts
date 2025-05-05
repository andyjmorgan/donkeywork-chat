import { BaseNodeData, createNodeTypeGuard } from './BaseNodeData';
import { NodeTypeEnum } from './NodeTypeEnum';
import { ValidationResult } from './ValidationTypes';

/**
 * Interface for template node data
 */
export interface TemplateNodeData extends BaseNodeData {
  nodeType: NodeTypeEnum.TEMPLATE;
  template: string;
  connectedNodeIds: string[];
}

/**
 * Type guard to verify a node is a template node
 */
export const isTemplateNode = createNodeTypeGuard<TemplateNodeData>(NodeTypeEnum.TEMPLATE);

/**
 * Default template node data
 */
export const defaultTemplateNodeData: TemplateNodeData = {
  label: 'Template',
  nodeType: NodeTypeEnum.TEMPLATE,
  immutable: false,
  template: '',
  connectedNodeIds: []
};

/**
 * Validates template node data
 */
export function validateTemplateNodeData(data: TemplateNodeData): ValidationResult {
  const errors = [];

  // Template can be empty, but if not empty, perform validation
  if (data.template && typeof data.template !== 'string') {
    errors.push({
      field: 'template',
      message: 'Template must be a string'
    });
  }

  return {
    valid: errors.length === 0,
    errors
  };
}