import { ChatRequestMessage } from './ChatRequestMessage';

/**
 * Model for a chat request sent to the API
 */
export interface ChatRequest {
  /**
   * The messages to send to the AI
   */
  messages: ChatRequestMessage[];
  
  /**
   * Optional model to use
   */
  model?: string;
  
  /**
   * Optional provider to use
   */
  provider?: string;

  /**
   * Optional prompt ID to use (instead of including the prompt content)
   */
  promptId?: string;
  
  /**
   * Optional conversation ID to link messages in the same conversation
   */
  conversationId?: string;
}