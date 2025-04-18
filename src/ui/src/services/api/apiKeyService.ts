import ApiBase from './apiBase';
import axios from 'axios';
import { GetApiKeysModel, ApiKeyModel, UpsertApiKeyModel } from '../../models/api/ApiKey';

class ApiKeyService extends ApiBase {
  /**
   * Get all API keys for the current user
   * @param page Page number (0-based)
   * @param pageSize Number of items per page
   * @returns Promise with API keys
   */
  async getApiKeys(page: number = 0, pageSize: number = 10): Promise<GetApiKeysModel> {
    const response = await axios.get<GetApiKeysModel>(
      `${this.getApiBaseUrl()}/apikey?page=${page}&pageSize=${pageSize}`
    );
    return response.data;
  }

  /**
   * Get a specific API key by ID
   * @param id API key ID
   * @returns Promise with API key details
   */
  async getApiKey(id: string): Promise<ApiKeyModel> {
    const response = await axios.get<ApiKeyModel>(
      `${this.getApiBaseUrl()}/apikey/${id}`
    );
    return response.data;
  }

  /**
   * Create a new API key
   * @param apiKey API key data to create
   * @returns Promise with created API key
   */
  async createApiKey(apiKey: UpsertApiKeyModel): Promise<ApiKeyModel> {
    const response = await axios.post<ApiKeyModel>(
      `${this.getApiBaseUrl()}/apikey`,
      apiKey
    );
    return response.data;
  }

  /**
   * Update an existing API key
   * @param id API key ID
   * @param apiKey API key data to update
   * @returns Promise with success status
   */
  async updateApiKey(id: string, apiKey: UpsertApiKeyModel): Promise<void> {
    await axios.patch<void>(
      `${this.getApiBaseUrl()}/apikey/${id}`,
      apiKey
    );
  }
  
  /**
   * Delete an API key
   * @param id API key ID
   * @returns Promise with success status
   */
  async deleteApiKey(id: string): Promise<void> {
    await axios.delete<void>(
      `${this.getApiBaseUrl()}/apikey/${id}`
    );
  }
}

export const apiKeyService = new ApiKeyService();
