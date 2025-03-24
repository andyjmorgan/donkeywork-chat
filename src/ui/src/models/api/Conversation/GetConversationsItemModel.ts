/**
 * Model for a conversation item returned from the API
 */
export interface GetConversationsItemModel {
  /**
   * The unique identifier for the conversation
   */
  id: string;
  
  /**
   * The title of the conversation
   */
  title: string;
  
  /**
   * The last message in the conversation
   */
  lastMessage: string;
  
  /**
   * The number of messages in the conversation
   */
  messageCount: number;
  
  /**
   * The timestamp when the conversation was created
   */
  createdAt: string;
  
  /**
   * The timestamp when the conversation was last updated
   */
  updatedAt: string;
}