/**
 * Summary model for API key
 */
export interface ApiKeySummaryModel {
  /**
   * Unique identifier
   */
  id: string;

  /**
   * When the API key was created
   */
  createdAt: string;

  /**
   * Name of the API key
   */
  name: string;

  /**
   * Optional description
   */
  description?: string;

  /**
   * Whether the key is enabled
   */
  isEnabled: boolean;

  /**
   * Masked API key value (for display only)
   */
  apiKey: string;
}