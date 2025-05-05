import React, { useState, useRef, useEffect } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Toast } from 'primereact/toast';
import './AgentDialog.css';
import { AgentBuilder } from '../../../components/agent';
import { AgentModel } from '../../../services/api/agentService';

// Agent model for editing
interface EditAgentModel {
  id?: string;
  name: string;
  description: string;
  tags?: string[];
}

interface AgentDialogProps {
  visible: boolean;
  onHide: () => void;
  onSave: () => void;
  editAgent?: EditAgentModel; // Agent to edit
}

/**
 * A reusable component for creating or editing agents
 */
const AgentDialog: React.FC<AgentDialogProps> = ({ visible, onHide, onSave, editAgent }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [saving, setSaving] = useState(false);
  const [nameError, setNameError] = useState('');
  const [descriptionError, setDescriptionError] = useState('');
  const [isFormValid, setIsFormValid] = useState(false);
  const [agentJson, setAgentJson] = useState('');
  
  const toast = useRef<Toast>(null);

  // Reset form when the dialog is opened or editAgent changes
  useEffect(() => {
    if (visible) {
      if (editAgent) {
        setName(editAgent.name || '');
        setDescription(editAgent.description || '');
      } else {
        // Clear all fields for new agent
        setName('');
        setDescription('');
        setAgentJson('');
      }
      
      setNameError('');
      setDescriptionError('');
      setIsFormValid(false);
    }
  }, [visible, editAgent]);

  // Check form validity
  const checkFormValidity = (): boolean => {
    // Basic validation - name and description are required
    const basicValid = name.trim().length > 0 && description.trim().length > 0;
    return basicValid;
  };

  // Update form validity when required fields change
  useEffect(() => {
    setIsFormValid(checkFormValidity());
  }, [name, description]);

  // Validate the form
  const validateForm = (): boolean => {
    let isValid = true;
    
    // Name validation
    if (!name.trim()) {
      setNameError('Name is required');
      isValid = false;
    } else {
      setNameError('');
    }
    
    // Description validation
    if (!description.trim()) {
      setDescriptionError('Description is required');
      isValid = false;
    } else {
      setDescriptionError('');
    }
    
    setIsFormValid(isValid);
    return isValid;
  };

  // Handle save
  const handleSave = async () => {
    if (!validateForm()) {
      return;
    }
    
    setSaving(true);
    
    try {
      // The AgentBuilder component handles saving to the backend
      // This dialog is only responsible for validation and closing the dialog
      
      // Call onSave to refresh the agent list
      onSave();
      onHide();
    } catch (error) {
      console.error('Error during dialog save:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'An error occurred while processing the dialog'
      });
    } finally {
      setSaving(false);
    }
  };

  // Handle agent data from the builder
  const handleAgentDataSave = (agentJsonData: string) => {
    setAgentJson(agentJsonData);
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
            <i className={`pi ${editAgent?.id ? 'pi-pencil' : 'pi-plus-circle'} header-icon`}></i>
            <span>{editAgent?.id ? 'Edit Agent' : 'Create New Agent'}</span>
          </div>
        }
        visible={visible}
        style={{ width: '90vw', height: '90vh' }}
        contentStyle={{ height: 'calc(90vh - 120px)', overflow: 'auto' }}
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
          <div className="grid">
            <div className="col-12">
              <div className="field">
                <label htmlFor="name" className="field-label">Name</label>
                <InputText
                  id="name"
                  value={name}
                  onChange={(e) => {
                    setName(e.target.value);
                    if (e.target.value.trim()) {
                      setNameError('');
                    }
                  }}
                  className={nameError ? 'p-invalid' : ''}
                  placeholder="Enter a descriptive name"
                  maxLength={64}
                  disabled={saving}
                />
                {nameError && <small className="p-error">{nameError}</small>}
              </div>
            </div>
            
            <div className="col-12">
              <div className="field">
                <label htmlFor="description" className="field-label">Description</label>
                <InputTextarea
                  id="description"
                  value={description}
                  onChange={(e) => {
                    setDescription(e.target.value);
                    if (e.target.value.trim()) {
                      setDescriptionError('');
                    }
                  }}
                  rows={3}
                  className={descriptionError ? 'p-invalid' : ''}
                  placeholder="Brief description of what this agent does"
                  disabled={saving}
                />
                {descriptionError && <small className="p-error">{descriptionError}</small>}
              </div>
            </div>
            
            <div className="col-12">
              <div className="field">
                <label className="field-label">Agent Builder</label>
                <div className="agent-builder-wrapper">
                  <AgentBuilder
                    onSave={handleAgentDataSave}
                    agentId={editAgent?.id}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </Dialog>
    </>
  );
};

export { AgentDialog };
export default AgentDialog;