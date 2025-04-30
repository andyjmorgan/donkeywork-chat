import { GetActionsModel, UpsertActionModel } from '../../models/api/Action';
import ApiBase from './apiBase';

/**
 * Service for action-related API calls
 */
export class ActionService extends ApiBase {
  private static instance: ActionService;
  private baseUrl: string;

  private constructor() {
    super();
    this.baseUrl = `${this.getApiBaseUrl()}/actions`;
  }

  /**
   * Get the singleton instance
   */
  public static getInstance(): ActionService {
    if (!ActionService.instance) {
      ActionService.instance = new ActionService();
    }
    return ActionService.instance;
  }

  /**
   * Get actions with optional pagination
   * @param offset Page offset
   * @param limit Page size
   */
  public async getActions(offset: number = 0, limit: number = 10): Promise<GetActionsModel> {
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
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        }
        throw new Error(`Failed to fetch actions: ${response.statusText}`);
      }

      const result = await response.json();
      
      if (result.actions) {
        result.actions = result.actions.map((action: any) => {
          if (action.toolProviderApplicationTypes && !action.allowedTools) {
            action.allowedTools = action.toolProviderApplicationTypes;
          }
          return action;
        });
      }
      
      return result;
    } catch (error) {
      console.error('Error fetching actions:', error);
      throw error;
    }
  }

  /**
   * Get an action by ID
   * @param id The ID of the action to retrieve
   */
  public async getActionById(id: string): Promise<any> {
    const url = `${this.baseUrl}/${id}`;
    
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
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Action not found');
        }
        throw new Error(`Failed to fetch action: ${response.statusText}`);
      }

      const result = await response.json();
      
      if (result.toolProviderApplicationTypes && !result.allowedTools) {
        result.allowedTools = result.toolProviderApplicationTypes;
      }
      
      return result;
    } catch (error) {
      console.error('Error fetching action:', error);
      throw error;
    }
  }

  /**
   * Get an action by name
   * @param name The name of the action to retrieve
   */
  public async getActionByName(name: string): Promise<any> {
    const url = `${this.baseUrl}/name/${encodeURIComponent(name)}`;
    
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
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Action not found');
        }
        throw new Error(`Failed to fetch action: ${response.statusText}`);
      }

      const result = await response.json();
      
      if (result.toolProviderApplicationTypes && !result.allowedTools) {
        result.allowedTools = result.toolProviderApplicationTypes;
      }
      
      return result;
    } catch (error) {
      console.error('Error fetching action:', error);
      throw error;
    }
  }

  /**
   * Create a new action
   * @param action The action data to create
   */
  public async createAction(action: UpsertActionModel): Promise<void> {
    try {
      const response = await fetch(this.baseUrl, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(action)
      });

      if (!response.ok) {
        if (response.status === 401) {
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        }
        throw new Error(`Failed to create action: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error creating action:', error);
      throw error;
    }
  }

  /**
   * Update an existing action
   * @param id The ID of the action to update
   * @param action The updated action data
   */
  public async updateAction(id: string, action: UpsertActionModel): Promise<void> {
    const url = `${this.baseUrl}/${id}`;
    
    try {
      const response = await fetch(url, {
        method: 'PATCH',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(action)
      });

      if (!response.ok) {
        if (response.status === 401) {
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Action not found');
        }
        throw new Error(`Failed to update action: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error updating action:', error);
      throw error;
    }
  }

  /**
   * Delete an action
   * @param id The ID of the action to delete
   */
  public async deleteAction(id: string): Promise<void> {
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
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Action not found');
        }
        throw new Error(`Failed to delete action: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error deleting action:', error);
      throw error;
    }
  }
}

// Create and export default instance
const actionService = ActionService.getInstance();
export { actionService };
export default actionService;