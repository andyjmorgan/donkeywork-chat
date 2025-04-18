/**
 * Model for creating or updating an API key
 */
export interface UpsertApiKeyModel {
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
}