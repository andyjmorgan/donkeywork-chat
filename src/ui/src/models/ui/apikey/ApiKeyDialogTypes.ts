import { ApiKeyModel } from '../../api/ApiKey';

/**
 * Props for the API Key Dialog component
 */
export interface ApiKeyDialogProps {
  /**
   * Whether the dialog is visible
   */
  visible: boolean;
  
  /**
   * Function to call when dialog is closed
   */
  onHide: () => void;
  
  /**
   * API key to edit, or undefined if creating a new key
   */
  apiKey?: ApiKeyModel;
  
  /**
   * Function to call when API key is saved
   */
  onSave: () => void;
  
  /**
   * Whether to open in edit mode (vs. view mode)
   */
  editMode?: boolean;
}