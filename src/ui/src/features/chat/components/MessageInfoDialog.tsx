import React from 'react';
import { Dialog } from 'primereact/dialog';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
// Import directly from model files
import type { MessageMetadata, ChatMessage } from '../../../models/api/chat/ChatMessage';
import { formatDuration } from '../../../utils/formatUtils';
import ToolCallDetails from './ToolCallDetails';
import { MessageInfoDialogProps } from '../../../models/ui/chat/MessageInfoDialogTypes';

/**
 * Dialog to display detailed information about a message and its tool calls
 */
const MessageInfoDialog: React.FC<MessageInfoDialogProps> = ({
    visible,
    onHide,
    selectedMessage,
    messageMetadata,
    expandedSections,
    toggleSection
}) => {
    if (!selectedMessage) return null;

    const metadata = selectedMessage.id ? messageMetadata[selectedMessage.id] : undefined;
    const toolCallDetails = metadata?.toolCallDetails || [];

    return (
        <Dialog
            header="Message Details:"
            visible={visible}
            onHide={onHide}
            style={{ width: '70vw' }}
            breakpoints={{ '960px': '85vw', '641px': '100vw' }}
            maximizable
        >
            <div>
                <div className="mb-3">
                    <div className="font-semibold">Role</div>
                    <div>{selectedMessage.role}</div>
                </div>
                <div className="mb-3">
                    <div className="font-semibold">Content</div>
                    <div className="dialog-markdown-content">
                        <ReactMarkdown 
                            remarkPlugins={[remarkGfm]}
                            components={{
                                a: ({ node, ...props }) => (
                                    <a 
                                        {...props}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                    />
                                ),
                                table: ({ node, ...props }) => (
                                    <div className="markdown-table-container">
                                        <table className="markdown-table" {...props} />
                                    </div>
                                )
                            }}
                        >
                            {selectedMessage.content}
                        </ReactMarkdown>
                    </div>
                </div>
                
                {/* Tool Call Details */}
                {selectedMessage.role === 'assistant' && toolCallDetails.length > 0 && (
                    <div className="mb-3">
                        <div className="font-semibold text-lg" style={{ 
                            borderBottom: '2px solid var(--primary-color)',
                            paddingBottom: '0.5rem', 
                            marginBottom: '1rem' 
                        }}>
                            Tool Calls ({toolCallDetails.length})
                        </div>
                        <ToolCallDetails 
                            toolCalls={toolCallDetails}
                            expandedSections={expandedSections}
                            toggleSection={toggleSection}
                        />
                    </div>
                )}
                
                {/* Token Usage */}
                {selectedMessage.role === 'assistant' && metadata?.tokenCount && (
                    <div className="mb-3">
                        <div className="font-semibold">Token Usage</div>
                        <div className="token-usage-info">
                            <div>Input: {metadata.tokenCount.input}</div>
                            <div>Output: {metadata.tokenCount.output}</div>
                            <div>Total: {metadata.tokenCount.input + metadata.tokenCount.output}</div>
                        </div>
                    </div>
                )}
                
                {/* Message Duration */}
                {selectedMessage.role === 'assistant' && metadata?.duration && (
                    <div className="mb-3">
                        <div className="font-semibold">Total Duration</div>
                        <div>{formatDuration(metadata.duration)}</div>
                    </div>
                )}
                
                {/* Model Information */}
                {selectedMessage.role === 'assistant' && metadata?.modelName && (
                    <div className="mb-3">
                        <div className="font-semibold">Model</div>
                        <div>{metadata.modelName}</div>
                    </div>
                )}
                
                {/* Message Provider IDs */}
                {selectedMessage.role === 'assistant' && metadata?.messageProviderIds && metadata.messageProviderIds.length > 0 && (
                    <div className="mb-3">
                        <div className="font-semibold">Message Provider IDs</div>
                        <div className="dialog-markdown-content" style={{ maxHeight: "150px", overflowY: "auto" }}>
                            <table className="markdown-table">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th>Provider ID</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {metadata.messageProviderIds.map((id, index) => (
                                        <tr key={index}>
                                            <td>{index + 1}</td>
                                            <td style={{ fontFamily: "monospace", fontSize: "0.85rem" }}>{id}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}
                
                <div className="mb-3">
                    <div className="font-semibold">Timestamp</div>
                    <div>{selectedMessage.timestamp?.toLocaleString()}</div>
                </div>

                <div className="mb-3">
                    <div className="font-semibold">
                        {selectedMessage.id.startsWith('temp-') ? 'Message ID (temporary)' : 
                         selectedMessage.role === 'assistant' ? 'Execution ID' : 
                         selectedMessage.id.endsWith('-user') ? 'User Request ID' : 'Message ID'}
                    </div>
                    <div>
                        {/* For user messages with execution IDs, extract and display the main execution ID */}
                        {selectedMessage.id.endsWith('-user') 
                            ? selectedMessage.id.substring(0, selectedMessage.id.length - 5) + ' (user request)'
                            : selectedMessage.id}
                    </div>
                </div>
            </div>
        </Dialog>
    );
};

export default MessageInfoDialog;