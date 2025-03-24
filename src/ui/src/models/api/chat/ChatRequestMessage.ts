/**
 * Model for a message in a chat request
 */
export interface ChatRequestMessage {
  /**
   * The role of the message sender: 'user', 'assistant', 'system', etc.
   */
  role: string;
  
  /**
   * The content of the message
   */
  content: string;
  
  /**
   * Optional name of the sender
   */
  name?: string;
}
