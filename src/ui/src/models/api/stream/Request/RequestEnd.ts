import { BaseStreamItem } from '../BaseStreamItem';

/**
 * Message to indicate the end of a request
 */
export interface RequestEnd extends BaseStreamItem {
  /**
   * The type of message - always 'RequestEnd'
   */
  MessageType: 'RequestEnd';
  
  /**
   * The conversation ID created or used for this chat session
   */
  ConversationId?: string;
}