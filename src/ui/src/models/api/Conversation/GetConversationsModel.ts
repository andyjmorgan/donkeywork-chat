import { GetConversationsItemModel } from './GetConversationsItemModel';

/**
 * Model for the paginated list of conversations returned from the API
 */
export interface GetConversationsModel {
  /**
   * The total number of conversations
   */
  total: number;
  
  /**
   * The list of conversations
   */
  items: GetConversationsItemModel[];
  
  /**
   * Backward compatibility field
   */
  conversations?: GetConversationsItemModel[];
}