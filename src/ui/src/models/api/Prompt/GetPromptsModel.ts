import { GetPromptsItemModel } from './GetPromptsItemModel';

/**
 * Model for getting prompts from the API
 */
export interface GetPromptsModel {
  /**
   * The total count of prompts
   */
  count: number;

  /**
   * The list of prompts
   */
  prompts: GetPromptsItemModel[];
  
  /**
   * The list of prompts (alternative field name for compatibility)
   */
  items?: GetPromptsItemModel[];
}