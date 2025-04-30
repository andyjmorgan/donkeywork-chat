import { ActionExecutionItem } from './ActionExecutionItem';

export interface GetActionExecutionsResponse {
  totalCount: number;
  actions: ActionExecutionItem[];
}
