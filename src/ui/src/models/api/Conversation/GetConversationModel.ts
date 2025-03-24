/**
 * Model for message ownership
 */
export enum MessageOwner {
  USER = 'User',
  ASSISTANT = 'Assistant'
}

/**
 * Model for a single conversation message
 */
export interface GetConversationMessageModel {
  /**
   * The unique identifier for the message
   */
  id: string;
  
  /**
   * The message pair ID (groups related user/assistant messages)
   */
  messagePairId: string;
  
  /**
   * Who owns this message (User or Assistant)
   */
  owner: MessageOwner;
  
  /**
   * The content of the message
   */
  content: string;
  
  /**
   * The timestamp when the message was created
   */
  timestamp: string;
}

/**
 * Model for a detailed conversation
 */
export interface GetConversationModel {
  /**
   * The unique identifier for the conversation
   */
  id: string;
  
  /**
   * The title of the conversation
   */
  title: string;
  
  /**
   * The messages in the conversation
   */
  messages: GetConversationMessageModel[];
}