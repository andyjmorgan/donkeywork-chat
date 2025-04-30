import { ActionExecutionStatus } from './ActionExecutionStatus';

export interface ActionExecutionItem {
  id: string;
  executionId: string;
  actionId: string;
  actionName: string;
  executionStatus: ActionExecutionStatus;
  createdAt: string | Date;
  endTime: string | Date;
}
