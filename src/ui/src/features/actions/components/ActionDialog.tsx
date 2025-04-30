import React, { useState, useEffect, useRef } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { InputNumber } from 'primereact/inputnumber';
import { Dropdown } from 'primereact/dropdown';
import { Toast } from 'primereact/toast';
import { TabView, TabPanel } from 'primereact/tabview';
import { MultiSelect } from 'primereact/multiselect';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Tag } from 'primereact/tag';
import { Skeleton } from 'primereact/skeleton';
import { formatDistanceToNow, parseISO } from 'date-fns';
import './ActionDialog.css'; // Import at the top of the file with other imports
import { actionService } from '../../../services/api/actionService';
import { promptService } from '../../../services/api/promptService';
import { actionPromptService } from '../../../services/api/actionPromptService';
import { actionExecutionService } from '../../../services/api/actionExecutionService';
import { UpsertActionModel } from '../../../models/api/Action';
import { GetPromptsItemModel } from '../../../models/api/Prompt';
import { ActionPromptItem } from '../../../models/api/Prompt';
import { ActionExecutionItem, ActionExecutionStatus } from '../../../models/api/ActionExecution';
import { modelsService } from '../../../services/api/modelsService';
import { integrationsService } from '../../../services/api/integrationsService';
import { BackendActionModelConfiguration } from '../../../models/api/Action/BackendActionModelConfiguration';
import { ToolProviderApplicationType } from '../../../models/api/provider/ToolProviderApplicationType';
import { ToolProviderModel } from '../../../models/api/provider/ToolProviderModel';
import { ToolProviderApplicationDefinition } from '../../../models/api/provider/ToolProviderApplicationType';

interface ActionDialogProps {
  visible: boolean;
  onHide: () => void;
  onSave: () => void;
  editAction?: UpsertActionModel & { id?: string };
}

/**
 * A reusable component for creating or editing actions
 */
const ActionDialog: React.FC<ActionDialogProps> = ({ visible, onHide, onSave, editAction }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [icon, setIcon] = useState('');
  const [systemPromptIds, setSystemPromptIds] = useState<string[]>([]);
  const [userPromptIds, setUserPromptIds] = useState<string[]>([]);
  const [allowedTools, setAllowedTools] = useState<ToolProviderApplicationType[]>([]);
  // Track UI state internally with provider and model directly in component state
  const [selectedProvider, setSelectedProvider] = useState<string>('');
  const [selectedModel, setSelectedModel] = useState<string>('');
  const [parameters, setParameters] = useState<Record<string, string>>({}); // Dictionary for model parameters
  // Temporary state for parameter being edited to preserve focus
  const [parameterBeingEdited, setParameterBeingEdited] = useState<{
    originalKey: string;
    currentKey: string;
    value: string;
  } | null>(null);
  
  const [saving, setSaving] = useState(false);
  const [nameError, setNameError] = useState('');
  const [descriptionError, setDescriptionError] = useState('');
  const [isFormValid, setIsFormValid] = useState(false);
  
  // State for available providers/models
  const [providers, setProviders] = useState<{ name: string; value: string }[]>([]);
  const [modelsForProvider, setModelsForProvider] = useState<{ name: string; value: string }[]>([]);
  const [isLoadingModels, setIsLoadingModels] = useState(false);
  
  // State for tool options with proper grouping structure
  const [toolOptions, setToolOptions] = useState<{ 
    label: string, 
    icon: string,
    isConnected?: boolean,
    items: { name: string, value: string, isConnected?: boolean }[] 
  }[]>([]);
  const [isLoadingTools, setIsLoadingTools] = useState(false);
  
  // State for available prompts
  const [systemPrompts, setSystemPrompts] = useState<GetPromptsItemModel[]>([]);
  const [actionPrompts, setActionPrompts] = useState<ActionPromptItem[]>([]);
  const [isLoadingPrompts, setIsLoadingPrompts] = useState(false);
  
  // State for action executions (logs)
  const [executions, setExecutions] = useState<ActionExecutionItem[]>([]);
  const [executionsCount, setExecutionsCount] = useState<number>(0);
  const [isLoadingExecutions, setIsLoadingExecutions] = useState(false);
  
  // Format timestamp to exact date and time
  const formatTimestamp = (timestamp: Date | string) => {
    const dateObj = timestamp instanceof Date ? timestamp : parseISO(timestamp as string);
    return dateObj.toISOString().replace('T', ' ').substring(0, 19);
  };

  // Template for timestamp column
  const timestampTemplate = (rowData: ActionExecutionItem) => {
    return <span className="text-sm">{formatTimestamp(rowData.createdAt)}</span>;
  };

  // Template for completion timestamp column
  const completionTimestampTemplate = (rowData: ActionExecutionItem) => {
    if (rowData.executionStatus === ActionExecutionStatus.Pending || 
        rowData.executionStatus === ActionExecutionStatus.InProgress) {
      return <span className="text-sm">In progress</span>;
    }
    return <span className="text-sm">{formatTimestamp(rowData.endTime)}</span>;
  };

  // Template for duration column
  const durationTemplate = (rowData: ActionExecutionItem) => {
    if (rowData.executionStatus === ActionExecutionStatus.Pending || 
        rowData.executionStatus === ActionExecutionStatus.InProgress) {
      return <span className="text-sm">In progress</span>;
    }
    
    const startDate = rowData.createdAt instanceof Date ? rowData.createdAt : parseISO(rowData.createdAt as string);
    const endDate = rowData.endTime instanceof Date ? rowData.endTime : parseISO(rowData.endTime as string);
    
    // Calculate duration in seconds
    const durationMs = endDate.getTime() - startDate.getTime();
    const durationSec = Math.round(durationMs / 1000);
    
    if (durationSec < 60) {
      return <span className="text-sm">{durationSec}s</span>;
    } else {
      const minutes = Math.floor(durationSec / 60);
      const seconds = durationSec % 60;
      return <span className="text-sm">{minutes}m {seconds}s</span>;
    }
  };

  // Template for status column
  const statusTemplate = (rowData: ActionExecutionItem) => {
    let severity: 'success' | 'info' | 'secondary' | 'contrast' | 'warning' | 'danger' | null | undefined = 'secondary';
    
    switch (rowData.executionStatus) {
      case ActionExecutionStatus.Completed:
        severity = 'success';
        break;
      case ActionExecutionStatus.Failed:
        severity = 'danger';
        break;
      case ActionExecutionStatus.Canceled:
        severity = 'warning';
        break;
      case ActionExecutionStatus.InProgress:
        severity = 'info';
        break;
      case ActionExecutionStatus.Pending:
      default:
        severity = 'secondary';
        break;
    }
    
    return <Tag value={rowData.executionStatus} severity={severity} />;
  };

  const toast = useRef<Toast>(null);

  // Fetch models when the dialog is opened
  useEffect(() => {
    const fetchModels = async () => {
      try {
        setIsLoadingModels(true);
        const response = await modelsService.getModels();
        
        if (!response || !response.allowedModels) {
          return;
        }
        
        // Extract providers from the response
        const providersFromAPI: { name: string; value: string }[] = Object.keys(response.allowedModels).map(key => ({
          name: key,
          value: key
        }));
        
        setProviders(providersFromAPI);
      } catch (error) {
        console.error('Error fetching models:', error);
        toast.current?.show({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load AI providers and models'
        });
      } finally {
        setIsLoadingModels(false);
      }
    };
    
    if (visible) {
      fetchModels();
      fetchPrompts();
    }
  }, [visible]);
  
  // Load models whenever the provider changes
  useEffect(() => {
    const loadModelsForProvider = async () => {
      if (!selectedProvider) return;
      
      try {
        setIsLoadingModels(true);
        const response = await modelsService.getModels();
        
        if (!response || !response.allowedModels) {
          return;
        }
        
        const providerModels = response.allowedModels[selectedProvider] || [];
        const mappedModels = providerModels.map(model => ({
          name: model,
          value: model
        }));
        
                setModelsForProvider(mappedModels);
      } catch (error) {
        console.error('Error fetching models for provider:', error);
      } finally {
        setIsLoadingModels(false);
      }
    };
    
    if (selectedProvider) {
      loadModelsForProvider();
    } else {
      // Clear models if no provider selected
      setModelsForProvider([]);
    }
    
    // Form validity will be updated through the useEffect tied to selectedModel
  }, [selectedProvider]);
  
  // Define checkFormValidity first before using it in useEffect
  const checkFormValidity = React.useCallback((): boolean => {
    // Basic validation - name and description are required
    const basicValid = name.trim().length > 0 && description.trim().length > 0;
    
    // If a provider is selected, a model should also be selected
    const modelValid = !selectedProvider || (selectedProvider && selectedModel);
    
    return Boolean(basicValid && modelValid);
  }, [name, description, selectedProvider, selectedModel]);

  // Update form validity when any required field changes
  useEffect(() => {
    setIsFormValid(checkFormValidity());
  }, [name, description, selectedProvider, selectedModel, checkFormValidity]);

  // Fetch prompts for selection
  const fetchPrompts = async () => {
    try {
      setIsLoadingPrompts(true);
      
      // Fetch both system prompts and action prompts in parallel
      const [systemPromptsResponse, actionPromptsResponse] = await Promise.all([
        promptService.getPrompts(0, 500),
        actionPromptService.getActionPrompts(0, 500)
      ]);
      
      // Extract system prompts (regular prompts)
      const systemPromptsArray = systemPromptsResponse.items || systemPromptsResponse.prompts || [];
      
      // Extract action prompts
      const actionPromptsArray = actionPromptsResponse.items || actionPromptsResponse.prompts || [];
      
      setSystemPrompts(systemPromptsArray);
      setActionPrompts(actionPromptsArray);
    } catch (error) {
      console.error('Error fetching prompts:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load prompts'
      });
    } finally {
      setIsLoadingPrompts(false);
    }
  };
  
  // Fetch action executions for this specific action
  const fetchActionExecutions = async (actionId: string) => {
    if (!actionId) return;
    
    try {
      setIsLoadingExecutions(true);
      const result = await actionExecutionService.getActionExecutionsByActionId(actionId, 0, 10);
      
      // Convert date strings to Date objects
      const executionItems: ActionExecutionItem[] = result.actions.map(execution => ({
        ...execution,
        createdAt: execution.createdAt instanceof Date ? execution.createdAt : parseISO(execution.createdAt as string),
        endTime: execution.endTime instanceof Date ? execution.endTime : parseISO(execution.endTime as string)
      }));
      
      setExecutions(executionItems);
      setExecutionsCount(result.totalCount);
    } catch (error) {
      console.error('Error fetching action executions:', error);
      toast.current?.show({ 
        severity: 'error', 
        summary: 'Error', 
        detail: 'Failed to load action executions' 
      });
    } finally {
      setIsLoadingExecutions(false);
    }
  };

  // Handle provider change
  const handleProviderChange = (providerValue: string) => {
    // Update provider state
    setSelectedProvider(providerValue);
    
    // Clear model when provider changes
    setSelectedModel('');
    
    // Form validity will be updated through the useEffect
  };

  // Fetch available tools when the dialog is opened
  useEffect(() => {
    const fetchTools = async () => {
      try {
        setIsLoadingTools(true);
        const response = await integrationsService.getToolProviders();
        
        if (!response || !response.toolProviders) {
          return;
        }
        
        // Create a grouped structure for tools organized by provider
        const groupedTools: { 
          label: string, 
          icon: string, 
          isConnected?: boolean,
          items: { name: string, value: string, isConnected?: boolean }[] 
        }[] = [];
        
        // Process each tool provider - provide unique values based on the app key, not the app.application
        response.toolProviders.forEach(provider => {
          // Only include tools from connected providers
          // Built-in utilities (DateTime, Delay) will be added separately
          const includeProvider = provider.isConnected;
          
          if (includeProvider && provider.applications && Object.keys(provider.applications).length > 0) {
            // Create an array of tools for this provider
            const providerTools = Object.entries(provider.applications)
              .filter(([key, app]) => app && app.name) // Ensure valid app
              .map(([key, app]) => ({
                name: app.name,
                // Use a unique value that combines provider and key to ensure uniqueness
                value: `${provider.name.replace(/\s+/g, '')}_${key}`,
                // Include the isConnected status for filtering later
                isConnected: provider.isConnected
              }));
            
            // Only add the provider if it has tools
            if (providerTools.length > 0) {
              groupedTools.push({
                label: provider.name,
                icon: provider.icon || '', // Use the provider's icon from API
                items: providerTools,
                isConnected: provider.isConnected
              });
            }
          }
        });
        
        // Always add built-in utility tools (DateTime, Delay)
        const utilityTools = [
          { name: 'Date Time', value: 'Utilities_dateTime' },
          { name: 'Delay', value: 'Utilities_delay' }
        ];
        
        // Add utilities group if needed
        if (utilityTools.length > 0) {
          groupedTools.push({
            label: 'Utilities',
            icon: 'pi-wrench',
            isConnected: true, // Utilities are always connected
            items: utilityTools.map(tool => ({
              ...tool,
              isConnected: true // Utilities are always connected
            }))
          });
        }
        
        setToolOptions(groupedTools);
      } catch (error) {
        console.error('Error fetching tools:', error);
        // Only use utilities in fallback options
        const fallbackOptions = [
          {
            label: 'Utilities',
            icon: 'pi-wrench',
            isConnected: true, // Utilities are always connected
            items: [
              { name: 'Date Time', value: 'Utilities_dateTime', isConnected: true },
              { name: 'Delay', value: 'Utilities_delay', isConnected: true }
            ]
          }
        ];
        
        setToolOptions(fallbackOptions);
      } finally {
        setIsLoadingTools(false);
      }
    };
    
    if (visible) {
      fetchTools();
    }
  }, [visible]); // Run when dialog becomes visible
  
  // Add useEffect to handle tools mapping when toolOptions or editAction changes
  useEffect(() => {
    if (!editAction || !toolOptions.length) return;
    
    // Convert from enum values to our custom values
    const toolsArray = editAction.allowedTools || [];
    
    if (toolsArray.length === 0) {
      return;
    }
    
    // Flatten all provider tools
    const allTools = toolOptions.flatMap(provider => provider.items || []);
    
    // Map each tool to the appropriate display value
    const mappedTools = toolsArray.map(tool => {
      if (typeof tool === 'string') {
        const suffix = tool.toLowerCase();
        
        // Try exact matches with provider prefix
        for (const provider of toolOptions) {
          const providerPrefix = provider.label.replace(/\s+/g, '');
          for (const item of provider.items) {
            if (item.value === `${providerPrefix}_${suffix}` || 
                item.value === `${providerPrefix}_${tool}`) {
              return item.value;
            }
          }
        }
        
        // Try to match by application name
        const displayTool = allTools.find(displayItem => {
          const itemValueParts = displayItem.value.split('_');
          const itemKey = itemValueParts[itemValueParts.length - 1]?.toLowerCase();
          
          return itemKey === suffix ||
                 displayItem.name.toLowerCase() === tool.replace(/([A-Z])/g, ' $1').trim().toLowerCase();
        });
        
        if (displayTool) {
          return displayTool.value;
        }
        
        // Special handling for utility tools
        if (tool === 'DateTime' || tool === 'Delay') {
          return `Utilities_${tool.toLowerCase()}`;
        }
      }
      return tool;
    });
    
    setAllowedTools(mappedTools as ToolProviderApplicationType[]);
  }, [toolOptions, editAction]);

  // Reset form when the dialog is opened or editAction changes
  useEffect(() => {
    if (visible) {
      if (editAction) {
        setName(editAction.name || '');
        setDescription(editAction.description || '');
        setIcon(editAction.icon || '');
        setSystemPromptIds(editAction.systemPromptIds || []);
        setUserPromptIds(editAction.userPromptIds || []);
        
        // Handle model configuration from backend format
        if (editAction.actionModelConfiguration) {
          // Map from backend model to UI state
          setSelectedProvider(editAction.actionModelConfiguration.providerType || '');
          setSelectedModel(editAction.actionModelConfiguration.modelName || '');
          
          // Load parameters from metadata or initialize empty object
          if (editAction.actionModelConfiguration.metadata) {
            setParameters(editAction.actionModelConfiguration.metadata as Record<string, string>);
          } else {
            setParameters({});
          }
        }
        
        // Fetch action executions if editing an existing action
        if (editAction.id) {
          fetchActionExecutions(editAction.id);
        }
      } else {
        // Clear all fields for new action
        setName('');
        setDescription('');
        setIcon('');
        setSystemPromptIds([]);
        setUserPromptIds([]);
        setAllowedTools([]);
        setSelectedProvider('');
        setSelectedModel('');
        setParameters({});
        setParameterBeingEdited(null);
        // Clear executions when creating a new action
        setExecutions([]);
        setExecutionsCount(0);
      }
      
      setNameError('');
      setDescriptionError('');
      setIsFormValid(false);
      
      // Form validity will be updated through the dedicated useEffect
    }
  }, [visible, editAction]);

  // Form validation function moved to before useEffect
  
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
    
    // Model validation - if provider is selected, model must also be selected
    if (selectedProvider && !selectedModel) {
      toast.current?.show({
        severity: 'warn',
        summary: 'Missing Model',
        detail: 'If you select a provider, you must also select a model'
      });
      isValid = false;
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
      // Create the model configuration with the current state
      const backendModelConfig: BackendActionModelConfiguration = {
        providerType: selectedProvider,
        modelName: selectedModel,
        streaming: true,
        metadata: parameters
      };
      
      // Convert our custom tool values back to enum values using a more efficient approach
      const toolApplicationTypes = allowedTools.map(tool => {
        if (typeof tool === 'string' && tool.includes('_')) {
          // Custom value format, extract the application type from the key
          const key = tool.split('_').pop() || ''; // Get last part more efficiently
          // Convert key to PascalCase and return it as the application type
          return key.charAt(0).toUpperCase() + key.slice(1);
        }
        // If it's not our custom format or can't be parsed, pass it through
        return tool;
      });
      
      const actionData: UpsertActionModel = {
        name: name.trim(),
        description: description.trim(),
        icon: icon.trim() || undefined,
        systemPromptIds: systemPromptIds,
        userPromptIds: userPromptIds,
        allowedTools: toolApplicationTypes as ToolProviderApplicationType[],
        actionModelConfiguration: backendModelConfig || { 
          providerType: '',
          modelName: '',
          streaming: true,
          metadata: {}
        }
      };
      
            
      if (editAction?.id) {
        await actionService.updateAction(editAction.id, actionData);
        toast.current?.show({
          severity: 'success',
          summary: 'Success',
          detail: 'Action updated successfully'
        });
      } else {
        await actionService.createAction(actionData);
        toast.current?.show({
          severity: 'success',
          summary: 'Success',
          detail: 'Action created successfully'
        });
      }
      
      onSave();
      onHide();
    } catch (error) {
      console.error('Error saving action:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: `Failed to ${editAction?.id ? 'update' : 'create'} action`
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
            <i className={`pi ${editAction?.id ? 'pi-pencil' : 'pi-plus-circle'} header-icon`}></i>
            <span>{editAction?.id ? 'Edit Action' : 'Create New Action'}</span>
          </div>
        }
        visible={visible}
        style={{ width: '80vw', height: '90vh' }}
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
          <TabView>
            <TabPanel header="Basic Information">
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
                    setIsFormValid(checkFormValidity());
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
                <InputTextarea
                  id="description"
                  value={description}
                  onChange={(e) => {
                    setDescription(e.target.value);
                    if (e.target.value.trim()) {
                      setDescriptionError('');
                    }
                    setIsFormValid(checkFormValidity());
                  }}
                  rows={3}
                  className={descriptionError ? 'p-invalid' : ''}
                  placeholder="Brief description of what this action does"
                  disabled={saving}
                />
                {descriptionError && <small className="p-error">{descriptionError}</small>}
              </div>

              <div className="field">
                <label htmlFor="icon" className="field-label">Icon (Optional)</label>
                <InputText
                  id="icon"
                  value={icon}
                  onChange={(e) => setIcon(e.target.value)}
                  placeholder="Icon class or URL (e.g., pi pi-user)"
                  disabled={saving}
                />
                <small className="text-color-secondary">Provide a PrimeReact icon class (pi pi-*) or image URL</small>
              </div>
              
              <div className="field">
                <label htmlFor="allowedTools" className="field-label">Allowed Tools</label>
                <div className="mb-2">
                  <small className="text-color-secondary">Selected: {allowedTools.length} tool{allowedTools.length !== 1 ? 's' : ''}</small>
                </div>
                <MultiSelect
                  id="allowedTools"
                  value={allowedTools}
                  options={toolOptions.filter(group => 
                    // Keep a group if it has at least one item that's connected
                    group.items.some(item => item.isConnected === true)
                  )}
                  onChange={(e) => setAllowedTools(e.value)}
                  optionLabel="name"
                  optionGroupLabel="label"
                  optionGroupChildren="items"
                  optionDisabled={(option) => !option.isConnected}
                  placeholder="Select allowed tools"
                  filter
                  display="chip"
                  disabled={saving || isLoadingTools}
                  className="w-full"
                  loading={isLoadingTools}
                  scrollHeight="350px"
                  itemTemplate={(option) => (
                    <div className="flex align-items-center">
                      <span>{option.name}</span>
                    </div>
                  )}
                  optionGroupTemplate={(option) => (
                    <div className="flex align-items-center p-2 font-bold">
                      {option.icon && option.icon.includes('pi-') && (
                        <i className={`pi ${option.icon} mr-2`} style={{ fontSize: '1rem' }}></i>
                      )}
                      {option.icon && !option.icon.includes('pi-') && option.icon.startsWith('http') && (
                        <img src={option.icon} alt="" className="mr-2" style={{ width: '1rem', height: '1rem' }} />
                      )}
                      {(!option.icon || (!option.icon.includes('pi-') && !option.icon.startsWith('http'))) && 
                        <i className="pi pi-box mr-2" style={{ fontSize: '1rem' }}></i>}
                      <span>{option.label}</span>
                      <span className="ml-auto text-color-secondary">
                        ({option.items.filter((item: any) => item.isConnected).length})
                      </span>
                    </div>
                  )}
                />
                <small className="text-color-secondary">Specify which tools the AI is allowed to use in this action</small>
              </div>
            </TabPanel>

            <TabPanel header="Prompts">
              <div className="field">
                <label htmlFor="systemPrompts" className="field-label">System Prompts</label>
                <MultiSelect
                  id="systemPrompts"
                  value={systemPromptIds}
                  options={systemPrompts.map(p => ({ name: p.name, value: p.id }))}
                  onChange={(e) => setSystemPromptIds(e.value)}
                  optionLabel="name"
                  placeholder="Select system prompts"
                  display="chip"
                  loading={isLoadingPrompts}
                  disabled={saving || isLoadingPrompts}
                  className="w-full"
                  emptyMessage="No system prompts found"
                  emptyFilterMessage="No matching system prompts found"
                />
                <small className="text-color-secondary">System prompts are used to guide the AI's behavior</small>
              </div>

              <div className="field">
                <label htmlFor="userPrompts" className="field-label">Action Prompts</label>
                <MultiSelect
                  id="userPrompts"
                  value={userPromptIds}
                  options={actionPrompts.map(p => ({ name: p.name, value: p.id }))}
                  onChange={(e) => setUserPromptIds(e.value)}
                  optionLabel="name"
                  placeholder="Select action prompts"
                  display="chip"
                  loading={isLoadingPrompts}
                  disabled={saving || isLoadingPrompts}
                  className="w-full"
                  emptyMessage="No action prompts found"
                  emptyFilterMessage="No matching action prompts found"
                />
                <small className="text-color-secondary">Action prompts define the specific actions the AI will perform</small>
              </div>
            </TabPanel>
            

            <TabPanel header="Model Configuration">
              <div className="field">
                <label htmlFor="provider" className="field-label">Provider</label>
                <Dropdown
                  id="provider"
                  value={selectedProvider}
                  options={providers}
                  onChange={(e) => handleProviderChange(e.value)}
                  optionLabel="name"
                  placeholder="Select AI provider"
                  loading={isLoadingModels}
                  disabled={saving || isLoadingModels}
                  className="w-full"
                />
              </div>

              <div className="field">
                <label htmlFor="model" className="field-label">Model</label>
                <Dropdown
                  id="model"
                  value={selectedModel}
                  options={modelsForProvider}
                  onChange={(e) => {
                    setSelectedModel(e.value);
                    // Form validity will be updated through the useEffect
                  }}
                  optionLabel="name"
                  placeholder="Select AI model"
                  loading={isLoadingModels}
                  disabled={saving || isLoadingModels || !selectedProvider}
                  className="w-full"
                />
                <div className="mt-1">
                  <small className="text-color-secondary">
                    Current model: {selectedModel || 'None selected'}
                  </small>
                </div>
              </div>

              <div className="field">
                <label className="field-label mb-2">Model Parameters</label>
                <div className="surface-border border-1 border-round p-3">
                  <div className="grid mb-3">
                    <div className="col-5">
                      <label className="font-medium block mb-1">Parameter Name</label>
                    </div>
                    <div className="col-6">
                      <label className="font-medium block mb-1">Value</label>
                    </div>
                    <div className="col-1"></div>
                  </div>
                  
                  {/* Show existing parameters */}
                  {Object.entries(parameters).map(([key, value], index) => {
                    // Handle special case for the parameter being edited
                    const isBeingEdited = parameterBeingEdited?.originalKey === key;
                    
                    // Render a different input when being edited
                    if (isBeingEdited) {
                      return (
                        <div className="grid mb-2 align-items-center" key={`editing-param-${index}`}>
                          <div className="col-5">
                            <InputText
                              value={parameterBeingEdited.currentKey}
                              onChange={(e) => {
                                setParameterBeingEdited({
                                  ...parameterBeingEdited,
                                  currentKey: e.target.value
                                });
                              }}
                              onBlur={() => {
                                if (parameterBeingEdited.currentKey.trim()) {
                                  // Apply changes to the parameters object
                                  const newParams: Record<string, string> = {};
                                  Object.entries(parameters).forEach(([k, v]) => {
                                    if (k === parameterBeingEdited.originalKey) {
                                      // Use the new key and value
                                      newParams[parameterBeingEdited.currentKey.trim()] = parameterBeingEdited.value;
                                    } else {
                                      newParams[k] = v;
                                    }
                                  });
                                  setParameters(newParams);
                                }
                                setParameterBeingEdited(null);
                                setIsFormValid(checkFormValidity());
                              }}
                              onKeyDown={(e) => {
                                if (e.key === 'Enter') {
                                  (e.target as HTMLInputElement).blur();
                                }
                              }}
                              autoFocus
                              className="w-full"
                              placeholder="parameter"
                              disabled={saving}
                            />
                          </div>
                          <div className="col-6">
                            <InputText
                              value={parameterBeingEdited.value}
                              onChange={(e) => {
                                setParameterBeingEdited({
                                  ...parameterBeingEdited,
                                  value: e.target.value
                                });
                              }}
                              onBlur={() => {
                                if (parameterBeingEdited.currentKey.trim()) {
                                  // Apply changes to the parameters object
                                  const newParams: Record<string, string> = {};
                                  Object.entries(parameters).forEach(([k, v]) => {
                                    if (k === parameterBeingEdited.originalKey) {
                                      // Use the new key and value
                                      newParams[parameterBeingEdited.currentKey.trim()] = parameterBeingEdited.value;
                                    } else {
                                      newParams[k] = v;
                                    }
                                  });
                                  setParameters(newParams);
                                }
                                setParameterBeingEdited(null);
                                setIsFormValid(checkFormValidity());
                              }}
                              onKeyDown={(e) => {
                                if (e.key === 'Enter') {
                                  (e.target as HTMLInputElement).blur();
                                }
                              }}
                              className="w-full"
                              placeholder="value"
                              disabled={saving}
                            />
                          </div>
                          <div className="col-1 text-center">
                            <Button
                              icon="pi pi-trash"
                              className="p-button-text p-button-danger p-button-rounded"
                              onClick={() => {
                                // Create a new object without the deleted key
                                const newParams: Record<string, string> = {};
                                Object.entries(parameters).forEach(([k, v]) => {
                                  if (k !== parameterBeingEdited.originalKey) {
                                    newParams[k] = v;
                                  }
                                });
                                setParameters(newParams);
                                setParameterBeingEdited(null);
                                setIsFormValid(checkFormValidity());
                              }}
                              disabled={saving}
                            />
                          </div>
                        </div>
                      );
                    }
                    
                    // Regular display when not being edited
                    return (
                      <div className="grid mb-2 align-items-center" key={`param-${index}-${key}`}>
                        <div className="col-5">
                          <InputText
                            value={key}
                            onClick={() => {
                              // When clicked, start editing this parameter
                              setParameterBeingEdited({
                                originalKey: key,
                                currentKey: key,
                                value: value
                              });
                            }}
                            readOnly
                            className="w-full cursor-text"
                            placeholder="parameter"
                            disabled={saving}
                          />
                        </div>
                        <div className="col-6">
                          <InputText
                            value={value}
                            onChange={(e) => {
                              const newValue = e.target.value;
                              // Update just the value
                              const newParams = { ...parameters };
                              newParams[key] = newValue;
                              setParameters(newParams);
                              setIsFormValid(checkFormValidity());
                            }}
                            className="w-full"
                            placeholder="value"
                            disabled={saving}
                          />
                        </div>
                        <div className="col-1 text-center">
                          <Button
                            icon="pi pi-trash"
                            className="p-button-text p-button-danger p-button-rounded"
                            onClick={() => {
                              // Create a new object without the deleted key
                              const newParams: Record<string, string> = {};
                              Object.entries(parameters).forEach(([k, v]) => {
                                if (k !== key) {
                                  newParams[k] = v;
                                }
                              });
                              setParameters(newParams);
                              setIsFormValid(checkFormValidity());
                            }}
                            disabled={saving}
                          />
                        </div>
                      </div>
                    );
                  })}
                  
                  {/* Add parameter button */}
                  <Button
                    label="Add Parameter"
                    icon="pi pi-plus"
                    className="p-button-outlined p-button-secondary mt-2"
                    onClick={() => {
                      // Generate a unique key for the new parameter
                      let newKey = '';
                      let counter = 1;
                      while (newKey in parameters || newKey === '') {
                        newKey = `param${counter}`;
                        counter++;
                      }
                      
                      // Add the parameter to the state
                      const newParams = { ...parameters };
                      newParams[newKey] = '';
                      setParameters(newParams);
                      
                      // Immediately enter edit mode for the new parameter
                      setParameterBeingEdited({
                        originalKey: newKey,
                        currentKey: newKey,
                        value: ''
                      });
                    }}
                    disabled={saving}
                  />
                </div>
                <small className="text-color-secondary block mt-2">
                  Common parameters include: temperature (0-2), maxTokens, topP (0-1), presencePenalty (-2 to 2), frequencyPenalty (-2 to 2)
                </small>
              </div>
            </TabPanel>
            
            {/* Only show Execution History tab when editing an existing action with an ID */}
            {editAction?.id && (
              <TabPanel header="Execution History">
                <div className="field">
                  <div className="flex align-items-center justify-content-between mb-3">
                    <h3 className="m-0">Action Execution History</h3>
                    <Button 
                      label="Refresh" 
                      icon="pi pi-refresh" 
                      className="p-button-outlined p-button-sm" 
                      onClick={() => editAction?.id && fetchActionExecutions(editAction.id)}
                      disabled={isLoadingExecutions}
                    />
                  </div>
                  
                  {isLoadingExecutions ? (
                    <div className="mt-3">
                      <Skeleton width="100%" height="2rem" className="mb-2" />
                      <Skeleton width="100%" height="2rem" className="mb-2" />
                      <Skeleton width="100%" height="2rem" className="mb-2" />
                      <Skeleton width="100%" height="2rem" className="mb-2" />
                      <Skeleton width="100%" height="2rem" />
                    </div>
                  ) : (
                    <>
                      {executions.length === 0 ? (
                        <div className="p-3 text-center surface-100 border-round">
                          <p className="text-color-secondary m-0">No execution history found for this action</p>
                        </div>
                      ) : (
                        <DataTable 
                          value={executions}
                          rows={5}
                          paginator={executions.length > 5}
                          rowsPerPageOptions={[5, 10, 25]}
                          emptyMessage="No executions found"
                          className="p-datatable-sm"
                          sortField="createdAt"
                          sortOrder={-1}
                          responsiveLayout="stack"
                          breakpoint="960px"
                          dataKey="id"
                          stripedRows
                          rowClassName={(data) => {
                            if (data.executionStatus === ActionExecutionStatus.Failed) {
                              return 'execution-failed';
                            }
                            return '';
                          }}
                        >
                          <Column 
                            field="executionStatus" 
                            header="Status" 
                            body={statusTemplate} 
                            sortable 
                            headerClassName="w-2 column-status"
                            style={{ width: '15%' }}
                          />
                          <Column 
                            field="createdAt" 
                            header="Time" 
                            body={timestampTemplate}
                            sortable 
                            headerClassName="w-3 column-timestamp"
                            style={{ width: '30%' }}
                          />
                          <Column 
                            field="endTime" 
                            header="End Time" 
                            body={completionTimestampTemplate}
                            sortable 
                            headerClassName="w-3 column-timestamp"
                            style={{ width: '30%' }}
                          />
                          <Column 
                            field="duration" 
                            header="Duration" 
                            body={durationTemplate}
                            headerClassName="w-2 column-duration"
                            style={{ width: '15%' }}
                          />
                        </DataTable>
                      )}
                      
                      {executionsCount > 0 && (
                        <div className="mt-3 text-center">
                          <small className="text-color-secondary">
                            Showing {Math.min(executions.length, 5)} of {executionsCount} total executions
                          </small>
                        </div>
                      )}
                    </>
                  )}
                </div>
              </TabPanel>
            )}
          </TabView>
        </div>
      </Dialog>
    </>
  );
};

export { ActionDialog };
export default ActionDialog;