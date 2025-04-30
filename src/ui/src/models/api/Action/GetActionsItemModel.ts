import { BackendActionModelConfiguration } from './BackendActionModelConfiguration';
import { ToolProviderApplicationType } from '../provider/ToolProviderApplicationType';

/**
 * Model for an action item in the list view
 */
export interface ActionItem {
  /**
   * The unique identifier for the action
   */
  id: string;
  
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
   * The allowed tools for this action
   */
  allowedTools: ToolProviderApplicationType[];
  
  toolProviderApplicationTypes?: ToolProviderApplicationType[];
  
  /**
   * The action model configuration
   */
  actionModelConfiguration: BackendActionModelConfiguration;
  
  /**
   * The timestamp when the action was created
   */
  createdAt: Date | string;
  
  /**
   * The timestamp when the action was last updated
   */
  updatedAt: Date | string;
}