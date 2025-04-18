import { ApiKeySummaryModel } from './ApiKeySummaryModel';

/**
 * Model for retrieving API keys
 */
export interface GetApiKeysModel {
  /**
   * Total count of API keys
   */
  count: number;

  /**
   * Collection of API key summaries
   */
  apiKeys: ApiKeySummaryModel[];
}