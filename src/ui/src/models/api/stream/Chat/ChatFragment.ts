import { BaseStreamItem } from "../BaseStreamItem";

export interface ChatFragment extends BaseStreamItem {
  MessageType: 'ChatFragment';
  Content: string;
}
