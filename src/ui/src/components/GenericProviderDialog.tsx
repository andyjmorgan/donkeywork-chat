import React, { useState, useEffect, useRef } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { Password } from 'primereact/password';
import { InputNumber } from 'primereact/inputnumber';
import { InputSwitch } from 'primereact/inputswitch';
import { Divider } from 'primereact/divider';
import { Toast } from 'primereact/toast';
import { GenericProviderType, GenericProviderConfigurationModel, GenericProviderPropertyType, GenericProviderPropertyModel } from '../models/api/provider';
import { ToolProviderType } from '../models/api/provider/ToolProviderType';
import { integrationsService } from '../services/api';
import './GenericProviderDialog.css';

interface GenericProviderDialogProps {
  visible: boolean;
  onHide: () => void;
  providerType: GenericProviderType | ToolProviderType | null;
  providerImage?: string; // Add provider image URL
  providerName?: string;  // Add provider name
  onSave: () => void;
}

const GenericProviderDialog: React.FC<GenericProviderDialogProps> = ({ 
  visible, 
  onHide, 
  providerType,
  providerImage,
  providerName, 
  onSave 
}) => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [config, setConfig] = useState<GenericProviderConfigurationModel | null>(null);
  const [isEnabled, setIsEnabled] = useState(true);
  const [errors, setErrors] = useState<Record<string, boolean>>({});
  const toast = useRef<Toast>(null);

  useEffect(() => {
    if (visible && providerType) {
      loadConfiguration();
    } else {
      setConfig(null);
      setErrors({});
      setIsEnabled(true); // Reset to enabled by default
    }
  }, [visible, providerType]);
  
  // Update isEnabled state when configuration loads
  useEffect(() => {
    if (config) {
      setIsEnabled(config.isEnabled);
    }
  }, [config]);

  const loadConfiguration = async () => {
    if (!providerType) return;
    
    try {
      setLoading(true);
      const config = await integrationsService.getGenericProviderConfiguration(providerType as ToolProviderType);
      
      // Log the received configuration for debugging
      console.log('Received provider configuration:', config);
      
      // Ensure we have the properties - if using camelCase, may need to access differently
      if (!config.properties && (config as any).Properties) {
        config.properties = (config as any).Properties;
      }
      
      setConfig(config);
    } catch (error) {
      console.error("Failed to load provider configuration:", error);
      // Initialize a default config if none exists
      setConfig({
        providerType: providerType as GenericProviderType,
        isEnabled: false,
        properties: {}
      });
    } finally {
      setLoading(false);
    }
  };

  // Function to check if form is valid
  const isFormValid = (): boolean => {
    if (!config) return false;
    
    // Check that all required fields have values
    return !Object.entries(config.properties || {}).some(([_, prop]) => {
      return prop.required && !prop.value;
    });
  };

  const handleSave = async () => {
    if (!config) return;
    
    // Validate required fields
    const newErrors: Record<string, boolean> = {};
    let hasErrors = false;
    
    Object.entries(config.properties || {}).forEach(([key, prop]) => {
      if (prop.required && !prop.value) {
        newErrors[key] = true;
        hasErrors = true;
      }
    });
    
    setErrors(newErrors);
    if (hasErrors) return;
    
    try {
      setSaving(true);
      
      // Prepare the data for saving - ensure property names match expectations
      const saveConfig: GenericProviderConfigurationModel = {
        providerType: config.providerType,
        isEnabled: isEnabled, // Use the isEnabled state value 
        properties: {}
      };
      
      // Preserve all properties exactly as they were received
      if (config.properties) {
        // Copy the entire properties object while updating only the values
        saveConfig.properties = JSON.parse(JSON.stringify(config.properties));
      }
      
      console.log("Saving configuration:", JSON.stringify(saveConfig, null, 2));
      
      await integrationsService.upsertGenericProviderConfiguration(saveConfig);
      toast.current?.show({ 
        severity: 'success', 
        summary: 'Success', 
        detail: `${providerName || String(providerType)} provider configured successfully`
      });
      onSave();
      onHide();
    } catch (error) {
      console.error("Failed to save provider configuration:", error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: error instanceof Error ? error.message : String(error),
        life: 5000
      });
    } finally {
      setSaving(false);
    }
  };

  const updatePropertyValue = (key: string, value: any) => {
    if (!config) return;
    
    // Create a deep copy to avoid modifying the original reference
    const updatedConfig = {
      ...config,
      properties: {
        ...config.properties
      }
    };
    
    // Update the value in the property
    if (updatedConfig.properties[key]) {
      updatedConfig.properties[key] = {
        ...updatedConfig.properties[key],
        value
      };
    }
    
    setConfig(updatedConfig);
    
    // Clear error if value is now valid
    if (errors[key] && value) {
      const newErrors = { ...errors };
      delete newErrors[key];
      setErrors(newErrors);
    }
    
  };

  const renderFormField = (key: string, property: GenericProviderPropertyModel) => {
    const hasError = errors[key];
    
    // Handle string type names ("String", "Secret", etc.) or enum values (0, 1, etc.)
    let propType: GenericProviderPropertyType;
    
    if (typeof property.type === 'number') {
      propType = property.type;
    } else if (typeof property.type === 'string') {
      // If type is a string like "String", convert to enum value
      switch (property.type) {
        case "String":
          propType = GenericProviderPropertyType.String;
          break;
        case "Secret":
          propType = GenericProviderPropertyType.Secret;
          break;
        case "Boolean":
          propType = GenericProviderPropertyType.Boolean;
          break;
        case "Integer":
          propType = GenericProviderPropertyType.Integer;
          break;
        case "Double":
          propType = GenericProviderPropertyType.Double;
          break;
        default:
          // If we got a numeric string like "0" or "1", parse it
          propType = parseInt(property.type, 10) as GenericProviderPropertyType;
      }
    } else {
      // Default to string type if we can't determine
      propType = GenericProviderPropertyType.String;
    }
    
    
    switch (propType) {
      case GenericProviderPropertyType.String:
        return (
          <div className="field">
            <label htmlFor={key} className={hasError ? 'p-error' : ''}>
              {property.friendlyName}{property.required ? ' *' : ''}
            </label>
            <InputText
              id={key}
              value={property.value || ''}
              onChange={(e) => updatePropertyValue(key, e.target.value)}
              className={hasError ? 'p-invalid w-full' : 'w-full'}
              required={property.required}
            />
            {hasError && <small className="p-error">This field is required</small>}
          </div>
        );
        
      case GenericProviderPropertyType.Secret:
        return (
          <div className="field">
            <label htmlFor={key} className={hasError ? 'p-error' : ''}>
              {property.friendlyName}{property.required ? ' *' : ''}
            </label>
            <Password
              id={key}
              value={property.value || ''}
              onChange={(e) => updatePropertyValue(key, e.target.value)}
              className={hasError ? 'p-invalid w-full' : 'w-full'}
              feedback={false}
              toggleMask
              required={property.required}
            />
            {hasError && <small className="p-error">This field is required</small>}
          </div>
        );
        
      case GenericProviderPropertyType.Boolean:
        return (
          <div className="field">
            <div className="flex align-items-center">
              <InputSwitch
                checked={!!property.value}
                onChange={(e) => updatePropertyValue(key, e.value)}
              />
              <label htmlFor={key} className="ml-2">
                {property.friendlyName}
              </label>
            </div>
          </div>
        );
        
      case GenericProviderPropertyType.Integer:
        return (
          <div className="field">
            <label htmlFor={key} className={hasError ? 'p-error' : ''}>
              {property.friendlyName}{property.required ? ' *' : ''}
            </label>
            <InputNumber
              id={key}
              value={property.value}
              onValueChange={(e) => updatePropertyValue(key, e.value)}
              className={hasError ? 'p-invalid w-full' : 'w-full'}
              required={property.required}
            />
            {hasError && <small className="p-error">This field is required</small>}
          </div>
        );
        
      case GenericProviderPropertyType.Double:
        return (
          <div className="field">
            <label htmlFor={key} className={hasError ? 'p-error' : ''}>
              {property.friendlyName}{property.required ? ' *' : ''}
            </label>
            <InputNumber
              id={key}
              value={property.value}
              onValueChange={(e) => updatePropertyValue(key, e.value)}
              className={hasError ? 'p-invalid w-full' : 'w-full'}
              mode="decimal"
              minFractionDigits={1}
              required={property.required}
            />
            {hasError && <small className="p-error">This field is required</small>}
          </div>
        );
        
      default:
          // Default to string input if type is unknown
        return (
          <div className="field">
            <label htmlFor={key} className={hasError ? 'p-error' : ''}>
              {property.friendlyName}{property.required ? ' *' : ''}
            </label>
            <InputText
              id={key}
              value={property.value || ''}
              onChange={(e) => updatePropertyValue(key, e.target.value)}
              className={hasError ? 'p-invalid w-full' : 'w-full'}
              required={property.required}
            />
            {hasError && <small className="p-error">This field is required</small>}
          </div>
        );
    }
  };

  const footerContent = (
    <div>
      <Button label="Cancel" icon="pi pi-times" onClick={onHide} className="p-button-text" />
      <Button 
        label="Save" 
        icon="pi pi-check" 
        onClick={handleSave} 
        loading={saving} 
        disabled={!config || !isFormValid()}
      />
    </div>
  );

  const dialogTitle = `${config?.isEnabled && config ? 'Modify' : 'Connect'} ${providerName || providerType} Provider`;

  
  return (
    <>
      <Toast ref={toast} />
      <Dialog 
        visible={visible} 
        onHide={onHide}
        header={
          <div className="flex align-items-center gap-2">
            {providerImage && (
              <img src={providerImage} alt={providerName || String(providerType)} className="w-2rem h-2rem mr-2" />
            )}
            <span>{dialogTitle}</span>
          </div>
        }
        footer={footerContent}
        style={{ width: '550px' }}
        modal
        blockScroll
      >
      {loading ? (
        <div className="flex justify-content-center">
          <i className="pi pi-spin pi-spinner" style={{ fontSize: '2rem' }}></i>
        </div>
      ) : config ? (
        <div className="p-fluid generic-provider-dialog">
          <Divider />
          
          {/* Enable/disable toggle */}
          <div className="field mb-4">
            <div className="flex align-items-center gap-2">
              <InputSwitch 
                checked={isEnabled} 
                onChange={(e) => setIsEnabled(e.value)}
              />
              <label className="ml-2 font-medium">
                {isEnabled ? 'Enabled' : 'Disabled'}
              </label>
            </div>
            <small className="text-500 mt-1 block">
              {isEnabled 
                ? 'The provider will be available for use with AI models' 
                : 'The provider will be saved but not available for use'
              }
            </small>
          </div>
          
          <Divider />
          
          {/* Show debug info if properties are empty */}
          {(!config.properties || Object.keys(config.properties).length === 0) && (
            <div className="p-message p-message-warn mb-3">
              <div className="p-message-wrapper">
                <span className="p-message-text">No properties found in configuration</span>
              </div>
            </div>
          )}
          
          
          {Object.entries(config.properties || {}).map(([key, property]) => (
            <React.Fragment key={key}>
              {renderFormField(key, property)}
            </React.Fragment>
          ))}
        </div>
      ) : null}
    </Dialog>
    </>
  );
};

export default GenericProviderDialog;