// Use direct imports instead of barrel imports
import type { ChatRequestMessage } from "../../models/api/chat/ChatRequestMessage";
import type { ChatRequest } from "../../models/api/chat/ChatRequest";
import type { TokenUsage } from "../../models/api/stream/Chat/TokenUsage";
import type { ChatFragment } from "../../models/api/stream/Chat/ChatFragment";
import type { ToolCall } from "../../models/api/stream/Tool/ToolCall";
import type { ToolResult } from "../../models/api/stream/Tool/ToolResult";
import type { BaseStreamItem } from "../../models/api/stream/BaseStreamItem";
import type { RequestEnd } from "../../models/api/stream/Request/RequestEnd";
import type { StreamChatOptions, StreamChatResult } from "../../models/api/services/ChatServiceTypes";
import ApiBase from "./apiBase";

/**
 * Service for chat-related API calls
 */
export class ChatService extends ApiBase {
    private static instance: ChatService;
    private baseUrl: string;
    private activeRequests: Map<string, { controller: AbortController, reader: ReadableStreamDefaultReader }> = new Map();

    private constructor() {
        super();
        this.baseUrl = `${this.getApiBaseUrl()}/chat`;
    }

    /**
     * Get the singleton instance
     */
    public static getInstance(): ChatService {
        if (!ChatService.instance) {
            ChatService.instance = new ChatService();
        }
        return ChatService.instance;
    }

    /**
     * Stream a chat conversation with the AI
     * @param messages Array of chat messages to send
     * @param options Configuration options and callbacks
     * @returns Object with cancel method to abort the request
     */
    streamChat(messages: ChatRequestMessage[], options: StreamChatOptions): StreamChatResult {
        const requestId = crypto.randomUUID();
        const controller = new AbortController();

        // First create the chat request
        const chatRequest: ChatRequest = {
            messages: messages.map(msg => ({
                content: msg.content,
                role: msg.role,
            })),
            model: options.model,
            provider: options.provider,
            promptId: options.promptId,
            conversationId: options.conversationId,
        };

        fetch(this.baseUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(chatRequest),
            signal: controller.signal,
            credentials: 'include'
        }).then(async response => {
            // Check for error responses
            if (!response.ok) {
                // Handle different error types
                if (response.status === 401) {
                    console.error('Authentication error: Session expired');
                    
                    // Redirect to login screen with session expired message
                    window.location.href = '/login?session=expired';
                    throw new Error('Session expired');
                } else {
                    // For other errors, try to parse the response body
                    const errorData = await response.json();
                    console.error('API error response:', errorData);
                    const errorMessage = errorData.title || errorData.detail || errorData.message || 'Error communicating with API';
                    throw new Error(errorMessage);
                }
            }
            
            const reader = response.body?.getReader();
            if (!reader) throw new Error('No reader available');

            this.activeRequests.set(requestId, { controller, reader });

            const textDecoder = new TextDecoder();
            let buffer = '';

            // Define handlers object to use in processStream
            const handlers = {
                onStart: options.onStart,
                onFragment: options.onFragment,
                onUsage: options.onUsage,
                onToolCall: options.onToolCall,
                onToolResult: options.onToolResult,
                onEnd: options.onEnd,
                onError: options.onError
            };
            
            const processStream = async () => {
                try {
                    while (true) {
                        const { done, value } = await reader.read();
                        if (done) break;

                        buffer += textDecoder.decode(value, { stream: true });
                        const lines = buffer.split('\n');
                        buffer = lines.pop() || '';

                        for (const line of lines) {
                            if (line.startsWith('data: ')) {
                                const data = JSON.parse(line.slice(6))
                                const baseMessage = data as BaseStreamItem;
                                switch (baseMessage.MessageType) {
                                    case 'RequestStart':
                                        handlers.onStart?.(baseMessage.ExecutionId);
                                        break;
                                    case 'ChatStartFragment':
                                        break;
                                    case 'ChatFragment':
                                        const chatMsg = data as ChatFragment;
                                        // Trim any extra whitespace from fragments
                                        handlers.onFragment?.(chatMsg.Content);
                                        break;
                                    case 'ToolCall':
                                        const toolMsg = data as ToolCall;
                                        handlers.onToolCall?.(toolMsg);
                                        break;
                                    case 'ToolResult':
                                        const toolResultMsg = data as ToolResult;
                                        handlers.onToolResult?.(toolResultMsg);
                                        break;
                                    case 'TokenUsage':
                                        const usageMsg = data as TokenUsage;
                                        handlers.onUsage?.(usageMsg);
                                        break;
                                    case 'ChatEndFragment':
                                        break;
                                    case 'RequestEnd':
                                        const requestEndMsg = data as RequestEnd;
                                        // If there's a conversation ID in the response, pass it to the onEnd handler
                                        handlers.onEnd?.(requestEndMsg.ConversationId);
                                        this.activeRequests.delete(requestId);
                                        break;
                                }
                            }
                        }
                    }
                } catch (error) {
                    console.error('Stream processing error:', error);
                    handlers.onError?.(error as Event);
                    this.activeRequests.delete(requestId);
                }
            };

            processStream();
        }).catch(error => {
            if (error.name === 'AbortError') {
                // Request was canceled
            } else if (error.message === 'Session expired') {
                // Already handled by redirecting to login
            } else {
                console.error('Fetch error:', error);
                
                // Pass the original error to preserve all details
                options.onError?.(error);
            }
            this.activeRequests.delete(requestId);
        });

        return {
            cancel: () => {
                const request = this.activeRequests.get(requestId);
                if (request) {
                    request.controller.abort();
                    request.reader.cancel();
                    this.activeRequests.delete(requestId);
                }
            }
        };
    }

    /**
     * Cancel all active streaming requests
     */
    cancelAll() {
        for (const [_, request] of this.activeRequests) {
            request.controller.abort();
            request.reader.cancel();
        }
        this.activeRequests.clear();
    }
}

// Create and export default instance
const chatService = ChatService.getInstance();
export { chatService };
export default chatService;