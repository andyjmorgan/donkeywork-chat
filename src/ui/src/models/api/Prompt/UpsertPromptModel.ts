/**
 * Model for creating or updating a prompt
 */
export interface UpsertPromptModel {
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
}