import { GetActionPromptsModel, UpsertActionPromptModel } from '../../models/api/Prompt';
import ApiBase from './apiBase';

/**
 * Service for action prompt-related API calls
 */
export class ActionPromptService extends ApiBase {
  private static instance: ActionPromptService;
  private baseUrl: string;

  private constructor() {
    super();
    this.baseUrl = `${this.getApiBaseUrl()}/actionprompts`;
  }

  /**
   * Get the singleton instance
   */
  public static getInstance(): ActionPromptService {
    if (!ActionPromptService.instance) {
      ActionPromptService.instance = new ActionPromptService();
    }
    return ActionPromptService.instance;
  }

  /**
   * Get action prompts with optional pagination
   * @param offset Page offset
   * @param limit Page size
   */
  public async getActionPrompts(offset: number = 0, limit: number = 10): Promise<GetActionPromptsModel> {
    const url = `${this.baseUrl}?offset=${offset}&limit=${limit}`;
    
    try {
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
        throw new Error(`Failed to fetch action prompts: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching action prompts:', error);
      throw error;
    }
  }

  /**
   * Create a new action prompt
   * @param prompt The action prompt data to create
   */
  public async createActionPrompt(prompt: UpsertActionPromptModel): Promise<void> {
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
        throw new Error(`Failed to create action prompt: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error creating action prompt:', error);
      throw error;
    }
  }

  /**
   * Update an existing action prompt
   * @param id The ID of the action prompt to update
   * @param prompt The updated action prompt data
   */
  public async updateActionPrompt(id: string, prompt: UpsertActionPromptModel): Promise<void> {
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
          throw new Error('Action prompt not found');
        }
        throw new Error(`Failed to update action prompt: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error updating action prompt:', error);
      throw error;
    }
  }

  /**
   * Delete an action prompt
   * @param id The ID of the action prompt to delete
   */
  public async deleteActionPrompt(id: string): Promise<void> {
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
          throw new Error('Action prompt not found');
        }
        throw new Error(`Failed to delete action prompt: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error deleting action prompt:', error);
      throw error;
    }
  }
}

// Create and export default instance
const actionPromptService = ActionPromptService.getInstance();
export { actionPromptService };
export default actionPromptService;