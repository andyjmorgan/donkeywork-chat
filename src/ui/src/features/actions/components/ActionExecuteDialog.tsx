import React, { useState, useRef } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputTextarea } from 'primereact/inputtextarea';
import { Toast } from 'primereact/toast';
import { ProgressSpinner } from 'primereact/progressspinner';
import './ActionExecuteDialog.css'; // Import CSS file
import { actionExecutionService } from '../../../services/api/actionExecutionService';
import { ActionItem } from '../../../models/api/Action';

interface ActionExecuteDialogProps {
  visible: boolean;
  onHide: () => void;
  action: ActionItem | null;
}

/**
 * A dialog component for executing actions
 */
const ActionExecuteDialog: React.FC<ActionExecuteDialogProps> = ({ visible, onHide, action }) => {
  const [actionInput, setActionInput] = useState<string>('');
  const [inputError, setInputError] = useState<string>('');
  const [executing, setExecuting] = useState<boolean>(false);
  const [executionId, setExecutionId] = useState<string | null>(null);
  const toast = useRef<Toast>(null);
  
  // Check if action input is mandatory
  const isInputMandatory = React.useMemo(() => {
    if (!action) return false;
    
    // If there are no user prompts, input is mandatory
    if (!action.userPromptIds || action.userPromptIds.length === 0) {
      return true;
    }
    
    return false;
  }, [action]);

  const handleExecute = async () => {
    if (!action) return;
    
    // Validate input if mandatory
    if (isInputMandatory && !actionInput.trim()) {
      setInputError('Input is required for this action');
      return;
    }
    
    setInputError('');
    setExecuting(true);
    
    try {
      const result = await actionExecutionService.executeAction(action.id, actionInput);
      setExecutionId(result.executionId);
      
      toast.current?.show({
        severity: 'success',
        summary: 'Action Executed',
        detail: `Action "${action.name}" has been executed successfully. Execution ID: ${result.executionId}`,
        life: 5000
      });
      
      // Close the dialog after a short delay to allow the user to see the success message
      setTimeout(() => {
        handleClose();
      }, 2000);
    } catch (error) {
      console.error('Error executing action:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Execution Failed',
        detail: `Failed to execute action "${action.name}". Please try again.`,
        life: 5000
      });
    } finally {
      setExecuting(false);
    }
  };
  
  const handleClose = () => {
    setActionInput('');
    setInputError('');
    setExecutionId(null);
    onHide();
  };

  const renderFooter = () => {
    return (
      <div>
        <Button 
          label="Cancel" 
          icon="pi pi-times" 
          className="p-button-text" 
          onClick={handleClose} 
          disabled={executing}
        />
        <Button 
          label={executing ? 'Executing...' : 'Execute'} 
          icon={executing ? null : 'pi pi-play'} 
          className="p-button-primary" 
          onClick={handleExecute} 
          disabled={executing || (isInputMandatory && !actionInput.trim())}
          loading={executing}
        />
      </div>
    );
  };

  return (
    <>
      <Toast ref={toast} position="top-right" />
      <Dialog
        header={
          <div className="dialog-header">
            <i className="pi pi-play-circle header-icon"></i>
            <span>Execute Action: {action?.name}</span>
          </div>
        }
        visible={visible}
        style={{ width: '50vw' }}
        breakpoints={{ '960px': '75vw', '641px': '100vw' }}
        footer={renderFooter()}
        onHide={handleClose}
        closeOnEscape={!executing}
        closable={!executing}
        dismissableMask={false}
      >
        {executing ? (
          <div className="execution-progress-container">
            <ProgressSpinner style={{ width: '50px', height: '50px' }} strokeWidth="4" animationDuration=".5s" />
            <h3>Executing action...</h3>
            <p>Please wait while the action is being executed.</p>
          </div>
        ) : executionId ? (
          <div className="p-3 text-center">
            <i className="pi pi-check-circle execution-success-icon"></i>
            <h3>Action executed successfully!</h3>
            <p>Execution ID: <code>{executionId}</code></p>
          </div>
        ) : (
          <div className="p-fluid">
            <div className="field">
              <label htmlFor="actionInput" className="font-medium">
                Action Input {isInputMandatory ? <span className="text-red-500">*</span> : '(Optional)'}
              </label>
              <InputTextarea
                id="actionInput"
                value={actionInput}
                onChange={(e) => {
                  setActionInput(e.target.value);
                  if (inputError && e.target.value.trim()) {
                    setInputError('');
                  }
                }}
                rows={5}
                placeholder={isInputMandatory 
                  ? "Enter input data needed for this action (required)" 
                  : "Enter any input data needed for the action (optional)"}
                autoFocus
                className={inputError ? 'p-invalid' : ''}
                required={isInputMandatory}
              />
              {inputError ? (
                <small className="p-error block mt-1">{inputError}</small>
              ) : (
                <small className="text-color-secondary block mt-1">
                  {isInputMandatory 
                    ? "This action requires input to execute because it has no user prompts defined."
                    : "This input will be provided to the action. Leave empty if the action doesn't require input."}
                </small>
              )}
            </div>
            
            <div className="action-details border-1 surface-border">
              <h3>Action Details</h3>
              <p><strong>Name:</strong> {action?.name}</p>
              <p><strong>Description:</strong> {action?.description}</p>
              <p><strong>Provider:</strong> {action?.actionModelConfiguration?.providerType || 'Default'}</p>
            </div>
          </div>
        )}
      </Dialog>
    </>
  );
};

export { ActionExecuteDialog };
export default ActionExecuteDialog;