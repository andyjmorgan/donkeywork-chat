import { GetConversationsItemModel } from '../../../models/api/Conversation/GetConversationsItemModel';

/**
 * Extended model for conversation item with UI-specific fields
 */
export interface ConversationItem extends GetConversationsItemModel {
  /**
   * The status of the conversation in the UI
   */
  status: 'active' | 'archived' | string;
  
  /**
   * Parsed timestamp (from string to Date object)
   */
  timestamp: Date;
}