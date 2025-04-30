import { BaseStreamItem } from "../BaseStreamItem";

/**
 * Represents a chat start fragment message from the stream
 */
export interface ChatStartFragment extends BaseStreamItem {
  MessageType: 'ChatStartFragment';
  ModelName: string;
  MessageProviderId: string;
  ChatId: string;
}

/**
 * Represents a chat fragment message from the stream
 */
export interface ChatFragment extends BaseStreamItem {
  MessageType: 'ChatFragment';
  Content: string;
}