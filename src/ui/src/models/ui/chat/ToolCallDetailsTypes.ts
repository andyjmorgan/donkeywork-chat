import type { ToolCallInfo } from '../../../models/api/chat/Tool/ToolCallInfo';

/**
 * Props for the ToolCallDetails component
 */
export interface ToolCallDetailsProps {
  /**
   * Array of tool calls to display
   */
  toolCalls: ToolCallInfo[];
  
  /**
   * Record tracking which sections are expanded
   */
  expandedSections: Record<string, boolean>;
  
  /**
   * Function to toggle a section's expanded state
   */
  toggleSection: (sectionId: string) => void;
}