import React, { useRef } from 'react';
import { Button } from 'primereact/button';
import { Toast } from 'primereact/toast';
// Import directly from the model file
import type { ToolCallInfo } from '../../../models/api/chat/Tool/ToolCallInfo';
import { formatJsonForDisplay, sortToolCalls } from '../../../utils/toolCallUtils';
import { formatDuration } from '../../../utils/formatUtils';
import { ToolCallDetailsProps } from '../../../models/ui/chat/ToolCallDetailsTypes';

/**
 * Component to display detailed information about tool calls
 */
const ToolCallDetails: React.FC<ToolCallDetailsProps> = ({
    toolCalls,
    expandedSections,
    toggleSection
}) => {
    const toast = useRef<Toast>(null);
    
    if (!toolCalls || toolCalls.length === 0) {
        return <div>No tool calls for this message</div>;
    }

    // Sort tools by start time to show them in execution order
    const sortedTools = sortToolCalls(toolCalls);
    
    // Generic copy function for any JSON or string data
    const copyToClipboard = (data: any, contentType: string) => {
        if (!data) {
            toast.current?.show({
                severity: 'warn',
                summary: 'Nothing to Copy',
                detail: `No ${contentType} data to copy`,
                life: 3000
            });
            return;
        }
        
        // Format the data for copying
        let textToCopy = '';
        try {
            if (typeof data === 'string') {
                textToCopy = data;
            } else {
                textToCopy = JSON.stringify(data, null, 2);
            }
            
            navigator.clipboard.writeText(textToCopy)
                .then(() => {
                    toast.current?.show({
                        severity: 'success',
                        summary: 'Copied',
                        detail: `${contentType} copied to clipboard`,
                        life: 3000
                    });
                })
                .catch(err => {
                    console.error(`Failed to copy ${contentType}:`, err);
                    toast.current?.show({
                        severity: 'error',
                        summary: 'Error',
                        detail: 'Failed to copy to clipboard',
                        life: 3000
                    });
                });
        } catch (error) {
            console.error(`Error formatting ${contentType} for copying:`, error);
            toast.current?.show({
                severity: 'error',
                summary: 'Error',
                detail: `Failed to format ${contentType} for copying`,
                life: 3000
            });
        }
    };
    
    // Handle copying tool result to clipboard
    const handleCopyToolResult = (tool: ToolCallInfo) => {
        copyToClipboard(tool.result, 'result');
    };
    
    // Handle copying tool parameters to clipboard
    const handleCopyToolParams = (tool: ToolCallInfo) => {
        copyToClipboard(tool.arguments, 'parameters');
    };

    return (
        <div className="tool-calls-container">
            <Toast ref={toast} position="top-right" />
            
            {sortedTools.map((tool, index) => {
                // Create keys for tool parameters and results
                const toolId = tool.id || 'unknown';
                const paramsKey = `${toolId}-params`;
                const resultKey = `${toolId}-result`;
                
                return (
                    <div key={toolId} className="tool-call-item mb-2">
                        <div className="tool-call-header">
                            <span className="tool-call-name">
                                {index + 1}. {tool.name}
                                {(() => {
                                    // Count number of occurrences of this tool name
                                    const count = toolCalls.filter(t => t.name === tool.name).length;
                                    if (count > 1) {
                                        return <span className="tool-call-repeat-badge">{count}×</span>;
                                    }
                                    return null;
                                })()}
                            </span>
                            <span className="tool-call-id text-xs opacity-60"> 
                                (ID: {tool.id || 'unknown'})
                            </span>
                            {tool.duration !== undefined && (
                                <span className="tool-call-duration ml-2">({formatDuration(tool.duration)})</span>
                            )}
                        </div>
                        <div className="tool-call-params">
                            <div className="tool-result-header">
                                <div 
                                    className="collapsible-header flex-grow-1" 
                                    onClick={() => toggleSection(paramsKey)}
                                >
                                    <span className="font-semibold text-sm">Parameters <span className="text-xs opacity-70">(click to toggle)</span>:</span>
                                    <i className={`pi ${expandedSections[paramsKey] ? 'pi-chevron-down' : 'pi-chevron-right'}`}></i>
                                </div>
                                {tool.arguments && (
                                    <Button
                                        icon="pi pi-copy"
                                        className="p-button-text p-button-rounded p-button-sm result-copy-button"
                                        onClick={() => handleCopyToolParams(tool)}
                                        tooltip="Copy parameters"
                                        tooltipOptions={{ position: 'left' }}
                                    />
                                )}
                            </div>
                            {expandedSections[paramsKey] && (
                                <pre className="tool-call-arguments">
                                    {tool.arguments ? formatJsonForDisplay(tool.arguments) : 'No parameters'}
                                </pre>
                            )}
                        </div>
                        
                        {tool.result !== undefined && (
                            <div className="tool-call-result">
                                <div className="tool-result-header">
                                    <div 
                                        className="collapsible-header flex-grow-1" 
                                        onClick={() => toggleSection(resultKey)}
                                    >
                                        <span className="font-semibold text-sm">Result <span className="text-xs opacity-70">(click to toggle)</span>:</span>
                                        <i className={`pi ${expandedSections[resultKey] ? 'pi-chevron-down' : 'pi-chevron-right'}`}></i>
                                    </div>
                                    <Button
                                        icon="pi pi-copy"
                                        className="p-button-text p-button-rounded p-button-sm result-copy-button"
                                        onClick={() => handleCopyToolResult(tool)}
                                        tooltip="Copy result"
                                        tooltipOptions={{ position: 'left' }}
                                    />
                                </div>
                                {expandedSections[resultKey] && (
                                    <pre className="tool-call-result-data">
                                        {tool.result ? formatJsonForDisplay(tool.result) : 'No result'}
                                    </pre>
                                )}
                            </div>
                        )}
                    </div>
                );
            })}
        </div>
    );
};

export default ToolCallDetails;