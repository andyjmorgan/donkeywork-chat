import { BaseStreamItem } from "../BaseStreamItem";

/**
 * ToolCall message from the API
 */
export interface ToolCall extends BaseStreamItem {
  MessageType: 'ToolCall';
  
  /**
   * The tool call ID
   */
  ToolCallId: string;
  
  /**
   * The name of the tool
   */
  ToolName: string;
  
  /**
   * The arguments for the tool call
   */
  Arguments: any;
  
  /**
   * The index in the sequence of tool calls
   */
  CallIndex: number;
  
  // Backward compatibility fields
  Id?: string;
  Name?: string;
  QueryParameters?: any;
}
