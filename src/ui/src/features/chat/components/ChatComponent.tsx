import React, { useState, useRef, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from 'primereact/button';
import { ProgressBar } from 'primereact/progressbar';
import { InputTextarea } from 'primereact/inputtextarea';
import { Toast } from 'primereact/toast';
import { chatService } from '../../../services/api/chatService';
// Import directly from the model files instead of through barrel files
import type { ChatMessage } from '../../../models/api/chat/ChatMessage';
import type { MessageMetadata } from '../../../models/api/chat/ChatMessage';
import type { ToolCallInfo } from '../../../models/api/chat/Tool/ToolCallInfo';
import MessageInfoDialog from './MessageInfoDialog';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { sortToolCalls } from '../../../utils/toolCallUtils';
import { formatDuration } from '../../../utils/formatUtils';
import '../../../styles/components/Chat.css';
import { ChatComponentProps } from '../../../models/ui/chat/ChatComponentTypes';

const ChatComponent: React.FC<ChatComponentProps> = ({ 
    provider,
    model,
    systemPrompt = '',
    promptId = undefined,
    initialMessages = []
}) => {
    // Get conversation ID from URL params if available
    const { conversationId: urlConversationId } = useParams<{ conversationId?: string }>();
    const navigate = useNavigate();
    
    // State to track the conversation ID from the server
    // Initialize as undefined to avoid using any cached value
    const [conversationId, setConversationId] = useState<string | undefined>(undefined);
    
    // Using useRef for messageMetadata to avoid state batching issues with React
    const messageMetadataRef = useRef<Record<string, MessageMetadata>>({});
    
    // Chat message state
    const [messages, setMessages] = useState<ChatMessage[]>(initialMessages);
    const [inputMessage, setInputMessage] = useState<string>('');
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [currentAction, setCurrentAction] = useState<string>('thinking');
    const [viewMessageDialog, setViewMessageDialog] = useState<boolean>(false);
    const [selectedMessage, setSelectedMessage] = useState<ChatMessage | null>(null);
    const [tokenUsage, setTokenUsage] = useState<{input: number, output: number} | null>(null);
    const [toolsUsed, setToolsUsed] = useState<string[]>([]);
    const [messageMetadata, setMessageMetadata] = useState<Record<string, MessageMetadata>>({});
    const [expandedSections, setExpandedSections] = useState<Record<string, boolean>>({});
    
    // Toast reference for notifications - create late in the component lifecycle
    const toast = useRef<Toast>(null);
    
    // Make sure toast is always available after component mounts
    useEffect(() => {
        // Force toast to be available after a short delay to ensure DOM is ready
        const timeoutId = setTimeout(() => {
            if (!toast.current) {
                console.warn('Toast reference not initialized. Will attempt to re-render Toast component.');
            }
        }, 500);
        
        return () => clearTimeout(timeoutId);
    }, []);
    
    // Keep messageMetadataRef in sync with the state
    useEffect(() => {
        messageMetadataRef.current = messageMetadata;
    }, [messageMetadata]);
    const messagesEndRef = useRef<null | HTMLDivElement>(null);
    const cancelRef = useRef<{ cancel: () => void } | null>(null);
    const startTimeRef = useRef<number>(0);
    const currentMessageIdRef = useRef<string>('');
    
    useEffect(() => {
        scrollToBottom();
    }, [messages]);
    
    useEffect(() => {
        const updateConversationId = () => {
            const queryId = new URLSearchParams(window.location.search).get('id');
            
            if (urlConversationId) {
                setConversationId(urlConversationId);
            } else if (queryId) {
                setConversationId(queryId);
            } else {
                setConversationId(undefined);
                setMessages([]);
                setTokenUsage(null);
                setToolsUsed([]);
                setMessageMetadata({});
                setExpandedSections({});
            }
        };
        
        updateConversationId();
        
        window.addEventListener('popstate', updateConversationId);
        
        const originalReplaceState = window.history.replaceState;
        window.history.replaceState = function(data: any, unused: string, url?: string | URL | null) {
            const result = originalReplaceState.call(this, data, unused, url);
            setTimeout(updateConversationId, 0);
            return result;
        };
        
        return () => {
            window.removeEventListener('popstate', updateConversationId);
            window.history.replaceState = originalReplaceState;
        };
    }, [urlConversationId]);
    
    const systemPromptRef = useRef<string>(systemPrompt || '');
    
    useEffect(() => {
        systemPromptRef.current = systemPrompt || '';
    }, [systemPrompt]);
    
    useEffect(() => {
        const hasIdParam = new URLSearchParams(window.location.search).has('id');
        
        if (!urlConversationId && !hasIdParam) {
            setMessages([]);
            setConversationId(undefined);
            setTokenUsage(null);
            setToolsUsed([]);
            setMessageMetadata({});
            setExpandedSections({});
            setIsLoading(false);
            setCurrentAction('thinking');
            setInputMessage('');
            return;
        } 
        
        if (initialMessages && initialMessages.length > 0) {
            const checkURL = new URLSearchParams(window.location.search).get('id');
            
            if (checkURL) {
                setMessages(initialMessages);
            } else {
                setMessages([]);
            }
        }
    }, [initialMessages, urlConversationId]);
    
    useEffect(() => {
        if (!urlConversationId && !new URLSearchParams(window.location.search).has('id')) {
            setMessages([]);
            setConversationId(undefined);
            setTokenUsage(null);
            setToolsUsed([]);
            setMessageMetadata({});
            setExpandedSections({});
            setIsLoading(false);
            setCurrentAction('thinking');
            setInputMessage('');
        }
    }, [initialMessages, urlConversationId]);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        setInputMessage(e.target.value);
    };

    const handleSendMessage = () => {
        // Prevent sending if no message, provider, or model
        if (!inputMessage.trim()) return;
        if (!provider || !model) {
            toast.current?.show({
                severity: 'warn',
                summary: 'Missing Selection',
                detail: 'Please select a provider and model first',
                life: 3000
            });
            return;
        }

        // Generate a temporary prefix to correlate user message and assistant response
        // We'll replace this with the real execution ID from the server later
        const requestCorrelationId = 'req-' + crypto.randomUUID();
        
        const userMessage: ChatMessage = {
            id: requestCorrelationId, // Will be updated later with the execution ID
            role: 'user',
            content: inputMessage,
            timestamp: new Date()
        };

        setMessages(prev => [...prev, userMessage]);
        setInputMessage('');
        setIsLoading(true);
        setToolsUsed([]);
        setTokenUsage(null);

        // Prepare the messages for the API
        const apiMessages = [...messages];
        
        // Add the user message
        apiMessages.push(userMessage);
        
        // Create formatted messages array with system prompt first (if available)
        const formattedMessages = [];
        
        // Add system prompt if available and promptId is not provided
        // If promptId is provided, we'll use the prompt from the server side
        if (systemPromptRef.current && !promptId) {
            formattedMessages.push({
                content: systemPromptRef.current,
                role: 'system'
            });
        }
        
        // Add all other messages
        apiMessages.forEach(msg => {
            // Skip any system messages that might be in the message history
            if (msg.role !== 'system') {
                formattedMessages.push({
                    content: msg.content,
                    role: msg.role
                });
            }
        });

        // Current message being streamed
        let currentContent = '';
        let executionId = '';  // Will be set in onStart

        // Start tracking time
        startTimeRef.current = Date.now();
        
        // Create a temporary placeholder for the assistant message
        const tempId = 'temp-' + crypto.randomUUID(); 
        currentMessageIdRef.current = tempId;
        
        // Create an empty assistant message first with temporary ID
        setMessages(prev => [
            ...prev, 
            { 
                id: tempId, 
                role: 'assistant', 
                content: '', 
                timestamp: new Date() 
            }
        ]);

        const urlQueryId = new URLSearchParams(window.location.search).get('id');
        const effectiveConversationId = urlQueryId;
        
        if (!urlQueryId && conversationId) {
            // IMPORTANT: Use URL ID, not state ID to avoid stale references
            setConversationId(undefined);
        }
        
        cancelRef.current = chatService.streamChat(formattedMessages, {
            model: model,
            provider: provider,
            promptId: promptId,
            conversationId: effectiveConversationId || undefined,
            onStart: (receivedExecutionId) => {
                setCurrentAction('thinking');
                
                executionId = receivedExecutionId;
                const oldId = currentMessageIdRef.current;
                currentMessageIdRef.current = executionId;
                
                setMessages(prev => 
                    prev.map(msg => {
                        if (msg.id === oldId) {
                            return { ...msg, id: executionId };
                        }
                        if (msg.role === 'user' && msg.id.startsWith('req-') && prev.findIndex(m => m.id === msg.id) === prev.length - 2) {
                            return { ...msg, id: executionId + '-user' };
                        }
                        return msg;
                    })
                );
                
                setMessageMetadata(prev => {
                    const newMetadata = {...prev};
                    
                    if (prev[oldId]) {
                        newMetadata[executionId] = newMetadata[oldId];
                        delete newMetadata[oldId];
                    }
                    
                    const userPrefix = 'req-';
                    Object.keys(prev).forEach(key => {
                        if (key.startsWith(userPrefix)) {
                            newMetadata[executionId + '-user'] = newMetadata[key];
                            delete newMetadata[key];
                        }
                    });
                    
                    return newMetadata;
                });
            },
            onFragment: (content) => {
                currentContent += content;
                setCurrentAction('generating response');
                
                const normalizedContent = currentContent.replace(/\n{3,}/g, "\n\n");
                
                if (normalizedContent.trim().length > 0) {
                    setMessages(prev => 
                        prev.map(msg => 
                            msg.id === currentMessageIdRef.current 
                                ? { ...msg, content: normalizedContent } 
                                : msg
                        )
                    );
                }
            },
            onToolCall: (toolCall) => {
                setCurrentAction(`using tool: ${toolCall.Name}`);
                
                setToolsUsed(prev => {
                    const toolName = toolCall.Name || toolCall.ToolName;
                    if (toolName && !prev.includes(toolName)) {
                        return [...prev, toolName];
                    }
                    return prev;
                });
                
                const messageId = currentMessageIdRef.current;
                
                setMessageMetadata(prev => {
                    const newState = {...prev};
                    const currentMetadata = newState[messageId] || {};
                    
                    if (!currentMetadata.toolCallDetails) {
                        currentMetadata.toolCallDetails = [];
                    }
                    
                    const toolCallDetails = [...currentMetadata.toolCallDetails];
                    
                    let parsedParams = toolCall.QueryParameters;
                    if (typeof toolCall.QueryParameters === 'string') {
                        try {
                            parsedParams = JSON.parse(toolCall.QueryParameters);
                        } catch (e) {
                            // Keep original if not valid JSON
                        }
                    }
                    
                    const newTool = {
                        id: toolCall.ToolCallId || `generated-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`,
                        name: toolCall.Name,
                        arguments: parsedParams,
                        startTime: Date.now()
                    };
                    
                    toolCallDetails.push(newTool);
                    
                    newState[messageId] = {
                        ...currentMetadata,
                        toolCalls: (currentMetadata.toolCalls || 0) + 1,
                        toolCallDetails: toolCallDetails
                    };
                    
                    return newState;
                });
            },
            onToolResult: (toolResult) => {
                setCurrentAction('processing tool results');
                const messageId = currentMessageIdRef.current;
                
                // Update the corresponding tool call with the result
                setMessageMetadata(prev => {
                    const newState = {...prev};
                    const currentMetadata = newState[messageId] || {};
                    
                    if (!currentMetadata.toolCallDetails) {
                        currentMetadata.toolCallDetails = [];
                    }
                    
                    const toolCallDetails = [...currentMetadata.toolCallDetails];
                    
                    // Parse the result if it's a JSON string
                    let parsedResult = toolResult.Result;
                    if (typeof toolResult.Result === 'string') {
                        try {
                            parsedResult = JSON.parse(toolResult.Result);
                        } catch (e) {
                            // Keep original if not valid JSON
                        }
                    }
                    
                    // Find matching tool calls by ToolCallId
                    const matchingTools = toolCallDetails.filter(tool => tool.id === toolResult.ToolCallId);
                    
                    if (matchingTools.length === 0) {
                        // Create a placeholder if we missed the tool call event
                        const newTool = {
                            id: toolResult.ToolCallId || `generated-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`,
                            name: 'Unknown Tool',
                            arguments: {},
                            startTime: Date.now() - 1000,
                            result: parsedResult,
                            endTime: Date.now(),
                            duration: 1000
                        };
                        
                        toolCallDetails.push(newTool);
                    } else {
                        // Since ToolCallId is unique, there should only be one match
                        // Find the index of the tool in the array
                        const existingToolIndex = toolCallDetails.findIndex(
                            tool => tool.id === toolResult.ToolCallId
                        );
                        
                        // Update the tool call with its result
                        const endTime = Date.now();
                        toolCallDetails[existingToolIndex] = {
                            ...toolCallDetails[existingToolIndex],
                            result: parsedResult,
                            endTime: endTime,
                            duration: endTime - toolCallDetails[existingToolIndex].startTime
                        };
                    }
                    
                    // Update the state with the updated tool calls
                    newState[messageId] = {
                        ...currentMetadata,
                        toolCallDetails: toolCallDetails
                    };
                    
                    return newState;
                });
            },
            onUsage: (usage) => {
                setCurrentAction('finalizing');
                
                const inputTokens = usage.InputTokens;
                const outputTokens = usage.OutputTokens;
                
                // Update token usage by adding new values to existing counts
                setTokenUsage(prev => {
                    const prevInput = prev?.input || 0;
                    const prevOutput = prev?.output || 0;
                    
                    return {
                        input: prevInput + inputTokens,
                        output: prevOutput + outputTokens
                    };
                });
                
                // Store token counts with the message, adding to existing values
                setMessageMetadata(prev => {
                    const currentMetadata = prev[currentMessageIdRef.current] || {};
                    const currentInput = currentMetadata?.tokenCount?.input || 0;
                    const currentOutput = currentMetadata?.tokenCount?.output || 0;
                    
                    return {
                        ...prev,
                        [currentMessageIdRef.current]: {
                            ...currentMetadata,
                            tokenCount: {
                                input: currentInput + inputTokens,
                                output: currentOutput + outputTokens
                            }
                        }
                    };
                });
            },
            onEnd: (responseConversationId) => {
                // Calculate duration
                const duration = Date.now() - startTimeRef.current;
                
                // Store duration with the message
                setMessageMetadata(prev => ({
                    ...prev,
                    [currentMessageIdRef.current]: {
                        ...prev[currentMessageIdRef.current],
                        duration: duration
                    }
                }));
                
                // Store the conversation ID if one is returned
                if (responseConversationId) {
                    // Update state with the new ID
                    setConversationId(responseConversationId);
                    
                    // Update the URL with the new conversation ID
                    const url = `/chat?id=${responseConversationId}`;
                    window.history.replaceState({}, '', url);
                    
                    // Log for debugging
                    console.debug('Conversation ID updated:', responseConversationId);
                }
                
                setIsLoading(false);
                cancelRef.current = null;
            },
            onError: (error) => {
                console.error('Chat error:', error);
                setIsLoading(false);
                cancelRef.current = null;
                
                // Get the error message from the error object
                let errorMessage = 'An error occurred while processing your request. Please try again.';
                
                // Try to extract a more specific error message
                if (error instanceof ErrorEvent && error.message) {
                    errorMessage = error.message;
                } else if (error instanceof Error) {
                    errorMessage = error.message;
                } else if (typeof error === 'string') {
                    errorMessage = error;
                } else if (error && typeof error === 'object' && 'message' in error) {
                    errorMessage = String(error.message);
                }
                
                // Show error message as toast notification instead of adding to chat
                if (toast.current) {
                    toast.current.show({
                        severity: 'error',
                        summary: 'Error',
                        detail: errorMessage,
                        life: 6000,
                        closable: true,
                        sticky: true
                    });
                } else {
                    // Fallback for when toast component isn't available
                    console.error('Toast notification failed: ', errorMessage);
                    alert(`Error: ${errorMessage}`);
                }
            }
        });
    };

    const handleCancelRequest = () => {
        if (cancelRef.current) {
            cancelRef.current.cancel();
            cancelRef.current = null;
            setIsLoading(false);
        }
    };
    
    /**
     * Handles viewing detailed information about a message, including tool calls
     */
    const handleViewMessage = (message: ChatMessage) => {
        if (message.id && messageMetadata[message.id]?.toolCallDetails) {
            const tools = messageMetadata[message.id].toolCallDetails!;
            
            // Sort tool calls by start time for consistent display
            const sortedTools = sortToolCalls(tools);
            
            // Update the metadata with sorted tools
            setMessageMetadata(prev => ({
                ...prev,
                [message.id]: {
                    ...prev[message.id],
                    toolCallDetails: sortedTools
                }
            }));
        }
        
        setSelectedMessage(message);
        setViewMessageDialog(true);
        
        // Initialize expanded sections state for tool call details
        setTimeout(() => {
            if (message.id && messageMetadata[message.id]?.toolCallDetails) {
                const initialState: Record<string, boolean> = {};
                const currentTools = messageMetadata[message.id].toolCallDetails || [];
                
                currentTools.forEach(tool => {
                    // Use ToolCallId as the unique key for each tool
                    const toolId = tool.id || 'unknown';
                    
                    // Set parameters and results expanded by default
                    initialState[`${toolId}-params`] = true;
                    initialState[`${toolId}-result`] = true;
                });
                
                setExpandedSections(initialState);
            }
        }, 10); // Small timeout to ensure state updates are processed
    };

    const handleCopyMessage = (message: ChatMessage) => {
        if (!message.content) return;
        
        navigator.clipboard.writeText(message.content)
            .then(() => {
                // Show toast notification for success
                toast.current?.show({
                    severity: 'success',
                    summary: 'Copied',
                    detail: 'Message copied to clipboard',
                    life: 3000
                });
            })
            .catch(err => {
                // Show toast notification for error
                toast.current?.show({
                    severity: 'error',
                    summary: 'Error',
                    detail: 'Failed to copy message',
                    life: 3000
                });
                console.error('Failed to copy message:', err);
            });
    };
    
    const handleDeleteMessage = (messageId: string) => {
        // Remove the message from the messages array
        setMessages(prevMessages => 
            prevMessages.filter(msg => msg.id !== messageId)
        );
    };
    
    const toggleSection = (sectionId: string) => {
        setExpandedSections(prev => ({
            ...prev,
            [sectionId]: !prev[sectionId]
        }));
    };

    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSendMessage();
        }
    };

    return (
        <div className="chat-container">
            {/* Toast for notifications with higher z-index to ensure visibility */}
            <Toast ref={toast} position="top-right" baseZIndex={1000} />
            
            <div className="chat-messages">
                {messages.map((message) => {
                    // Skip system messages from being displayed
                    if (message.role === 'system') {
                        return null;
                    }
                    
                    // Skip assistant messages with no content
                    if (message.role === 'assistant' && !message.content) {
                        return null;
                    }
                    
                    return (
                        <div key={message.id} className={`message-container ${message.role}`}>
                            <div className="message-content">
                                <div className="markdown-content">
                                    <ReactMarkdown 
                                        remarkPlugins={[remarkGfm]}
                                        components={{
                                            // Make links open in a new tab
                                            a: ({ node, ...props }) => (
                                                <a 
                                                    {...props}
                                                    target="_blank"
                                                    rel="noopener noreferrer"
                                                />
                                            ),
                                            // Add table styling
                                            table: ({ node, ...props }) => (
                                                <div className="markdown-table-container">
                                                    <table className="markdown-table" {...props} />
                                                </div>
                                            ),
                                            // Ensure paragraphs preserve whitespace but prevent double spacing
                                            p: ({ node, ...props }) => (
                                                <p style={{ 
                                                    whiteSpace: 'pre-wrap', 
                                                    marginTop: 0,
                                                    marginBottom: '0.5rem'
                                                }} {...props} />
                                            )
                                        }}
                                    >
                                        {message.content}
                                    </ReactMarkdown>
                                </div>
                                
                                {/* Show timestamp, controls, and metadata at the bottom */}
                                {message.timestamp && (
                                    <div className={`message-metadata-container ${message.role}`}>
                                        {message.role === 'assistant' && messageMetadata[message.id] ? (
                                            <>
                                                {/* Start with time and controls on the left for assistant messages */}
                                                <div className="metadata-left">
                                                    <span className="metadata-time">
                                                        {message.timestamp.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
                                                    </span>
                                                    
                                                    {/* Add controls inline */}
                                                    {message.content && (
                                                        <div className="controls-inline">
                                                            <Button
                                                                icon="pi pi-info-circle"
                                                                className="p-button-rounded p-button-text p-button-sm control-button"
                                                                tooltip="View details"
                                                                tooltipOptions={{ position: 'top' }}
                                                                onClick={() => handleViewMessage(message)}
                                                            />
                                                            <Button
                                                                icon="pi pi-copy"
                                                                className="p-button-rounded p-button-text p-button-sm control-button"
                                                                tooltip="Copy"
                                                                tooltipOptions={{ position: 'top' }}
                                                                onClick={() => handleCopyMessage(message)}
                                                            />
                                                            <Button
                                                                icon="pi pi-trash"
                                                                className="p-button-rounded p-button-text p-button-sm p-button-danger control-button"
                                                                tooltip="Delete"
                                                                tooltipOptions={{ position: 'top' }}
                                                                onClick={() => handleDeleteMessage(message.id)}
                                                            />
                                                        </div>
                                                    )}
                                                </div>
                                                
                                                <div className="metadata-group">
                                                    {/* Duration */}
                                                    {messageMetadata[message.id].duration !== undefined && (
                                                        <span className="metadata-item duration">
                                                            <i className="pi pi-clock"></i>
                                                            {formatDuration(messageMetadata[message.id].duration!)}
                                                        </span>
                                                    )}
                                                    
                                                    {/* Tool calls - show the actual count of unique tools */}
                                                    <span className="metadata-item tool-count">
                                                        <i className="pi pi-wrench"></i>
                                                        {(function() {
                                                          // Use a immediately-invoked function to avoid complex inline logic
                                                          const metadata = messageMetadata[message.id];
                                                          if (!metadata) return '0';
                                                          
                                                          const toolDetails = metadata.toolCallDetails;
                                                          if (!toolDetails || !Array.isArray(toolDetails) || toolDetails.length === 0) return '0';
                                                          
                                                          // Count tool occurrences by name
                                                          const toolCounts: Record<string, number> = {};
                                                          let repeatedTools = false;
                                                          
                                                          toolDetails.forEach(tool => {
                                                            if (tool && tool.name) {
                                                              toolCounts[tool.name] = (toolCounts[tool.name] || 0) + 1;
                                                              if (toolCounts[tool.name] > 1) {
                                                                repeatedTools = true;
                                                              }
                                                            }
                                                          });
                                                          
                                                          // Count unique tools
                                                          const uniqueToolCount = Object.keys(toolCounts).length;
                                                          
                                                          // If any tool was used multiple times, show total calls
                                                          if (repeatedTools) {
                                                            return (
                                                              <span>
                                                                {uniqueToolCount.toString()}
                                                                <span className="tool-repeat-badge">{toolDetails.length}</span>
                                                              </span>
                                                            );
                                                          }
                                                          
                                                          return uniqueToolCount.toString();
                                                        })()}
                                                    </span>
                                                    
                                                    {/* Token counts */}
                                                    {messageMetadata[message.id].tokenCount !== undefined && (
                                                        <>
                                                            <span className="token-item token-in">
                                                                <i className="pi pi-chevron-up"></i>
                                                                {messageMetadata[message.id].tokenCount!.input}
                                                            </span>
                                                            <span className="token-item token-out">
                                                                <i className="pi pi-chevron-down"></i>
                                                                {messageMetadata[message.id].tokenCount!.output}
                                                            </span>
                                                        </>
                                                    )}
                                                </div>
                                            </>
                                        ) : (
                                            // For user messages, show time and controls right-aligned
                                            <div className="metadata-right">
                                                <div className="metadata-group">
                                                    {message.content && (
                                                        <div className="controls-inline">
                                                            <Button
                                                                icon="pi pi-copy"
                                                                className="p-button-rounded p-button-text p-button-sm control-button"
                                                                tooltip="Copy"
                                                                tooltipOptions={{ position: 'top' }}
                                                                onClick={() => handleCopyMessage(message)}
                                                            />
                                                            <Button
                                                                icon="pi pi-trash"
                                                                className="p-button-rounded p-button-text p-button-sm control-button"
                                                                tooltip="Delete"
                                                                tooltipOptions={{ position: 'top' }}
                                                                onClick={() => handleDeleteMessage(message.id)}
                                                            />
                                                        </div>
                                                    )}
                                                    <span className="metadata-time-right">
                                                        {message.timestamp.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
                                                    </span>
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                )}
                            </div>
                        </div>
                    );
                })}
                <div ref={messagesEndRef} style={{ padding: '0.25rem' }} />
                
                {/* Progress indicator for loading state */}
                {isLoading && (
                    <div className="progress-container">
                        <ProgressBar mode="indeterminate" style={{ height: '6px' }} />
                        <div className="progress-text">
                            {currentAction === 'thinking' && 'AI is thinking...'}
                            {currentAction === 'generating response' && 'Generating response...'}
                            {currentAction.startsWith('using tool') && currentAction}
                            {currentAction === 'processing tool results' && 'Processing tool results...'}
                            {currentAction === 'finalizing' && 'Finishing up...'}
                        </div>

                        {/* Display tools used */}
                        {toolsUsed.length > 0 && (
                            <div className="usage-report">
                                <div className="tools-list">
                                    {(() => {
                                        // Count tool occurrences
                                        const toolCounts: Record<string, number> = {};
                                        toolsUsed.forEach(tool => {
                                            toolCounts[tool] = (toolCounts[tool] || 0) + 1;
                                        });
                                        
                                        // Create unique list with counts
                                        return Object.keys(toolCounts).map((tool, index) => (
                                            <span key={index} className="tool-chip">
                                                {tool}
                                                {toolCounts[tool] > 1 && (
                                                    <span className="tool-chip-badge">{toolCounts[tool]}</span>
                                                )}
                                            </span>
                                        ));
                                    })()}
                                </div>
                            </div>
                        )}

                        {/* Display token usage if available */}
                        {tokenUsage && (
                            <div className="usage-report">
                                <div className="usage-report-content">
                                    <div className="usage-report-item">
                                        <span className="usage-report-label">Input</span>
                                        <span className="usage-report-value">{tokenUsage.input}</span>
                                    </div>
                                    <div className="usage-report-item">
                                        <span className="usage-report-label">Output</span>
                                        <span className="usage-report-value">{tokenUsage.output}</span>
                                    </div>
                                    <div className="usage-report-item">
                                        <span className="usage-report-label">Total</span>
                                        <span className="usage-report-value">{tokenUsage.input + tokenUsage.output}</span>
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                )}
            </div>

            {/* New floating chat input */}
            <div className="chat-input-floating">
                <div className="chat-input-container">
                    <InputTextarea 
                        value={inputMessage}
                        onChange={handleInputChange}
                        onKeyDown={handleKeyDown}
                        placeholder="Type your message..."
                        disabled={isLoading}
                        autoResize
                        rows={1}
                        maxLength={10000}
                        className="p-inputtext chat-input-textarea"
                    />
                    {isLoading ? (
                        <Button
                            icon="pi pi-times"
                            className="p-button-danger chat-action-button"
                            onClick={handleCancelRequest}
                            tooltip="Cancel"
                            tooltipOptions={{ position: 'top' }}
                            aria-label="Cancel"
                        />
                    ) : (
                        <Button
                            icon="pi pi-send"
                            className="p-button-primary chat-action-button"
                            onClick={handleSendMessage}
                            disabled={!inputMessage.trim() || !provider || !model}
                            tooltip={!provider || !model ? "Select provider and model first" : "Send message (Enter)"}
                            tooltipOptions={{ position: 'top' }}
                            aria-label="Send"
                        />
                    )}
                </div>
            </div>

            <MessageInfoDialog
                visible={viewMessageDialog}
                onHide={() => setViewMessageDialog(false)}
                selectedMessage={selectedMessage}
                messageMetadata={messageMetadata}
                expandedSections={expandedSections}
                toggleSection={toggleSection}
            />
        </div>
    );
};

export default ChatComponent;