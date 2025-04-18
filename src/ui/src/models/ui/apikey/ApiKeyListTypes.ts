import { ApiKeySummaryModel } from '../../../models/api/ApiKey';

/**
 * Extended model for API key item with UI-specific fields
 */
export interface ApiKeyItem extends ApiKeySummaryModel {
  /**
   * Parsed timestamp (from string to Date object)
   */
  timestamp: Date;
}