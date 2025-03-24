import type { ChatMessage, MessageMetadata } from '../../../models/api/chat/ChatMessage';

/**
 * Props for the MessageInfoDialog component
 */
export interface MessageInfoDialogProps {
  /**
   * Controls dialog visibility
   */
  visible: boolean;
  
  /**
   * Handler for dialog close event
   */
  onHide: () => void;
  
  /**
   * The selected message to display details for
   */
  selectedMessage: ChatMessage | null;
  
  /**
   * Metadata for all messages in the conversation
   */
  messageMetadata: Record<string, MessageMetadata>;
  
  /**
   * Tracks which sections are expanded in the UI
   */
  expandedSections: Record<string, boolean>;
  
  /**
   * Function to toggle a section's expanded state
   */
  toggleSection: (sectionId: string) => void;
}