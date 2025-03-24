export interface ToolCallResult {
    callId: string;
    name: string;
    arguments: Record<string, any>;
    result: any;
    executionId: string;
}