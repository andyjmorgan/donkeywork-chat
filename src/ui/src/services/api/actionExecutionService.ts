import { GetActionExecutionsResponse } from '../../models/api/ActionExecution';
import ApiBase from './apiBase';

/**
 * Service for action execution-related API calls
 */
export class ActionExecutionService extends ApiBase {
  private static instance: ActionExecutionService;
  private baseUrl: string;

  private constructor() {
    super();
    this.baseUrl = `${this.getApiBaseUrl()}/actionexecution`;
  }

  /**
   * Get the singleton instance
   */
  public static getInstance(): ActionExecutionService {
    if (!ActionExecutionService.instance) {
      ActionExecutionService.instance = new ActionExecutionService();
    }
    return ActionExecutionService.instance;
  }

  /**
   * Get action executions with optional pagination
   * @param offset Page offset
   * @param limit Page size
   */
  public async getActionExecutions(offset: number = 0, limit: number = 10): Promise<GetActionExecutionsResponse> {
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
        throw new Error(`Failed to fetch action executions: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching action executions:', error);
      throw error;
    }
  }
  
  /**
   * Get action executions for a specific action with optional pagination
   * @param actionId The ID of the action to get executions for
   * @param offset Page offset
   * @param limit Page size
   */
  public async getActionExecutionsByActionId(actionId: string, offset: number = 0, limit: number = 10): Promise<GetActionExecutionsResponse> {
    const url = `${this.baseUrl}/action/${actionId}?offset=${offset}&limit=${limit}`;
    
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
        throw new Error(`Failed to fetch action executions: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Error fetching action executions for action ${actionId}:`, error);
      throw error;
    }
  }

  /**
   * Execute an action
   * @param actionId The ID of the action to execute
   * @param actionInput Optional input for the action
   */
  public async executeAction(actionId: string, actionInput: string = ''): Promise<{ executionId: string }> {
    try {
      const response = await fetch(this.baseUrl, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          id: actionId,
          actionInput: actionInput,
          executionId: crypto.randomUUID() // Generate a client-side execution ID
        })
      });

      if (!response.ok) {
        if (response.status === 401) {
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        }
        throw new Error(`Failed to execute action: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error executing action:', error);
      throw error;
    }
  }
}

// Create and export the singleton instance
const actionExecutionService = ActionExecutionService.getInstance();
export { actionExecutionService };
export default actionExecutionService;