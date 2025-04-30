/**
 * Model for prompt item in the prompts list
 */
export interface GetPromptsItemModel {
  /**
   * The prompt ID
   */
  id: string;

  /**
   * The prompt name
   */
  name: string;

  /**
   * The prompt description
   */
  description: string;

  /**
   * The prompt content
   */
  content: string[];

  /**
   * Number of times the prompt has been used
   */
  usageCount: number;

  /**
   * When the prompt was created
   */
  createdAt: string;

  /**
   * When the prompt was last updated
   */
  updatedAt: string;
}