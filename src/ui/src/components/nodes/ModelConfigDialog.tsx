import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { TabView, TabPanel } from 'primereact/tabview';
import { Dropdown } from 'primereact/dropdown';
import { MultiSelect } from 'primereact/multiselect';
import { InputText } from 'primereact/inputtext';
import { Toast } from 'primereact/toast';
import { Checkbox } from 'primereact/checkbox';
import { Message } from 'primereact/message';
import { ModelNodeData } from '../../types/nodes';
import { modelsService } from '../../services/api/modelsService';
import { promptService } from '../../services/api/promptService';
import { integrationsService } from '../../services/api/integrationsService';

interface ModelConfigDialogProps {
  visible: boolean;
  onHide: () => void;
  nodeData: ModelNodeData;
  onUpdate: (data: ModelNodeData) => void;
}

const ModelConfigDialog: React.FC<ModelConfigDialogProps> = ({
  visible,
  onHide,
  nodeData,
  onUpdate
}) => {
  // State for model configuration
  const [providerType, setProviderType] = useState<string>('');
  const [modelName, setModelName] = useState<string>('');
  const [systemPromptIds, setSystemPromptIds] = useState<string[]>([]);
  const [allowedTools, setAllowedTools] = useState<string[]>([]);
  const [modelParameters, setModelParameters] = useState<Record<string, string>>({});
  const [streaming, setStreaming] = useState<boolean>(true);
  const [dynamicTools, setDynamicTools] = useState<boolean>(false);
  
  // State for parameter being edited
  const [parameterBeingEdited, setParameterBeingEdited] = useState<{
    originalKey: string;
    currentKey: string;
    value: string;
  } | null>(null);
  
  // State for loading resources
  const [isLoadingProviders, setIsLoadingProviders] = useState(false);
  const [isLoadingModels, setIsLoadingModels] = useState(false);
  const [isLoadingPrompts, setIsLoadingPrompts] = useState(false);
  const [isLoadingTools, setIsLoadingTools] = useState(false);
  
  // State for available options
  const [providers, setProviders] = useState<{ name: string; value: string }[]>([]);
  const [modelsForProvider, setModelsForProvider] = useState<{ name: string; value: string }[]>([]);
  const [systemPrompts, setSystemPrompts] = useState<any[]>([]);
  const [toolOptions, setToolOptions] = useState<any[]>([]);
  
  const toast = useRef<Toast>(null);
  
  // Initialize form state from nodeData
  useEffect(() => {
    if (visible) {
      setProviderType(nodeData.providerType || '');
      setModelName(nodeData.modelName || '');
      setSystemPromptIds(nodeData.systemPromptIds || []);
      
      // Initialize model parameters and settings
      setModelParameters(nodeData.modelParameters || {});
      setStreaming(nodeData.streaming !== undefined ? nodeData.streaming : true);
      setDynamicTools(nodeData.dynamicTools || false);
      
      // Load resources
      fetchModels();
      fetchPrompts();
      // Wait to set allowedTools after fetchTools() so we can match them properly
      fetchTools(nodeData.allowedTools || []);
    }
  }, [visible, nodeData]);
  
  // Fetch available models
  const fetchModels = async () => {
    try {
      setIsLoadingProviders(true);
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
      
      // If provider is already selected, load models for that provider
      if (nodeData.providerType) {
        const providerModels = response.allowedModels[nodeData.providerType] || [];
        const mappedModels = providerModels.map(model => ({
          name: model,
          value: model
        }));
        
        setModelsForProvider(mappedModels);
      }
    } catch (error) {
      console.error('Error fetching models:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load AI providers and models'
      });
    } finally {
      setIsLoadingProviders(false);
    }
  };
  
  // Fetch available prompts
  const fetchPrompts = async () => {
    try {
      setIsLoadingPrompts(true);
      const response = await promptService.getPrompts(0, 500);
      
      const systemPromptsArray = response.items || response.prompts || [];
      setSystemPrompts(systemPromptsArray);
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
  
  // Fetch available tools
  const fetchTools = async (initialAllowedTools: string[] = []) => {
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
      
      // Process each tool provider
      response.toolProviders.forEach(provider => {
        const includeProvider = provider.isConnected;
        
        if (includeProvider && provider.applications && Object.keys(provider.applications).length > 0) {
          // Create an array of tools for this provider
          const providerTools = Object.entries(provider.applications)
            .filter(([key, app]) => app && app.name)
            .map(([key, app]) => ({
              name: app.name,
              value: key,
              isConnected: provider.isConnected
            }));
          
          // Only add the provider if it has tools
          if (providerTools.length > 0) {
            groupedTools.push({
              label: provider.name,
              icon: provider.icon || '',
              items: providerTools,
              isConnected: provider.isConnected
            });
          }
        }
      });
      
      // Always add built-in utility tools (DateTime, Delay)
      const utilityTools = [
        { name: 'Date Time', value: 'DateTime' },
        { name: 'Delay', value: 'Delay' }
      ];
      
      // Add utilities group if needed
      if (utilityTools.length > 0) {
        groupedTools.push({
          label: 'Utilities',
          icon: 'pi-wrench',
          isConnected: true,
          items: utilityTools.map(tool => ({
            ...tool,
            isConnected: true
          }))
        });
      }
      
      // Get all available tool values for matching
      const allToolValues = groupedTools.flatMap(group => 
        group.items.map(item => item.value)
      );
      
      // Set the tool options first so the dropdown has values
      setToolOptions(groupedTools);
      
      if (initialAllowedTools && initialAllowedTools.length > 0) {
        // Find the matching tool values using case-insensitive comparison
        const matchedTools = allToolValues.filter(toolValue => 
          initialAllowedTools.some(allowedTool => 
            allowedTool.toLowerCase() === toolValue.toLowerCase()
          )
        );
        
        // Update the allowedTools with the matched values
        setAllowedTools(matchedTools);
      }
    } catch (error) {
      console.error('Error fetching tools:', error);
      // Only use utilities in fallback options
      const fallbackOptions = [
        {
          label: 'Utilities',
          icon: 'pi-wrench',
          isConnected: true,
          items: [
            { name: 'Date Time', value: 'DateTime', isConnected: true },
            { name: 'Delay', value: 'Delay', isConnected: true }
          ]
        }
      ];
      
      setToolOptions(fallbackOptions);
      
      // Handle the initialAllowedTools in fallback case
      if (initialAllowedTools && initialAllowedTools.length > 0) {
        // Filter to only include utility tools in our fallback options
        const utilitiesTools = initialAllowedTools.filter(tool => 
          ['datetime', 'delay'].includes(tool.toLowerCase())
        );
        setAllowedTools(utilitiesTools);
      }
    } finally {
      setIsLoadingTools(false);
    }
  };
  
  // Handler for provider change
  const handleProviderChange = (provider: string) => {
    setProviderType(provider);
    setModelName(''); // Reset model when provider changes
    
    if (provider) {
      loadModelsForProvider(provider);
    } else {
      setModelsForProvider([]);
    }
  };
  
  // Load models for selected provider
  const loadModelsForProvider = async (provider: string) => {
    try {
      setIsLoadingModels(true);
      const response = await modelsService.getModels();
      
      if (!response || !response.allowedModels) {
        return;
      }
      
      const providerModels = response.allowedModels[provider] || [];
      const mappedModels = providerModels.map(model => ({
        name: model,
        value: model
      }));
      
      setModelsForProvider(mappedModels);
    } catch (error) {
      console.error('Error fetching models for provider:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load models for provider'
      });
    } finally {
      setIsLoadingModels(false);
    }
  };
  
  // Save the changes to the node
  const handleSave = () => {
    // Create a proper update that keeps all existing properties and only modifies what we changed
    const updatedNodeData: ModelNodeData = {
      ...nodeData,
      providerType,
      modelName,
      systemPromptIds,
      // Only update allowedTools if not in dynamic mode
      allowedTools: dynamicTools ? [] : allowedTools,
      // Use the current dynamicTools state
      dynamicTools,
      modelParameters,
      streaming
    };
    
    onUpdate(updatedNodeData);
    onHide();
  };
  
  // Check if form is valid
  const isFormValid = () => {
    // Require provider and model if any model configuration is being used
    return !providerType || (providerType && modelName);
  };
  
  // Render dialog footer
  const renderFooter = () => (
    <div>
      <Button
        label="Cancel"
        icon="pi pi-times"
        className="p-button-text"
        onClick={onHide}
      />
      <Button
        label="Save"
        icon="pi pi-check"
        className="p-button-primary"
        onClick={handleSave}
        disabled={!isFormValid()}
      />
    </div>
  );
  
  return (
    <>
      <Toast ref={toast} position="top-right" />
      
      <Dialog
        header={`Configure ${nodeData.label} Node`}
        visible={visible}
        style={{ width: '80vw', maxWidth: '900px' }}
        modal
        footer={renderFooter}
        onHide={onHide}
        maximizable
        draggable={false}
        resizable={false}
        className="fullscreen-dialog" /* Add a custom class for styling */
      >
        <TabView>
          <TabPanel header="Model Configuration">
            <div className="grid">
              <div className="col-12 md:col-6 field">
                <label htmlFor="provider" className="block font-medium mb-2">Provider</label>
                <Dropdown
                  id="provider"
                  value={providerType}
                  options={providers}
                  onChange={(e) => handleProviderChange(e.value)}
                  optionLabel="name"
                  placeholder="Select AI provider"
                  loading={isLoadingProviders}
                  className="w-full"
                />
                {!providerType && (
                  <small className="text-color-secondary block mt-1">
                    No provider selected. Node will use the default model.
                  </small>
                )}
              </div>
              
              <div className="col-12 md:col-6 field">
                <label htmlFor="model" className="block font-medium mb-2">Model</label>
                <Dropdown
                  id="model"
                  value={modelName}
                  options={modelsForProvider}
                  onChange={(e) => setModelName(e.value)}
                  optionLabel="name"
                  placeholder="Select AI model"
                  loading={isLoadingModels}
                  disabled={!providerType || isLoadingModels}
                  className="w-full"
                />
                <div className="mt-1">
                  <small className="text-color-secondary">
                    Current model: {modelName || 'None selected'}
                  </small>
                </div>
              </div>
              
              <div className="col-12 field">
                <div className="flex align-items-center mb-2">
                  <label htmlFor="streaming" className="font-medium mr-2">Streaming</label>
                  <Checkbox
                    id="streaming"
                    checked={streaming}
                    onChange={(e) => setStreaming(e.checked || false)}
                  />
                </div>
                <small className="text-color-secondary block">
                  Enable streaming for real-time responses. Disable if you need to process the entire response at once.
                </small>
              </div>
            </div>
            
            <div className="field mt-4">
              <label htmlFor="systemPrompts" className="block font-medium mb-2">System Prompts</label>
              <MultiSelect
                id="systemPrompts"
                value={systemPromptIds}
                options={systemPrompts.map(p => ({ name: p.name, value: p.id }))}
                onChange={(e) => setSystemPromptIds(e.value)}
                optionLabel="name"
                placeholder="Select system prompts"
                display="chip"
                loading={isLoadingPrompts}
                className="w-full"
                emptyMessage="No system prompts found"
                emptyFilterMessage="No matching system prompts found"
              />
              <small className="text-color-secondary block mt-1">
                System prompts are used to guide the AI's behavior and provide context.
              </small>
            </div>
            
            <div className="field mt-4">
              <div className="flex align-items-center mb-2">
                <Checkbox
                  id="dynamicTools"
                  checked={dynamicTools}
                  onChange={(e) => {
                    // Update the local state variable
                    setDynamicTools(e.checked || false);
                    
                    // If enabling dynamic tools, clear any existing tool selection in local state
                    if (e.checked && allowedTools.length > 0) {
                      setAllowedTools([]);
                    }
                  }}
                />
                <label htmlFor="dynamicTools" className="ml-2 cursor-pointer">
                  Enable Dynamic Tools
                </label>
                <small className="ml-2 text-color-secondary">
                  Allow the model to choose which tools to use dynamically
                </small>
              </div>
              
              <div className="field">
                <label htmlFor="allowedTools" className="block font-medium mb-2">Allowed Tools</label>
                <div className="mb-2">
                  <small className="text-color-secondary">Selected: {allowedTools.length} tool{allowedTools.length !== 1 ? 's' : ''}</small>
                </div>
                <MultiSelect
                  id="allowedTools"
                  value={allowedTools}
                  options={toolOptions.filter(group => 
                    // Keep a group if it has at least one item that's connected
                    group.items.some((item: { isConnected?: boolean }) => item.isConnected === true)
                  )}
                  onChange={(e) => setAllowedTools(e.value)}
                  optionLabel="name"
                  optionGroupLabel="label"
                  optionGroupChildren="items"
                  optionDisabled={(option) => !option.isConnected}
                  placeholder="Select allowed tools"
                  filter
                  display="chip"
                  disabled={isLoadingTools || dynamicTools}
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
                <small className="text-color-secondary block mt-1">
                  {dynamicTools 
                    ? "Tool selection is disabled because dynamic tools mode is enabled" 
                    : "Specify which tools the AI is allowed to use in this model node."}
                </small>
              </div>
            </div>
          </TabPanel>
          
          <TabPanel header="Advanced Parameters">
            <div className="field">
              <label className="block font-medium mb-2">Model Parameters</label>
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
                {Object.entries(modelParameters).map(([key, value], index) => {
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
                                Object.entries(modelParameters).forEach(([k, v]) => {
                                  if (k === parameterBeingEdited.originalKey) {
                                    // Use the new key and value
                                    newParams[parameterBeingEdited.currentKey.trim()] = parameterBeingEdited.value;
                                  } else {
                                    newParams[k] = v;
                                  }
                                });
                                setModelParameters(newParams);
                              }
                              setParameterBeingEdited(null);
                            }}
                            onKeyDown={(e) => {
                              if (e.key === 'Enter') {
                                (e.target as HTMLInputElement).blur();
                              }
                            }}
                            autoFocus
                            className="w-full"
                            placeholder="parameter"
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
                                Object.entries(modelParameters).forEach(([k, v]) => {
                                  if (k === parameterBeingEdited.originalKey) {
                                    // Use the new key and value
                                    newParams[parameterBeingEdited.currentKey.trim()] = parameterBeingEdited.value;
                                  } else {
                                    newParams[k] = v;
                                  }
                                });
                                setModelParameters(newParams);
                              }
                              setParameterBeingEdited(null);
                            }}
                            onKeyDown={(e) => {
                              if (e.key === 'Enter') {
                                (e.target as HTMLInputElement).blur();
                              }
                            }}
                            className="w-full"
                            placeholder="value"
                          />
                        </div>
                        <div className="col-1 text-center">
                          <Button
                            icon="pi pi-trash"
                            className="p-button-text p-button-danger p-button-rounded"
                            onClick={() => {
                              // Create a new object without the deleted key
                              const newParams: Record<string, string> = {};
                              Object.entries(modelParameters).forEach(([k, v]) => {
                                if (k !== parameterBeingEdited.originalKey) {
                                  newParams[k] = v;
                                }
                              });
                              setModelParameters(newParams);
                              setParameterBeingEdited(null);
                            }}
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
                        />
                      </div>
                      <div className="col-6">
                        <InputText
                          value={value}
                          onChange={(e) => {
                            const newValue = e.target.value;
                            // Update just the value
                            const newParams = { ...modelParameters };
                            newParams[key] = newValue;
                            setModelParameters(newParams);
                          }}
                          className="w-full"
                          placeholder="value"
                        />
                      </div>
                      <div className="col-1 text-center">
                        <Button
                          icon="pi pi-trash"
                          className="p-button-text p-button-danger p-button-rounded"
                          onClick={() => {
                            // Create a new object without the deleted key
                            const newParams: Record<string, string> = {};
                            Object.entries(modelParameters).forEach(([k, v]) => {
                              if (k !== key) {
                                newParams[k] = v;
                              }
                            });
                            setModelParameters(newParams);
                          }}
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
                    while (newKey in modelParameters || newKey === '') {
                      newKey = `param${counter}`;
                      counter++;
                    }
                    
                    // Add the parameter to the state
                    const newParams = { ...modelParameters };
                    newParams[newKey] = '';
                    setModelParameters(newParams);
                    
                    // Immediately enter edit mode for the new parameter
                    setParameterBeingEdited({
                      originalKey: newKey,
                      currentKey: newKey,
                      value: ''
                    });
                  }}
                />
              </div>
              <small className="text-color-secondary block mt-2">
                Common parameters include: temperature (0-2), maxTokens, topP (0-1), presencePenalty (-2 to 2), frequencyPenalty (-2 to 2)
              </small>
              
              <div className="mt-3">
                <Message
                  severity="info"
                  text="These parameters override the default model configuration. Leave empty to use defaults."
                />
              </div>
            </div>
          </TabPanel>
        </TabView>
      </Dialog>
    </>
  );
};

export default ModelConfigDialog;