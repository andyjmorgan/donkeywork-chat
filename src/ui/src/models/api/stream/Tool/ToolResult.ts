import { BaseStreamItem } from "../BaseStreamItem";

/**
 * ToolResult message from the API
 */
export interface ToolResult extends BaseStreamItem {
  MessageType: 'ToolResult';
  
  /**
   * The tool call ID
   */
  ToolCallId: string;
  
  /**
   * The result of the tool call
   */
  Result: any;
  
  // Backward compatibility fields
  CallId?: string;
}
