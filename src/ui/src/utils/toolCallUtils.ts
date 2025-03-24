import type { ToolCallInfo } from '../models/api/chat/Tool/ToolCallInfo';

/**
 * Sorts tool calls by their start time to ensure consistent display order
 */
export const sortToolCalls = (toolCalls: ToolCallInfo[]): ToolCallInfo[] => {
    if (!toolCalls || toolCalls.length <= 1) return toolCalls;

    // Simply sort the tools by execution order (start time)
    return [...toolCalls].sort((a, b) => a.startTime - b.startTime);
};

/**
 * Formats and sanitizes data for display in the UI
 * Handles both JSON strings and objects, with proper error handling
 */
export const formatJsonForDisplay = (data: any): string => {
    if (!data) return 'No data';
    
    try {
        if (typeof data === 'string') {
            // Try to parse if it's a JSON string
            try {
                const parsed = JSON.parse(data);
                return JSON.stringify(parsed, null, 2);
            } catch {
                // If not valid JSON, just return the string
                return data;
            }
        } else if (typeof data === 'object') {
            // Handle circular references in objects
            const seen = new WeakSet();
            return JSON.stringify(data, (key, value) => {
                if (typeof value === 'object' && value !== null) {
                    if (seen.has(value)) {
                        return '[Circular Reference]';
                    }
                    seen.add(value);
                }
                return value;
            }, 2);
        } else {
            return String(data);
        }
    } catch (e) {
        console.error('Error formatting data for display');
        return 'Error formatting data';
    }
};