import { ActionItem } from './GetActionsItemModel';

/**
 * Model for the paginated list of actions returned from the API
 */
export interface GetActionsModel {
  /**
   * The total number of actions
   */
  totalCount: number;
  
  /**
   * The list of actions
   */
  actions: ActionItem[];
}