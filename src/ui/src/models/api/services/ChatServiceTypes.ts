import type { TokenUsage } from "../../api/stream/Chat/TokenUsage";
import type { ChatStartFragment } from "../../api/stream/Chat/ChatFragment";
import type { ToolCall } from "../../api/stream/Tool/ToolCall";
import type { ToolResult } from "../../api/stream/Tool/ToolResult";

/**
 * Options for streaming chat requests
 */
export interface StreamChatOptions {
  /**
   * Model to use for the chat
   */
  model?: string;
  
  /**
   * Provider to use for the chat
   */
  provider?: string;
  
  /**
   * Optional prompt ID to use instead of including the prompt content
   */
  promptId?: string;
  
  /**
   * Optional conversation ID to link messages in the same conversation
   */
  conversationId?: string;
  
  /**
   * Callback when the request starts
   */
  onStart?: (executionId: string) => void;
  
  /**
   * Callback when the chat start fragment is received
   * Contains model information
   */
  onChatStart?: (startFragment: ChatStartFragment) => void;
  
  /**
   * Callback when a new content fragment is received
   */
  onFragment?: (content: string) => void;
  
  /**
   * Callback when token usage information is received
   */
  onUsage?: (usage: TokenUsage) => void;
  
  /**
   * Callback when a tool call is made
   */
  onToolCall?: (toolCall: ToolCall) => void;
  
  /**
   * Callback when a tool result is received
   */
  onToolResult?: (toolResult: ToolResult) => void;
  
  /**
   * Callback when the request ends
   * @param conversationId Optional conversation ID returned from the server
   */
  onEnd?: (conversationId?: string) => void;
  
  /**
   * Callback when an error occurs
   */
  onError?: (error: Event) => void;
}

/**
 * Result of a streaming chat request
 */
export interface StreamChatResult {
  /**
   * Function to cancel the streaming request
   */
  cancel: () => void;
}