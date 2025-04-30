import React, { useState, useEffect, useRef } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { Toast } from 'primereact/toast';
import { Dropdown } from 'primereact/dropdown';
import { UpsertActionPromptModel, ActionPromptMessageModel, ActionPromptVariable } from '../../../models/api/Prompt/UpsertActionPromptModel';
import { actionPromptService } from '../../../services/api/actionPromptService';
import './PromptDialog.css';

// Safely highlight variables in content
const HighlightVariables: React.FC<{ 
  content: string; 
  variables: { name: string }[]; 
}> = ({ content, variables }) => {
  try {
    // Safety checks
    if (!content) return null;
    if (!variables || !Array.isArray(variables) || variables.length === 0) {
      return <span>{content}</span>;
    }

    // Get all valid variable names
    const variableNames = variables
      .map(v => v.name?.trim())
      .filter(Boolean);

    if (variableNames.length === 0) {
      return <span>{content}</span>;
    }

    // Create components to render
    const result: React.ReactNode[] = [];
    let lastIndex = 0;
    let key = 0;

    // Find and process all variable matches
    variableNames.forEach(varName => {
      // Look for {{ varName }} pattern
      const pattern = `{{\\s*${varName}\\s*}}`;
      const regex = new RegExp(pattern, 'g');
      let match;

      while ((match = regex.exec(content)) !== null) {
        // Add text before the match
        if (match.index > lastIndex) {
          result.push(
            <span key={key++}>
              {content.substring(lastIndex, match.index)}
            </span>
          );
        }

        // Add the highlighted variable
        result.push(
          <span key={key++} className="variable-highlight px-1 rounded-sm">
            {match[0]}
          </span>
        );

        lastIndex = match.index + match[0].length;
      }
    });

    // Add any remaining text after the last match
    if (lastIndex < content.length) {
      result.push(
        <span key={key++}>
          {content.substring(lastIndex)}
        </span>
      );
    }

    return result.length > 0 ? <>{result}</> : <span>{content}</span>;
  } catch (error) {
    // If anything goes wrong, just return the plain content
    console.error('Error in variable highlighting:', error);
    return <span>{content}</span>;
  }
};

interface ActionPromptDialogProps {
  visible: boolean;
  onHide: () => void;
  onSave: () => void;
  editPrompt?: UpsertActionPromptModel & { id?: string };
}

/**
 * A reusable component for creating or editing action prompts
 */
const ActionPromptDialog: React.FC<ActionPromptDialogProps> = ({ visible, onHide, onSave, editPrompt }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [variables, setVariables] = useState<{ name: string; description: string; defaultValue: string; required: boolean }[]>([]);
  const [messages, setMessages] = useState<ActionPromptMessageModel[]>([]);
  const [saving, setSaving] = useState(false);
  const [nameError, setNameError] = useState('');
  const [descriptionError, setDescriptionError] = useState('');
  const [messagesError, setMessagesError] = useState('');
  const [isFormValid, setIsFormValid] = useState(false);
  // Track which sections are expanded
  const [activeVariableIndexes, setActiveVariableIndexes] = useState<number[]>([]);
  const [activeMessageIndexes, setActiveMessageIndexes] = useState<number[]>([]);
  const toast = useRef<Toast>(null);

  /**
   * Parse content from various formats to string
   */
  const parseMessageContent = (content: any): string => {
    if (!content) return '';
    
    // Direct string content
    if (typeof content === 'string') return content;
    
    // Object with text property
    if (typeof content === 'object' && 'text' in content) {
      return content.text || '';
    }
    
    // Array of content items (MessageContent[] from API)
    if (Array.isArray(content) && content.length > 0) {
      // Find the first text content
      const textContent = content.find(item => 
        typeof item === 'object' && 
        item.type === 'text' && 
        'text' in item
      );
      
      return textContent && 'text' in textContent ? textContent.text || '' : '';
    }
    
    // Unknown format
    return '';
  };

  /**
   * Convert variables from object format to array format
   */
  const convertVariablesToArray = (variables: any): { name: string; description: string; defaultValue: string; required: boolean }[] => {
    if (!variables) return [];
    
    // Already in array format
    if (Array.isArray(variables)) return variables;
    
    // Object format - convert to array
    if (typeof variables === 'object') {
      try {
        return Object.entries(variables).map(([name, details]: [string, any]) => ({
          name,
          description: details.description || '',
          defaultValue: details.defaultValue || '',
          required: details.required !== undefined ? Boolean(details.required) : Boolean(details.isRequired)
        }));
      } catch (error) {
        console.error('Error converting variables format:', error);
      }
    }
    
    return [];
  };

  // Reset form when the dialog is opened or editPrompt changes
  useEffect(() => {
    if (visible) {
      if (editPrompt) {
        // Set basic fields
        setName(editPrompt.name || '');
        setDescription(editPrompt.description || '');
        
        // Convert variables to array format
        setVariables(convertVariablesToArray(editPrompt.variables));
        
        // Process messages
        const processedMessages = (editPrompt.messages || [])
          .map(msg => ({
            role: msg.role,
            content: parseMessageContent(msg.content)
          }));
        
        // Set messages (or default if empty)
        setMessages(processedMessages.length > 0 
          ? processedMessages 
          : [{ role: 'User', content: '' }]
        );
      } else {
        // Clear form for new prompt
        setName('');
        setDescription('');
        setVariables([]);
        setMessages([{ role: 'User', content: '' }]);
      }
      
      // Clear all validation errors
      setNameError('');
      setDescriptionError('');
      setMessagesError('');
      
      // Set initial active indexes to expand all sections
      const variableIndexes = editPrompt?.variables
        ? Array.isArray(editPrompt.variables)
          ? editPrompt.variables.map((_, i) => i)
          : Object.keys(editPrompt.variables).map((_, i) => i)
        : [];
      
      const messageIndexes = editPrompt?.messages
        ? editPrompt.messages.map((_, i) => i)
        : [0];
      
      setActiveVariableIndexes(variableIndexes);
      setActiveMessageIndexes(messageIndexes);
      
      // Update form validity after a delay
      setTimeout(() => {
        setIsFormValid(checkFormValidity());
      }, 300);
    }
  }, [visible, editPrompt]);

  // Add a new variable
  const addVariable = () => {
    const newVariable = {
      name: '',
      description: '',
      defaultValue: '',
      required: true
    };
    const newVariables = [...variables, newVariable];
    setVariables(newVariables);
    // Ensure the new item is expanded
    setActiveVariableIndexes([...activeVariableIndexes, newVariables.length - 1]);
  };

  // Remove a variable
  const removeVariable = (index: number) => {
    const newVariables = [...variables];
    newVariables.splice(index, 1);
    setVariables(newVariables);
    
    // Update active indexes
    const newActiveIndexes = activeVariableIndexes
      .filter(i => i !== index)
      .map(i => i > index ? i - 1 : i);
    
    setActiveVariableIndexes(newActiveIndexes);
  };

  /**
   * Format a variable name to be valid (alphanumeric only)
   */
  const formatVariableName = (name: string): string => {
    return name.replace(/[^a-zA-Z0-9]/g, '');
  };

  /**
   * Update a variable field and format if needed
   */
  const updateVariable = (index: number, field: string, value: string | boolean) => {
    const newVariables = [...variables];
    
    // Format the name field if needed
    if (field === 'name' && typeof value === 'string') {
      // Only allow alphanumeric characters
      value = formatVariableName(value);
    }
    
    // Update the variable
    newVariables[index] = {
      ...newVariables[index],
      [field]: value
    };
    
    setVariables(newVariables);
    
    // Run debounced validation
    debouncedValidation();
  };

  // Add a new message (only User or Assistant roles)
  const addMessage = () => {
    // Determine which role to use for the new message (alternate between User and Assistant)
    const lastMessageRole = messages.length > 0 ? messages[messages.length - 1].role : null;
    const newRole = lastMessageRole === 'User' ? 'Assistant' : 'User';
    
    const newMessage: ActionPromptMessageModel = {
      role: newRole,
      content: ''
    };
    const newMessages = [...messages, newMessage];
    setMessages(newMessages);
    // Ensure the new item is expanded
    setActiveMessageIndexes([...activeMessageIndexes, newMessages.length - 1]);
    
    // Clear message error when adding a message
    setMessagesError('');
  };

  // Remove a message
  const removeMessage = (index: number) => {
    if (messages.length <= 1) {
      // Don't remove the last message
      return;
    }
    
    const newMessages = [...messages];
    newMessages.splice(index, 1);
    setMessages(newMessages);
    
    // Update active indexes
    const newActiveIndexes = activeMessageIndexes
      .filter(i => i !== index)
      .map(i => i > index ? i - 1 : i);
    
    setActiveMessageIndexes(newActiveIndexes);
    
    // Check if we should show message error after removing
    if (newMessages.length === 0 || !newMessages.some(m => m.content && m.content.trim())) {
      setMessagesError('At least one message with content is required');
    } else {
      setMessagesError('');
    }
  };

  // Update a message
  const updateMessage = (index: number, field: keyof ActionPromptMessageModel, value: any) => {
    const newMessages = [...messages];
    newMessages[index] = {
      ...newMessages[index],
      [field]: value
    };
    setMessages(newMessages);
    
    // Clear messages error if we have content
    if (newMessages.some(m => m.content && m.content.trim())) {
      setMessagesError('');
    }
  };

  /**
   * Debouncer for form validation
   */
  const formValidationTimeout = useRef<NodeJS.Timeout | null>(null);
  
  /**
   * Run validation with debouncing
   */
  const debouncedValidation = () => {
    // Clear any pending validation
    if (formValidationTimeout.current) {
      clearTimeout(formValidationTimeout.current);
    }
    
    // Schedule a new validation
    formValidationTimeout.current = setTimeout(() => {
      setIsFormValid(checkFormValidity());
      formValidationTimeout.current = null;
    }, 100);
  };

  /**
   * Update message content with validation
   */
  const updateMessageContent = (messageIndex: number, content: string) => {
    // Update the message
    const newMessages = [...messages];
    newMessages[messageIndex] = {
      ...newMessages[messageIndex],
      content: content
    };
    
    setMessages(newMessages);
    
    // Clear message error if we have valid content
    if (newMessages.some(m => m.content && m.content.trim())) {
      setMessagesError('');
    }
    
    // Run debounced validation
    debouncedValidation();
  };

  // Check form validity without setting error states
  const checkFormValidity = (): boolean => {
    // Basic field validation
    const hasName = name.trim().length > 0;
    const hasValidNameLength = name.trim().length <= 64;
    const hasDescription = description.trim().length > 0;
    
    // Message validation
    const hasMessageWithContent = messages.some(message => 
      message.content && typeof message.content === 'string' && message.content.trim().length > 0
    );
    
    // Variable name validation (if any variables exist)
    let variablesValid = true;
    if (variables.length > 0) {
      const variableNames = variables
        .map(v => v.name?.trim())
        .filter(Boolean);
      
      // Check for duplicate names
      const uniqueNames = new Set(variableNames);
      const hasDuplicateNames = variableNames.length !== uniqueNames.size;
      
      // Check for invalid variable names (only alphanumeric allowed)
      const hasInvalidNames = variables.some(v => 
        v.name && v.name.trim() !== '' && !/^[a-zA-Z0-9]+$/.test(v.name.trim())
      );
      
      variablesValid = !hasDuplicateNames && !hasInvalidNames;
    }
    
    return hasName && hasValidNameLength && hasDescription && hasMessageWithContent && variablesValid;
  };
  
  // Validate form and set error messages for display
  const validateForm = (): boolean => {
    // Name validation - required and max 64 characters
    const trimmedName = name.trim();
    if (!trimmedName) {
      setNameError('Name is required');
    } else if (trimmedName.length > 64) {
      setNameError(`Name must be 64 characters or less (currently ${trimmedName.length})`);
    } else {
      setNameError('');
    }
    
    // Description validation - required
    if (!description.trim()) {
      setDescriptionError('Description is required');
    } else {
      setDescriptionError('');
    }
    
    // Messages validation - at least one message with non-empty content
    if (messages.length === 0) {
      setMessagesError('At least one message is required');
    } else {
      // Check if at least one message has content
      const hasValidMessage = messages.some(message => 
        message.content && typeof message.content === 'string' && message.content.trim().length > 0
      );
      
      if (!hasValidMessage) {
        setMessagesError('At least one message must have non-empty content');
      } else {
        setMessagesError('');
      }
    }
    
    // Check overall validity
    const isValid = checkFormValidity();
    setIsFormValid(isValid);
    
    return isValid;
  };

  /**
   * Prepare the data for submission to the API
   */
  const preparePromptData = (): UpsertActionPromptModel | null => {
    // Validate the form first
    if (!validateForm()) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Validation Error', 
        detail: 'Please fix all validation errors before saving' 
      });
      return null;
    }
    
    // Double-check variables for duplicate or invalid names
    const variableNames = variables
      .map(v => v.name?.trim())
      .filter(Boolean);
    
    const hasDuplicateNames = variableNames.length !== new Set(variableNames).size;
    const hasInvalidNames = variables.some(v => 
      v.name && v.name.trim() !== '' && !/^[a-zA-Z0-9]+$/.test(v.name.trim())
    );
    
    if (hasDuplicateNames || hasInvalidNames) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Validation Error', 
        detail: hasDuplicateNames 
          ? 'Variables must have unique names' 
          : 'Variable names can only contain letters and numbers'
      });
      return null;
    }
    
    // Filter and format messages
    const filteredMessages = messages
      .filter(message => message.content && typeof message.content === 'string' && message.content.trim().length > 0)
      .map(message => ({
        role: message.role,
        content: message.content.trim()
      }));
    
    // Ensure we have at least one message
    if (filteredMessages.length === 0) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Validation Error', 
        detail: 'At least one message with content is required' 
      });
      return null;
    }
    
    // Convert variables array to object with keys as variable names
    const variablesObject: Record<string, ActionPromptVariable> = {};
    
    // Filter out variables with empty names and convert to object
    variables
      .filter(variable => variable.name && variable.name.trim().length > 0)
      .forEach(variable => {
        variablesObject[variable.name.trim()] = {
          description: variable.description || '',
          defaultValue: variable.defaultValue || '',
          required: Boolean(variable.required)
        };
      });
    
    // Return the prepared data
    return {
      name: name.trim(),
      description: description.trim(),
      variables: variablesObject,
      messages: filteredMessages
    };
  };

  // Handle save
  const handleSave = async () => {
    // Prepare the data
    const promptData = preparePromptData();
    
    // Return if data preparation failed
    if (!promptData) return;
    
    // Start saving
    setSaving(true);
    
    try {
      // Determine if we're creating or updating
      const isUpdating = Boolean(editPrompt?.id);
      
      // Perform the API call
      if (isUpdating) {
        await actionPromptService.updateActionPrompt(editPrompt!.id!, promptData);
      } else {
        await actionPromptService.createActionPrompt(promptData);
      }
      
      // Show success message
      toast.current?.show({ 
        severity: 'success', 
        summary: 'Success', 
        detail: `Action prompt ${isUpdating ? 'updated' : 'created'} successfully` 
      });
      
      // Close the dialog and refresh the list
      onSave();
      onHide();
    } catch (error) {
      // Log the error and show an error message
      console.error('Error saving action prompt:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: `Failed to ${editPrompt?.id ? 'update' : 'create'} action prompt` 
      });
    } finally {
      // Always reset the saving state
      setSaving(false);
    }
  };

  // Available role options for messages
  const roleOptions = [
    { label: 'User', value: 'User' },
    { label: 'Assistant', value: 'Assistant' }
  ];

  const dialogFooter = (
    <div>
      <Button 
        label="Cancel" 
        icon="pi pi-times" 
        className="p-button-text" 
        onClick={onHide} 
        disabled={saving}
      />
      <Button 
        label="Save" 
        icon="pi pi-check" 
        className="p-button-primary" 
        onClick={handleSave} 
        loading={saving}
        disabled={saving || !isFormValid}
      />
    </div>
  );

  return (
    <>
      <Toast ref={toast} position="top-right" />
      <Dialog
        header={
          <div className="dialog-header">
            <i className={`pi ${editPrompt?.id ? 'pi-pencil' : 'pi-plus-circle'} header-icon`}></i>
            <span>{editPrompt?.id ? 'Edit Action Prompt' : 'Create New Action Prompt'}</span>
          </div>
        }
        visible={visible}
        style={{ width: '80vw' }}
        breakpoints={{ '960px': '90vw', '641px': '100vw' }}
        modal
        footer={dialogFooter}
        onHide={onHide}
        closeOnEscape={!saving}
        closable={!saving}
        draggable={false}
        resizable={false}
        maximizable
      >
        <div className="p-fluid">
          <div className="field">
            <label htmlFor="name" className="field-label">Name</label>
            <InputText
              id="name"
              value={name}
              onChange={(e) => {
                setName(e.target.value);
                // Clear name error if we have a value
                if (e.target.value.trim()) {
                  setNameError('');
                }
                // Run debounced validation
                debouncedValidation();
              }}
              className={nameError ? 'p-invalid' : ''}
              placeholder="Enter a descriptive name"
              maxLength={64}
              disabled={saving}
            />
            {nameError && <small className="p-error">{nameError}</small>}
          </div>

          <div className="field">
            <label htmlFor="description" className="field-label">Description</label>
            <InputText
              id="description"
              value={description}
              onChange={(e) => {
                setDescription(e.target.value);
                // Clear description error if we have a value
                if (e.target.value.trim()) {
                  setDescriptionError('');
                }
                // Run debounced validation
                debouncedValidation();
              }}
              className={descriptionError ? 'p-invalid' : ''}
              placeholder="Brief description of what this action prompt does"
              disabled={saving}
            />
            {descriptionError && <small className="p-error">{descriptionError}</small>}
          </div>

          <div className="field content-field">
            <div className="flex justify-content-between align-items-center mb-2">
              <label className="field-label">Variables</label>
              <Button
                type="button"
                icon="pi pi-plus"
                onClick={addVariable}
                disabled={saving}
                className="p-button-sm p-button-rounded"
                tooltip="Add variable"
              />
            </div>
            
            <div className="content-container mb-4">
              <Accordion 
                multiple 
                activeIndex={activeVariableIndexes}
                onTabChange={(e) => setActiveVariableIndexes(e.index as number[])}
              >
                {variables.map((variable, index) => (
                  <AccordionTab 
                    key={index}
                    header={
                      <div className="flex justify-content-between align-items-center w-full">
                        <span>{variable.name || `Variable ${index + 1}`}</span>
                        <Button
                          type="button"
                          icon="pi pi-trash"
                          onClick={(e) => {
                            e.stopPropagation();
                            removeVariable(index);
                          }}
                          className="p-button-sm p-button-danger p-button-text"
                          disabled={saving}
                        />
                      </div>
                    }
                  >
                    <div className="p-grid mb-3">
                      <div className="p-fluid grid">
                        <div className="col-12 md:col-6 mb-2">
                          <label htmlFor={`var-name-${index}`} className="block mb-1">Name</label>
                          <InputText
                            id={`var-name-${index}`}
                            value={variable.name}
                            onChange={(e) => updateVariable(index, 'name', e.target.value)}
                            placeholder="variableName (letters/numbers only)"
                            className="w-full"
                            disabled={saving}
                          />
                          {variable.name && <small className="text-color-secondary">Will be displayed as: {`{{ ${variable.name} }}`}</small>}
                        </div>
                        <div className="col-12 md:col-6 mb-2">
                          <label htmlFor={`var-default-${index}`} className="block mb-1">Default Value</label>
                          <InputText
                            id={`var-default-${index}`}
                            value={variable.defaultValue || ''}
                            onChange={(e) => updateVariable(index, 'defaultValue', e.target.value)}
                            placeholder="Default value"
                            className="w-full"
                            disabled={saving}
                          />
                        </div>
                      </div>
                      <div className="col-12 mb-2">
                        <label htmlFor={`var-desc-${index}`} className="block mb-1">Description</label>
                        <InputText
                          id={`var-desc-${index}`}
                          value={variable.description}
                          onChange={(e) => updateVariable(index, 'description', e.target.value)}
                          placeholder="Variable description"
                          className="w-full"
                          disabled={saving}
                        />
                      </div>
                      <div className="col-12">
                        <div className="flex align-items-center">
                          <input
                            type="checkbox"
                            id={`var-required-${index}`}
                            checked={variable.required}
                            onChange={(e) => updateVariable(index, 'required', e.target.checked)}
                            className="mr-2"
                            disabled={saving}
                          />
                          <label htmlFor={`var-required-${index}`}>Required</label>
                        </div>
                      </div>
                    </div>
                  </AccordionTab>
                ))}
              </Accordion>
              
              {variables.length === 0 && (
                <div className="text-center p-3 text-color-secondary">
                  No variables defined. Click the + button to add a variable.
                </div>
              )}
            </div>

            <div className="flex justify-content-between align-items-center mb-2">
              <label className="field-label">Messages</label>
              <Button
                type="button"
                icon="pi pi-plus"
                onClick={addMessage}
                disabled={saving}
                className="p-button-sm p-button-rounded"
                tooltip="Add message"
              />
            </div>
            {messagesError && <small className="p-error block mb-2">{messagesError}</small>}
            
            <div className="content-container">
              <Accordion 
                multiple 
                activeIndex={activeMessageIndexes}
                onTabChange={(e) => setActiveMessageIndexes(e.index as number[])}
              >
                {messages.map((message, index) => (
                  <AccordionTab 
                    key={index}
                    header={
                      <div className="flex justify-content-between align-items-center w-full">
                        <span>{`${message.role} Message`}</span>
                        {messages.length > 1 && (
                          <Button
                            type="button"
                            icon="pi pi-trash"
                            onClick={(e) => {
                              e.stopPropagation();
                              removeMessage(index);
                            }}
                            className="p-button-sm p-button-danger p-button-text"
                            disabled={saving}
                          />
                        )}
                      </div>
                    }
                  >
                    <div className="mb-3">
                      <label htmlFor={`message-role-${index}`} className="block mb-1">Role</label>
                      <Dropdown
                        id={`message-role-${index}`}
                        value={message.role}
                        options={roleOptions}
                        onChange={(e) => updateMessage(index, 'role', e.value)}
                        className="w-full"
                        disabled={saving}
                      />
                    </div>
                    
                    <div>
                      <label htmlFor={`message-content-${index}`} className="block mb-1">Content</label>
                      <InputTextarea
                        id={`message-content-${index}`}
                        value={typeof message.content === 'string' ? message.content : 
                              (typeof message.content === 'object' && message.content && 'text' in message.content) ? 
                                String(message.content.text) : ''}
                        onChange={(e) => updateMessageContent(index, e.target.value)}
                        autoResize
                        rows={6}
                        style={{ overflow: 'auto', height: 'auto', minHeight: '150px' }}
                        className="w-full prompt-textarea"
                        placeholder="Enter message content"
                        disabled={saving}
                        onWheel={(e) => e.stopPropagation()} // Prevent wheel events from propagating to parent
                      />
                      
                      {/* Preview panel with variable highlighting in collapsible container */}
                      {message.content && variables.some(v => v.name) && (
                        <div className="mt-2">
                          <Button
                            type="button"
                            className="p-button-text p-button-sm w-full text-left flex align-items-center"
                            onClick={(e) => {
                              e.preventDefault();
                              const previewEl = document.getElementById(`preview-${index}`);
                              if (previewEl) {
                                const isVisible = previewEl.style.display !== 'none';
                                previewEl.style.display = isVisible ? 'none' : 'block';
                                (e.target as HTMLElement).querySelector('.pi')?.classList.toggle('pi-chevron-down');
                                (e.target as HTMLElement).querySelector('.pi')?.classList.toggle('pi-chevron-right');
                              }
                            }}
                          >
                            <i className="pi pi-chevron-right mr-2"></i>
                            <span>Preview with variable highlighting</span>
                          </Button>
                          <div id={`preview-${index}`} className="message-preview" style={{ display: 'none' }}>
                            <HighlightVariables 
                              content={typeof message.content === 'string' ? message.content : 
                                       (typeof message.content === 'object' && message.content && 'text' in message.content) ? 
                                       String(message.content.text) : ''} 
                              variables={variables} 
                            />
                          </div>
                        </div>
                      )}
                    </div>
                  </AccordionTab>
                ))}
              </Accordion>
            </div>
          </div>
        </div>
      </Dialog>
    </>
  );
};

export { ActionPromptDialog };
export default ActionPromptDialog;