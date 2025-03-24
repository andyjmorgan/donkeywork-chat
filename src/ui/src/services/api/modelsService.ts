/**
 * Type definition for the default model in the response
 */
export interface DefaultModel {
  key: string;   // Provider name
  value: string; // Model ID
}

/**
 * Type definition for the allowed models response
 */
export interface AllowedModelsResponse {
  allowedModels: Record<string, string[]>;
  defaultModel?: DefaultModel;
}

import ApiBase from './apiBase';

export class ModelsService extends ApiBase {
  private static instance: ModelsService;

  private constructor() {
    super();
  }

  static getInstance(): ModelsService {
    if (!ModelsService.instance) {
      ModelsService.instance = new ModelsService();
    }
    return ModelsService.instance;
  }

  /**
   * Fetches the available models from the API
   * @returns Dictionary of allowed models where key is provider and value is array of model IDs
   */
  async getModels(): Promise<AllowedModelsResponse | null> {
    try {
      const response = await fetch(`${this.getApiBaseUrl()}/models`, {
        credentials: 'include',
        headers: {
          'Cache-Control': 'no-cache'
        }
      });

      if (!response.ok) {
        throw new Error(`Failed to get models: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Failed to get models:', error);
      return null;
    }
  }
}

// Export a singleton instance as the default export
const modelsService = ModelsService.getInstance();
export { modelsService };
export default modelsService;