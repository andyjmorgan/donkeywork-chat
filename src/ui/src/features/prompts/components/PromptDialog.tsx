import React, { useState, useEffect, useRef } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Toast } from 'primereact/toast';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { UpsertPromptModel } from '../../../models/api/Prompt/UpsertPromptModel';
import { promptService } from '../../../services/api/promptService';
import { PromptDialogProps } from '../../../models/ui/prompts/PromptDialogTypes';
import './PromptDialog.css';

/**
 * A reusable component for creating or editing prompts
 */
const PromptDialog: React.FC<PromptDialogProps> = ({ visible, onHide, onSave, editPrompt }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [content, setContent] = useState<string[]>(['']);
  const [saving, setSaving] = useState(false);
  const [nameError, setNameError] = useState('');
  const [descriptionError, setDescriptionError] = useState('');
  const [contentError, setContentError] = useState('');
  const [isFormValid, setIsFormValid] = useState(false);
  // Track which content items are expanded
  const [activeContentIndexes, setActiveContentIndexes] = useState<number[]>([0]);
  const toast = useRef<Toast>(null);

  // Reset form when the dialog is opened or editPrompt changes
  useEffect(() => {
    if (visible) {
      let initialContent: string[] = [''];
      
      if (editPrompt) {
        setName(editPrompt.name);
        setDescription(editPrompt.description);
        initialContent = editPrompt.content && editPrompt.content.length > 0 ? editPrompt.content : [''];
        setContent(initialContent);
      } else {
        // Clear form for new prompt
        setName('');
        setDescription('');
        setContent(initialContent);
      }
      
      // Clear errors
      setNameError('');
      setDescriptionError('');
      setContentError('');
      
      // Set initial active indexes to expand all content items
      setActiveContentIndexes(initialContent.map((_, i) => i));
      
      // Set initial form validity
      const hasName = editPrompt && editPrompt.name ? editPrompt.name.trim().length > 0 : false;
      const hasDescription = editPrompt && editPrompt.description ? editPrompt.description.trim().length > 0 : false;
      const hasContent = editPrompt && editPrompt.content ? editPrompt.content.some(item => item.trim().length > 0) : false;
      setIsFormValid(hasName && hasDescription && hasContent);
    }
  }, [visible, editPrompt]);

  // Add a new item to the content list
  const addContentItem = () => {
    const newContent = [...content, ''];
    setContent(newContent);
    // Ensure the new item is expanded by updating the active indexes
    setActiveContentIndexes([...activeContentIndexes, newContent.length - 1]);
  };

  // Remove an item from the content list
  const removeContentItem = (index: number) => {
    if (content.length <= 1) {
      // Don't remove the last item
      return;
    }
    
    // Update content
    const newContent = [...content];
    newContent.splice(index, 1);
    setContent(newContent);
    
    // Update active indexes to maintain expanded state
    const newActiveIndexes = activeContentIndexes
      .filter(i => i !== index)  // Remove the deleted index
      .map(i => i > index ? i - 1 : i);  // Adjust indexes after the deleted one
    
    setActiveContentIndexes(newActiveIndexes);
    setTimeout(() => validateForm(), 0);
  };

  // Update a content item at a specific index
  const updateContentItem = (index: number, value: string) => {
    const newContent = [...content];
    newContent[index] = value;
    setContent(newContent);
    setTimeout(() => validateForm(), 0);
  };


  // Validate form
  const validateForm = (): boolean => {
    let isValid = true;
    
    // Name validation - required and max 64 characters
    const trimmedName = name.trim();
    if (!trimmedName) {
      setNameError('Name is required');
      isValid = false;
    } else if (trimmedName.length > 64) {
      setNameError(`Name must be 64 characters or less (currently ${trimmedName.length})`);
      isValid = false;
    } else {
      setNameError('');
    }
    
    // Description validation - required
    if (!description.trim()) {
      setDescriptionError('Description is required');
      isValid = false;
    } else {
      setDescriptionError('');
    }
    
    // Content validation - at least one non-empty item
    const hasNonEmptyContent = content.some(item => item.trim().length > 0);
    if (!hasNonEmptyContent) {
      setContentError('At least one content item is required');
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
    const trimmedName = name.trim();
    const trimmedDescription = description.trim();
    // Filter out empty content items
    const filteredContent = content.filter(item => item.trim().length > 0);
    
    if (!trimmedName || !trimmedDescription || filteredContent.length === 0) {
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Validation Error', 
        detail: 'Name, description, and at least one content item are required' 
      });
      return;
    }
    
    setSaving(true);
    
    try {
      const promptData: UpsertPromptModel = {
        name: trimmedName,
        description: trimmedDescription,
        content: filteredContent
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
            <label htmlFor="name" className="field-label">Name</label>
            <InputText
              id="name"
              value={name}
              onChange={(e) => {
                setName(e.target.value);
                setTimeout(() => validateForm(), 0);
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
                setTimeout(() => validateForm(), 0);
              }}
              className={descriptionError ? 'p-invalid' : ''}
              placeholder="Brief description of what this prompt does"
              disabled={saving}
            />
            {descriptionError && <small className="p-error">{descriptionError}</small>}
          </div>

          <div className="field content-field">
            <div className="flex justify-content-between align-items-center mb-2">
              <label className="field-label">Content</label>
              <Button
                type="button"
                icon="pi pi-plus"
                onClick={addContentItem}
                disabled={saving}
                className="p-button-sm p-button-rounded"
                tooltip="Add content item"
              />
            </div>
            {contentError && <small className="p-error block mb-2">{contentError}</small>}
            
            <div className="content-container">
              <Accordion 
                multiple 
                activeIndex={activeContentIndexes}
                onTabChange={(e) => setActiveContentIndexes(e.index as number[])}>
              {content.map((item, index) => (
                <AccordionTab 
                  key={index}
                  header={
                    <div className="flex justify-content-between align-items-center w-full">
                      <span>{`Content ${index + 1}`}</span>
                      {content.length > 1 && (
                        <Button
                          type="button"
                          icon="pi pi-trash"
                          onClick={(e) => {
                            e.stopPropagation();
                            removeContentItem(index);
                          }}
                          className="p-button-sm p-button-danger p-button-text"
                          disabled={saving}
                        />
                      )}
                    </div>
                  }
                >
                  <InputTextarea
                    value={item}
                    onChange={(e) => updateContentItem(index, e.target.value)}
                    autoResize
                    rows={6}
                    style={{ overflow: 'auto', height: 'auto', minHeight: '150px' }}
                    className="w-full prompt-textarea"
                    placeholder="Enter the content..."
                    disabled={saving}
                    onWheel={(e) => e.stopPropagation()} // Prevent wheel events from propagating to parent
                  />
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

export default PromptDialog;