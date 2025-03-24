/**
 * Model for a prompt item in the list view
 */
export interface PromptItem {
  /**
   * The unique identifier for the prompt
   */
  id: string;
  
  /**
   * The title of the prompt
   */
  title: string;
  
  /**
   * The description of the prompt
   */
  description: string;
  
  /**
   * The content of the prompt
   */
  content: string;
  
  /**
   * The number of times the prompt has been used
   */
  usageCount: number;
  
  /**
   * The timestamp when the prompt was created
   */
  timestamp: Date;
  
  /**
   * API fields for backward compatibility
   */
  updatedAt?: string;
  createdAt?: string;
}

/**
 * Model for the paginated list of prompts returned from the API
 */
export interface GetPromptsModel {
  /**
   * The total number of prompts
   */
  total: number;
  
  /**
   * The list of prompts
   */
  items: PromptItem[];
  
  /**
   * Backward compatibility field
   */
  prompts?: PromptItem[];
}

export type GetPromptsItemModel = PromptItem;

/**
 * Model for creating or updating a prompt
 */
export interface UpsertPromptModel {
  /**
   * The title of the prompt
   */
  title: string;
  
  /**
   * The description of the prompt
   */
  description: string;
  
  /**
   * The content of the prompt
   */
  content: string;
}