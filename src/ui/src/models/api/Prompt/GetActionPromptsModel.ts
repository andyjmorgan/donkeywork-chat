import { ActionPromptItem } from './GetActionPromptsItemModel';

/**
 * Model for the paginated list of action prompts returned from the API
 */
export interface GetActionPromptsModel {
  /**
   * The total number of action prompts
   */
  total: number;
  
  /**
   * The count of action prompts
   */
  count?: number;
  
  /**
   * The list of action prompts
   */
  items?: ActionPromptItem[];
  
  /**
   * Backward compatibility field
   */
  prompts?: ActionPromptItem[];
}