import { UpsertPromptModel } from '../../api/Prompt/UpsertPromptModel';

/**
 * Props for the PromptDialog component
 */
export interface PromptDialogProps {
  /**
   * Controls dialog visibility
   */
  visible: boolean;
  
  /**
   * Handler for dialog close event
   */
  onHide: () => void;
  
  /**
   * Handler for successful save operation
   */
  onSave: () => void;
  
  /**
   * Prompt data for editing (optional - if not provided, creates a new prompt)
   */
  editPrompt?: UpsertPromptModel & { id?: string };
}