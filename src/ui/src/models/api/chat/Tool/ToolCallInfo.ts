/**
 * Represents a single tool call with its arguments, results, and timing information
 */
export interface ToolCallInfo {
    /** Unique identifier for the tool call (ToolCallId from the API) */
    id: string;
    
    /** Index of the tool call within the sequence of calls for a message */
    callIndex?: number;
    
    /** Name of the tool being called (e.g., 'SearchGoogle', 'GetLocalKnowledgeAsync') */
    name: string;
    
    /** Parameters passed to the tool */
    arguments: any;
    
    /** Tool execution result data */
    result?: any;
    
    /** Timestamp when the tool call was initiated */
    startTime: number;
    
    /** Timestamp when the tool call completed */
    endTime?: number;
    
    /** Duration of the tool call execution in milliseconds */
    duration?: number;
}

/**
 * Metadata associated with each chat message to track performance and tool usage
 */
export interface MessageMetadata {
    /** Total processing time for the message in milliseconds */
    duration?: number;
    
    /** Token usage statistics for the message */
    tokenCount?: {
        /** Number of input tokens used */
        input: number;
        /** Number of output tokens generated */
        output: number;
    };
    
    /** Total number of tool calls made during message processing */
    toolCalls?: number;
    
    /** Detailed information about each tool call */
    toolCallDetails?: ToolCallInfo[];
}