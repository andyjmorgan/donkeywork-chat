export interface ChatMessage {
    id: string;
    role: string;
    content: string;
    timestamp?: Date;
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
    toolCallDetails?: any[];
    
    /** Name of the model used for generating this message */
    modelName?: string;
    
    /** Provider message IDs from the AI service */
    messageProviderIds?: string[];
}