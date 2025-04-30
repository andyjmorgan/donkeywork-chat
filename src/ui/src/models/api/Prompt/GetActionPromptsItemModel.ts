import { PromptMessage } from './Prompt';
import { ActionPromptMessageModel, ActionPromptVariable } from './UpsertActionPromptModel';

/**
 * Model for an action prompt item in the list view
 */
export interface ActionPromptItem {
  /**
   * The unique identifier for the action prompt
   */
  id: string;
  
  /**
   * The name of the action prompt
   */
  name: string;
  
  /**
   * The description of the action prompt
   */
  description: string;
  
  /**
   * The variables in the action prompt
   * Can be array (old format) or record (new format)
   */
  variables: Record<string, ActionPromptVariable> | any[];
  
  /**
   * The messages in the action prompt
   * This can be either PromptMessage[] or ActionPromptMessageModel[] depending on the context
   */
  messages: ActionPromptMessageModel[] | PromptMessage[];
  
  /**
   * The number of times the action prompt has been used
   */
  usageCount: number;
  
  /**
   * The timestamp when the action prompt was created
   */
  timestamp: Date;
  
  /**
   * API fields for backward compatibility
   */
  updatedAt?: string;
  createdAt?: string;
}

export type GetActionPromptsItemModel = ActionPromptItem;