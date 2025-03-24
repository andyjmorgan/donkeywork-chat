import React, { useState, useEffect, useRef } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Toast } from 'primereact/toast';
import { UpsertPromptModel } from '../../../models/api/Prompt/UpsertPromptModel';
import { promptService } from '../../../services/api/promptService';
import { PromptDialogProps } from '../../../models/ui/prompts/PromptDialogTypes';
import './PromptDialog.css';

/**
 * A reusable component for creating or editing prompts
 */
const PromptDialog: React.FC<PromptDialogProps> = ({ visible, onHide, onSave, editPrompt }) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [content, setContent] = useState('');
  const [saving, setSaving] = useState(false);
  const [titleError, setTitleError] = useState('');
  const [descriptionError, setDescriptionError] = useState('');
  const [contentError, setContentError] = useState('');
  const [isFormValid, setIsFormValid] = useState(false);
  const toast = useRef<Toast>(null);

  // Reset form when the dialog is opened or editPrompt changes
  useEffect(() => {
    if (visible) {
      if (editPrompt) {
        setTitle(editPrompt.title);
        setDescription(editPrompt.description);
        setContent(editPrompt.content);
      } else {
        // Clear form for new prompt
        setTitle('');
        setDescription('');
        setContent('');
      }
      
      // Clear errors
      setTitleError('');
      setDescriptionError('');
      setContentError('');
      
      // Set initial form validity
      const hasTitle = editPrompt && editPrompt.title ? editPrompt.title.trim().length > 0 : false;
      const hasDescription = editPrompt && editPrompt.description ? editPrompt.description.trim().length > 0 : false;
      const hasContent = editPrompt && editPrompt.content ? editPrompt.content.trim().length > 0 : false;
      setIsFormValid(hasTitle && hasDescription && hasContent);
    }
  }, [visible, editPrompt]);

  // Validate form
  const validateForm = (): boolean => {
    let isValid = true;
    
    // Title validation - required and max 64 characters
    const trimmedTitle = title.trim();
    if (!trimmedTitle) {
      setTitleError('Title is required');
      isValid = false;
    } else if (trimmedTitle.length > 64) {
      setTitleError(`Title must be 64 characters or less (currently ${trimmedTitle.length})`);
      isValid = false;
    } else {
      setTitleError('');
    }
    
    // Description validation - required
    if (!description.trim()) {
      setDescriptionError('Description is required');
      isValid = false;
    } else {
      setDescriptionError('');
    }
    
    // Content validation - required
    if (!content.trim()) {
      setContentError('Content is required');
      isValid = false;
    } else {
      setContentError('');
    }
    
    // Update form validity state
    setIsFormValid(isValid);
    
    return isValid;
  };

  // Handle save
  const handleSave = async () => {
    // Validate the form
    if (!validateForm()) {
      // Show a toast message if validation fails
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Validation Error', 
        detail: 'Please fix the errors before saving' 
      });
      return;
    }
    
    // Ensure all required fields have values
    const trimmedTitle = title.trim();
    const trimmedDescription = description.trim();
    const trimmedContent = content.trim();
    
    if (!trimmedTitle || !trimmedDescription || !trimmedContent) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Validation Error', 
        detail: 'Title, description, and content are all required' 
      });
      return;
    }
    
    setSaving(true);
    
    try {
      const promptData: UpsertPromptModel = {
        title: trimmedTitle,
        description: trimmedDescription,
        content: trimmedContent
      };
      
      if (editPrompt?.id) {
        // Update existing prompt
        await promptService.updatePrompt(editPrompt.id, promptData);
        toast.current?.show({ 
          severity: 'success', 
          summary: 'Success', 
          detail: 'Prompt updated successfully' 
        });
      } else {
        // Create new prompt
        await promptService.createPrompt(promptData);
        toast.current?.show({ 
          severity: 'success', 
          summary: 'Success', 
          detail: 'Prompt created successfully' 
        });
      }
      
      onSave();
      onHide();
    } catch (error) {
      console.error('Error saving prompt:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: `Failed to ${editPrompt?.id ? 'update' : 'create'} prompt` 
      });
    } finally {
      setSaving(false);
    }
  };

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
            <span>{editPrompt?.id ? 'Edit Prompt' : 'Create New Prompt'}</span>
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
            <label htmlFor="title" className="field-label">Title</label>
            <InputText
              id="title"
              value={title}
              onChange={(e) => {
                setTitle(e.target.value);
                setTimeout(() => validateForm(), 0);
              }}
              className={titleError ? 'p-invalid' : ''}
              placeholder="Enter a descriptive title"
              maxLength={64}
              disabled={saving}
            />
            {titleError && <small className="p-error">{titleError}</small>}
          </div>

          <div className="field">
            <label htmlFor="description" className="field-label">Description</label>
            <InputText
              id="description"
              value={description}
              onChange={(e) => {
                setDescription(e.target.value);
                setTimeout(() => validateForm(), 0);
              }}
              className={descriptionError ? 'p-invalid' : ''}
              placeholder="Brief description of what this prompt does"
              disabled={saving}
            />
            {descriptionError && <small className="p-error">{descriptionError}</small>}
          </div>

          <div className="field">
            <label htmlFor="content" className="field-label">Prompt Content</label>
            <InputTextarea
              id="content"
              value={content}
              onChange={(e) => {
                setContent(e.target.value);
                setTimeout(() => validateForm(), 0);
              }}
              autoResize
              rows={10}
              className={contentError ? 'p-invalid prompt-textarea' : 'prompt-textarea'}
              placeholder="Enter the prompt content..."
              disabled={saving}
              style={{ minHeight: '200px' }}
            />
            {contentError && <small className="p-error">{contentError}</small>}
          </div>
        </div>
      </Dialog>
    </>
  );
};

export default PromptDialog;