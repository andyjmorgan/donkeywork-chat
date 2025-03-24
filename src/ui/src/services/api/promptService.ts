import { GetPromptsModel, UpsertPromptModel } from '../../models/api/Prompt';
import ApiBase from './apiBase';

/**
 * Service for prompt-related API calls
 */
export class PromptService extends ApiBase {
  private static instance: PromptService;
  private baseUrl: string;

  private constructor() {
    super();
    this.baseUrl = `${this.getApiBaseUrl()}/prompts`;
  }

  /**
   * Get the singleton instance
   */
  public static getInstance(): PromptService {
    if (!PromptService.instance) {
      PromptService.instance = new PromptService();
    }
    return PromptService.instance;
  }

  /**
   * Get prompts with optional pagination
   * @param offset Page offset
   * @param limit Page size
   */
  public async getPrompts(offset: number = 0, limit: number = 10): Promise<GetPromptsModel> {
    const url = `${this.baseUrl}?offset=${offset}&limit=${limit}`;
    
    try {
      // No artificial delay
      
      const response = await fetch(url, {
        method: 'GET',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Redirect to login on auth failure like other services
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        }
        throw new Error(`Failed to fetch prompts: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching prompts:', error);
      throw error;
    }
  }

  /**
   * Create a new prompt
   * @param prompt The prompt data to create
   */
  public async createPrompt(prompt: UpsertPromptModel): Promise<void> {
    try {
      const response = await fetch(this.baseUrl, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(prompt)
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Redirect to login on auth failure like other services
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        }
        throw new Error(`Failed to create prompt: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error creating prompt:', error);
      throw error;
    }
  }

  /**
   * Update an existing prompt
   * @param id The ID of the prompt to update
   * @param prompt The updated prompt data
   */
  public async updatePrompt(id: string, prompt: UpsertPromptModel): Promise<void> {
    const url = `${this.baseUrl}/${id}`;
    
    try {
      const response = await fetch(url, {
        method: 'PATCH',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(prompt)
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Redirect to login on auth failure like other services
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Prompt not found');
        }
        throw new Error(`Failed to update prompt: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error updating prompt:', error);
      throw error;
    }
  }

  /**
   * Delete a prompt
   * @param id The ID of the prompt to delete
   */
  public async deletePrompt(id: string): Promise<void> {
    const url = `${this.baseUrl}/${id}`;
    
    try {
      const response = await fetch(url, {
        method: 'DELETE',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Redirect to login on auth failure like other services
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Prompt not found');
        }
        throw new Error(`Failed to delete prompt: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error deleting prompt:', error);
      throw error;
    }
  }
}

// Create and export default instance
const promptService = PromptService.getInstance();
export { promptService };
export default promptService;