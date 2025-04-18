import React, { useState, useEffect } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Checkbox } from 'primereact/checkbox';
import { classNames } from 'primereact/utils';
import { Toast } from 'primereact/toast';

import { ApiKeyDialogProps } from '../../../models/ui/apikey/ApiKeyDialogTypes';
import { apiKeyService } from '../../../services/api';
import { UpsertApiKeyModel } from '../../../models/api/ApiKey';

const ApiKeyDialog: React.FC<ApiKeyDialogProps> = ({ 
  visible, 
  onHide, 
  apiKey, 
  onSave,
  editMode = false
}) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [isEnabled, setIsEnabled] = useState(true);
  const [submitted, setSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [newApiKey, setNewApiKey] = useState('');
  const [copied, setCopied] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  
  const toast = React.useRef<Toast>(null);

  // Reset form when dialog visibility changes
  useEffect(() => {
    if (visible) {
      if (apiKey) {
        // Edit mode - populate form with existing data
        setName(apiKey.name);
        setDescription(apiKey.description || '');
        setIsEnabled(apiKey.isEnabled);
        setNewApiKey(apiKey.apiKey);
        setIsEditMode(editMode);
      } else {
        // Create mode - reset form
        setName('');
        setDescription('');
        setIsEnabled(true);
        setNewApiKey('');
        setIsEditMode(false);
      }
      setSubmitted(false);
      setCopied(false);
    }
  }, [visible, apiKey, editMode]);

  const handleSave = async () => {
    setSubmitted(true);
    
    // Form validation
    if (!name.trim()) {
      return;
    }

    try {
      setLoading(true);
      
      const model: UpsertApiKeyModel = {
        name,
        description: description || undefined,
        isEnabled
      };

      if (apiKey) {
        // Update existing key
        await apiKeyService.updateApiKey(apiKey.id, model);
        toast.current?.show({ 
          severity: 'success', 
          summary: 'Success', 
          detail: 'API key updated successfully' 
        });
      } else {
        // Create new key
        const result = await apiKeyService.createApiKey(model);
        setNewApiKey(result.apiKey);
        toast.current?.show({ 
          severity: 'success', 
          summary: 'Success', 
          detail: 'API key created successfully. Make sure to copy your key now!' 
        });
      }
      
      onSave();
    } catch (error) {
      console.error('Error saving API key:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to save API key' 
      });
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = () => {
    setCopied(true);
    // Reset the copied state after 2 seconds
    setTimeout(() => {
      setCopied(false);
    }, 2000);
  };

  // Dialog footer depends on whether we're in view mode, edit mode, or create mode
  const dialogFooter = () => {
    if (apiKey && !isEditMode) {
      // View mode
      return (
        <>
          <Button 
            label="Close" 
            icon="pi pi-times" 
            className="p-button-text" 
            onClick={onHide} 
          />
          <Button 
            label="Edit" 
            icon="pi pi-pencil" 
            className="p-button-primary" 
            onClick={() => setIsEditMode(true)} 
          />
        </>
      );
    } else {
      // Edit or create mode
      return (
        <>
          <Button 
            label="Cancel" 
            icon="pi pi-times" 
            className="p-button-text" 
            onClick={onHide} 
            disabled={loading}
          />
          <Button 
            label="Save" 
            icon="pi pi-check" 
            className="p-button-primary" 
            onClick={handleSave} 
            loading={loading}
          />
        </>
      );
    }
  };

  return (
    <>
      <Toast ref={toast} />
      
      <Dialog
        visible={visible}
        style={{ width: '550px' }}
        header={apiKey ? (isEditMode ? 'Edit API Key' : 'View API Key') : 'Create API Key'}
        modal
        className="p-fluid"
        footer={newApiKey && !apiKey ? undefined : dialogFooter()}
        onHide={onHide}
      >
        {newApiKey && !apiKey ? (
          // Display the new API key and copy button (only for newly created keys)
          <div className="p-4">
            <div className="mb-4">
              <h3 className="m-0 mb-2">Your API Key</h3>
              <p className="text-sm text-red-500 mb-3">
                This is the only time you'll see the full key. Please copy it now and store it securely.
              </p>
              
              <div className="p-inputgroup mb-3">
                <InputText
                  value={newApiKey}
                  className="w-full font-mono text-sm"
                  readOnly
                />
                <Button
                  type="button"
                  icon={copied ? "pi pi-check" : "pi pi-copy"}
                  className={copied ? "p-button-success" : ""}
                  tooltip={copied ? "Copied!" : "Copy to clipboard"}
                  onClick={() => {
                    navigator.clipboard.writeText(newApiKey);
                    handleCopy();
                  }}
                />
              </div>
              
              <Button
                label="Done"
                icon="pi pi-check"
                className="p-button-primary w-full"
                onClick={onHide}
              />
            </div>
          </div>
        ) : apiKey && !isEditMode ? (
          // View mode for existing API keys
          <div className="p-fluid">
            <div className="field">
              <label className="font-bold">Name</label>
              <div className="text-lg">{name}</div>
            </div>

            <div className="field">
              <label className="font-bold">Description</label>
              <div className="text-lg">{description || <em className="text-color-secondary">No description</em>}</div>
            </div>

            <div className="field">
              <label className="font-bold">Status</label>
              <div>
                {isEnabled ? 
                  <span className="p-tag p-tag-success">Enabled</span> : 
                  <span className="p-tag p-tag-danger">Disabled</span>
                }
              </div>
            </div>
            
            <div className="field">
              <label className="font-bold">API Key</label>
              <div className="p-inputgroup">
                <InputText
                  value={newApiKey}
                  className="font-mono text-sm"
                  readOnly
                />
                <Button
                  type="button"
                  icon={copied ? "pi pi-check" : "pi pi-copy"}
                  className={copied ? "p-button-success" : ""}
                  tooltip={copied ? "Copied!" : "Copy to clipboard"}
                  onClick={() => {
                    navigator.clipboard.writeText(newApiKey);
                    handleCopy();
                  }}
                />
              </div>
            </div>
          </div>
        ) : (
          // Form for creating/editing an API key
          <div className="p-fluid">
            <div className="field">
              <label htmlFor="name" className="font-bold">Name*</label>
              <InputText
                id="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
                autoFocus
                className={classNames({ 'p-invalid': submitted && !name.trim() })}
              />
              {submitted && !name.trim() && <small className="p-error">Name is required.</small>}
            </div>

            <div className="field">
              <label htmlFor="description" className="font-bold">Description</label>
              <InputTextarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                rows={3}
              />
            </div>

            <div className="field-checkbox">
              <Checkbox
                inputId="isEnabled"
                checked={isEnabled}
                onChange={(e) => setIsEnabled(e.checked || false)}
              />
              <label htmlFor="isEnabled" className="ml-2">Enabled</label>
            </div>
          </div>
        )}
      </Dialog>
    </>
  );
};

export default ApiKeyDialog;