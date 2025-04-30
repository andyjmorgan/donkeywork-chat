import { PromptMessageRole, PromptVariable } from './Prompt';

/**
 * Simple action prompt message model
 */
export interface ActionPromptMessageModel {
  /**
   * The role of the message
   * This can be either 'User'|'Assistant' or PromptMessageRole for compatibility
   */
  role: 'User' | 'Assistant' | PromptMessageRole;
  
  /**
   * The message content as a string
   */
  content: string | Record<string, any>;
}

/**
 * Variable definition for action prompts
 */
export interface ActionPromptVariable {
  /**
   * The description of the variable
   */
  description: string;
  
  /**
   * The default value of the variable
   */
  defaultValue?: string;
  
  /**
   * Whether the variable is required
   */
  required: boolean;
}

/**
 * Model for creating or updating an action prompt
 */
export interface UpsertActionPromptModel {
  /**
   * The name of the action prompt
   */
  name: string;
  
  /**
   * The description of the action prompt
   */
  description: string;
  
  /**
   * The variables of the action prompt as a dictionary
   * where the key is the variable name and the value is the variable properties
   */
  variables: Record<string, ActionPromptVariable>;
  
  /**
   * The messages of the action prompt
   */
  messages: ActionPromptMessageModel[];
}