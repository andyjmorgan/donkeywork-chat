import { BaseStreamItem } from "../BaseStreamItem";

/**
 * TokenUsage message from the API
 */
export interface TokenUsage extends BaseStreamItem {
  MessageType: 'TokenUsage';
  
  /**
   * The number of input tokens used
   */
  InputTokens: number;
  
  /**
   * The number of output tokens generated
   */
  OutputTokens: number;
  
  /**
   * Additional backward compatibility fields
   */
  PromptTokens?: number;
  CompletionTokens?: number;
  TotalTokens?: number;
}
