import React from 'react';
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
    if (!toolCalls || toolCalls.length === 0) {
        return <div>No tool calls for this message</div>;
    }

    // Sort tools by start time to show them in execution order
    const sortedTools = sortToolCalls(toolCalls);

    return (
        <div className="tool-calls-container">
            {sortedTools.map((tool, index) => {
                // Create keys for tool parameters and results
                const toolId = tool.id || 'unknown';
                const paramsKey = `${toolId}-params`;
                const resultKey = `${toolId}-result`;
                
                return (
                    <div key={toolId} className="tool-call-item mb-2">
                        <div className="tool-call-header">
                            <span className="tool-call-name">{index + 1}. {tool.name}</span>
                            <span className="tool-call-id text-xs opacity-60"> 
                                (ID: {tool.id || 'unknown'})
                            </span>
                            {tool.duration !== undefined && (
                                <span className="tool-call-duration ml-2">({formatDuration(tool.duration)})</span>
                            )}
                        </div>
                        <div className="tool-call-params">
                            <div 
                                className="collapsible-header" 
                                onClick={() => toggleSection(paramsKey)}
                            >
                                <span className="font-semibold text-sm">Parameters <span className="text-xs opacity-70">(click to toggle)</span>:</span>
                                <i className={`pi ${expandedSections[paramsKey] ? 'pi-chevron-down' : 'pi-chevron-right'}`}></i>
                            </div>
                            {expandedSections[paramsKey] && (
                                <pre className="tool-call-arguments">
                                    {tool.arguments ? formatJsonForDisplay(tool.arguments) : 'No parameters'}
                                </pre>
                            )}
                        </div>
                        
                        {tool.result !== undefined && (
                            <div className="tool-call-result">
                                <div 
                                    className="collapsible-header" 
                                    onClick={() => toggleSection(resultKey)}
                                >
                                    <span className="font-semibold text-sm">Result <span className="text-xs opacity-70">(click to toggle)</span>:</span>
                                    <i className={`pi ${expandedSections[resultKey] ? 'pi-chevron-down' : 'pi-chevron-right'}`}></i>
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