/**
 * Model for a prompt item in the list view
 */
export interface PromptItem {
  /**
   * The unique identifier for the prompt
   */
  id: string;
  
  /**
   * The name of the prompt
   */
  name: string;
  
  /**
   * The description of the prompt
   */
  description: string;
  
  /**
   * The content in the prompt
   */
  content: string[];
  
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