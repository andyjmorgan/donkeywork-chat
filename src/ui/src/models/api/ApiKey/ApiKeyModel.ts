/**
 * Full API key model including the actual key value
 */
export interface ApiKeyModel {
  /**
   * Unique identifier
   */
  id: string;

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
   * Full API key value
   */
  apiKey: string;
}