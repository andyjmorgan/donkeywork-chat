/**
 * Base interface for all streaming items
 */
export interface BaseStreamItem {
  /**
   * The type of message
   */
  MessageType: string;
  
  /**
   * The execution ID for the request
   */
  ExecutionId: string;
}
