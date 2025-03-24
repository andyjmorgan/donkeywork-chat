import { GetConversationsModel, GetConversationModel } from '../../models/api/Conversation';
import ApiBase from './apiBase';

/**
 * Service for conversation-related API calls
 */
export class ConversationService extends ApiBase {
  private static instance: ConversationService;
  private baseUrl: string;

  private constructor() {
    super();
    this.baseUrl = `${this.getApiBaseUrl()}/conversations`;
  }

  /**
   * Get the singleton instance
   */
  public static getInstance(): ConversationService {
    if (!ConversationService.instance) {
      ConversationService.instance = new ConversationService();
    }
    return ConversationService.instance;
  }

  /**
   * Get conversations with optional pagination
   * @param offset Page offset
   * @param limit Page size
   */
  public async getConversations(offset: number = 0, limit: number = 1000): Promise<GetConversationsModel> {
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
        throw new Error(`Failed to fetch conversations: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching conversations:', error);
      throw error;
    }
  }

  /**
   * Get a single conversation by ID with all messages
   * @param conversationId The ID of the conversation to retrieve
   */
  public async getConversation(conversationId: string): Promise<GetConversationModel> {
    const url = `${this.baseUrl}/${conversationId}`;
    
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
          // Redirect to login on auth failure
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Conversation not found');
        }
        throw new Error(`Failed to fetch conversation: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching conversation:', error);
      throw error;
    }
  }

  /**
   * Delete a conversation
   * @param conversationId The ID of the conversation to delete
   */
  public async deleteConversation(conversationId: string): Promise<void> {
    const url = `${this.baseUrl}/${conversationId}`;
    
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
          throw new Error('Conversation not found');
        }
        throw new Error(`Failed to delete conversation: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error deleting conversation:', error);
      throw error;
    }
  }

  /**
   * Update a conversation title
   * @param conversationId The ID of the conversation to update
   * @param title The new title
   */
  public async updateConversationTitle(conversationId: string, title: string): Promise<void> {
    const url = `${this.baseUrl}/${conversationId}`;
    
    try {
      const response = await fetch(url, {
        method: 'PATCH',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(title)
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Redirect to login on auth failure like other services
          window.location.href = '/login?session=expired';
          throw new Error('Unauthorized');
        } else if (response.status === 404) {
          throw new Error('Conversation not found');
        }
        throw new Error(`Failed to update conversation title: ${response.statusText}`);
      }
    } catch (error) {
      console.error('Error updating conversation title:', error);
      throw error;
    }
  }
}

// Create and export default instance
const conversationService = ConversationService.getInstance();
export { conversationService };
export default conversationService;