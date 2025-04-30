import { BackendActionModelConfiguration } from './BackendActionModelConfiguration';
import { ToolProviderApplicationType } from '../provider/ToolProviderApplicationType';

/**
 * Model for creating or updating an action
 */
export interface UpsertActionModel {
  /**
   * The name of the action
   */
  name: string;
  
  /**
   * The description of the action
   */
  description: string;
  
  /**
   * The icon for the action
   */
  icon?: string;
  
  /**
   * The system prompt IDs associated with this action
   */
  systemPromptIds: string[];
  
  /**
   * The user prompt IDs associated with this action
   */
  userPromptIds: string[];
  
  /**
   * The tool provider application types that are allowed for this action
   */
  allowedTools: ToolProviderApplicationType[];
  
  /**
   * The action model configuration
   */
  actionModelConfiguration: BackendActionModelConfiguration;
}