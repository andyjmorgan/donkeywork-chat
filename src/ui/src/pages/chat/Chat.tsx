import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Dropdown } from 'primereact/dropdown';
import { Menubar } from 'primereact/menubar';
import { Button } from 'primereact/button';
import { Toast } from 'primereact/toast';
import { ProgressSpinner } from 'primereact/progressspinner';

import ChatComponent from '../../features/chat/components/ChatComponent';
import PromptDialog from '../../features/prompts/components/PromptDialog';
import { promptService } from '../../services/api/promptService';
import { conversationService } from '../../services/api/conversationService';
import { modelsService } from '../../services/api/modelsService';
import { UpsertPromptModel } from '../../models/api/Prompt/UpsertPromptModel';
import { Provider, Model, Prompt } from '../../models/ui/chat/providers/ProviderTypes';
import { MessageOwner } from '../../models/api/Conversation/GetConversationModel';
import { ChatMessage } from '../../models/api/chat/ChatMessage';

const Chat: React.FC = () => {
  // Initialize hooks first
  const navigate = useNavigate();
  const { conversationId: urlPathId } = useParams<{ conversationId?: string }>();
  
  // Function to extract the query parameter
  const getConversationIdFromQuery = (): string | null => {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
  };
  
  // Combine both sources - prefer path parameter if available
  const conversationId = urlPathId || getConversationIdFromQuery();
  
  // If we have a URL path ID, redirect to the query parameter format
  useEffect(() => {
    if (urlPathId) {
      navigate(`/chat?id=${urlPathId}`, { replace: true });
    }
  }, [urlPathId, navigate]);
  
  // Added effect to make sure conversationMessages is cleared when there's no ID in URL
  useEffect(() => {
    const hasIdInURL = new URLSearchParams(window.location.search).has('id');
    if (!hasIdInURL && !urlPathId) {
      console.log("📋 No ID in URL - ensuring conversation messages are cleared");
      setConversationMessages([]);
    }
  }, [urlPathId]);
  const toast = useRef<Toast>(null);
  // Create a map of providers to their models
  const modelsMap: Record<string, Model[]> = {};

  const [selectedProvider, setSelectedProvider] = useState<Provider | null>(null);
  const [selectedModel, setSelectedModel] = useState<Model | null>(null);
  const [selectedPrompt, setSelectedPrompt] = useState<Prompt | null>(null);
  const [prompts, setPrompts] = useState<Prompt[]>([]);
  const [promptDialogVisible, setPromptDialogVisible] = useState(false);
  const [editingPrompt, setEditingPrompt] = useState<UpsertPromptModel & { id?: string }>();
  const [isLoading, setIsLoading] = useState(false);
  const [modelsLoading, setModelsLoading] = useState(false);
  
  const [conversationLoading, setConversationLoading] = useState(false);
  const [conversationMessages, setConversationMessages] = useState<ChatMessage[]>([]);
  const [conversationLoadError, setConversationLoadError] = useState<string | null>(null);
  
  const [providers, setProviders] = useState<Provider[]>([]);
  const [providerModelsMap, setProviderModelsMap] = useState<Record<string, Model[]>>({});
  
  // Add states to force ChatComponent re-renders when needed
  const [chatKey, setChatKey] = useState<string>(Date.now().toString());
  const [showChat, setShowChat] = useState<boolean>(true);
  
  const fetchModels = async () => {
    try {
      setModelsLoading(true);
      
      const response = await modelsService.getModels();

      if (!response || !response.allowedModels) {
        return;
      }
      
      // Extract providers from the response
      const providersFromAPI: Provider[] = Object.keys(response.allowedModels).map(key => ({
        name: key, // Using the key as both name and value
        value: key
      }));
      
      Object.entries(response.allowedModels).forEach(([provider, modelIds]) => {
        modelsMap[provider] = modelIds.map(modelId => ({
          name: modelId, // Using model ID as display name for simplicity
          value: modelId,
          provider: provider
        }));
      });
      
      setProviders(providersFromAPI);
      setProviderModelsMap(modelsMap);
      
      // Set default provider and model if available in the response
      if (response.defaultModel && response.defaultModel.key && response.defaultModel.value) {
        // Find the provider
        const defaultProviderKey = response.defaultModel.key;
        const defaultProvider = providersFromAPI.find(p => p.value === defaultProviderKey);
        
        if (defaultProvider) {
          setSelectedProvider(defaultProvider);
          
          // Find the model for this provider
          const defaultModelValue = response.defaultModel.value;
          const defaultModel = modelsMap[defaultProviderKey]?.find(m => m.value === defaultModelValue);
          
          if (defaultModel) {
            setSelectedModel(defaultModel);
          }
        }
      }
      
    } catch (error) {
      // Set fallback data in case of error
      const fallbackProviders = [
        { name: 'OpenAI', value: 'OpenAI' },
        { name: 'Anthropic', value: 'Anthropic' }
      ];
      
      const fallbackModelsMap = {
        'OpenAI': [
          { name: 'GPT-4', value: 'gpt-4', provider: 'OpenAI' },
          { name: 'GPT-3.5', value: 'gpt-3.5-turbo', provider: 'OpenAI' }
        ],
        'Anthropic': [
          { name: 'Claude 3 Opus', value: 'claude-3-opus', provider: 'Anthropic' },
          { name: 'Claude 3 Sonnet', value: 'claude-3-sonnet', provider: 'Anthropic' }
        ]
      };
      
      setProviders(fallbackProviders);
      setProviderModelsMap(fallbackModelsMap);
    } finally {
      setModelsLoading(false);
    }
  };
  
  const fetchPrompts = async () => {
    try {
      setIsLoading(true);
      const response = await promptService.getPrompts(0, 100);
      
      const apiPrompts: Prompt[] = (response.prompts || response.items || []).map((p) => {
        // Ensure content is always in the right format
        const contentArray = Array.isArray(p.content) ? p.content : [p.content];
        
        return {
          id: p.id,
          name: p.name, // Use name instead of title
          // Store first content item as string for use in chat UI
          value: contentArray[0] || '',
          // Store the full content array for editing
          fullContent: contentArray,
          description: p.description
        };
      });
      
      setPrompts(apiPrompts);
      
    } catch (error) {
      console.error('Error fetching prompts:', error);
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load prompts'
      });
    } finally {
      setIsLoading(false);
    }
  };
  
  const fetchConversation = async (id: string) => {
    const hasIdInUrl = window.location.search.includes(`id=${id}`);
    
    if (!hasIdInUrl) {
      setConversationMessages([]);
      return;
    }
    
    setConversationLoading(true);
    setConversationLoadError(null);
    
    try {
      const conversation = await conversationService.getConversation(id);
      
      // Create a map to track which IDs we've seen to ensure uniqueness
      const seenIds = new Map<string, number>();
      
      const chatMessages: ChatMessage[] = conversation.messages.map(msg => {
        // Start with the original ID
        let uniqueId = msg.id;
        
        // If we've seen this ID before, make it unique by appending an index
        if (seenIds.has(msg.id)) {
          const count = seenIds.get(msg.id)! + 1;
          seenIds.set(msg.id, count);
          uniqueId = `${msg.id}-${count}`;
        } else {
          seenIds.set(msg.id, 0);
        }
        
        return {
          id: uniqueId, // Use the potentially modified unique ID
          role: msg.owner === MessageOwner.USER ? 'user' : 'assistant',
          content: msg.content,
          timestamp: new Date(msg.timestamp)
        };
      });
      
      chatMessages.sort((a, b) => (a.timestamp?.getTime() || 0) - (b.timestamp?.getTime() || 0));
      setConversationMessages(chatMessages);
      
    } catch (error) {
      console.error('Error fetching conversation:', error);
      
      let errorMessage = 'Failed to load conversation';
      if (error instanceof Error) {
        errorMessage = error.message;
      }
      
      setConversationLoadError(errorMessage);
      
      toast.current?.show({
        severity: 'error',
        summary: 'Error',
        detail: errorMessage,
        life: 5000
      });
      
      if (errorMessage.includes('not found')) {
        navigate('/');
      }
    } finally {
      setConversationLoading(false);
    }
  };

  // Function to handle URL changes via popstate events (back/forward navigation)
  useEffect(() => {
    const handleUrlChange = () => {
      const queryId = getConversationIdFromQuery();
      // If URL changes and there's a query ID, handle it
      if (queryId && queryId !== conversationId) {
        fetchConversation(queryId);
      }
    };
    
    // Listen for popstate events (browser back/forward)
    window.addEventListener('popstate', handleUrlChange);
    
    return () => {
      window.removeEventListener('popstate', handleUrlChange);
    };
  }, [conversationId]);
  
  // Main effect for initial load only
  useEffect(() => {
    fetchPrompts();
    fetchModels();
    
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  
  useEffect(() => {
    if (!conversationId) {
      return;
    }
    
    const urlHasId = window.location.search.includes(`id=${conversationId}`);
    
    if (urlHasId) {
      fetchConversation(conversationId);
    }
  }, [conversationId]);
  
  const handleProviderChange = (e: any) => {
    // Handle clearing the provider
    if (e.value === null || e.value === undefined) {
      setSelectedProvider(null);
      setSelectedModel(null);
      return;
    }
    
    let providerValue = '';
    let providerName = '';
    
    // Handle different event formats
    if (typeof e === 'string') {
      providerValue = e;
      providerName = e;
    } 
    else if (e && typeof e.value === 'string') {
      providerValue = e.value;
      
      // Try to find the matching provider to get the proper name
      const matchedProvider = providers.find(p => p.value === providerValue);
      providerName = matchedProvider ? matchedProvider.name : providerValue;
    }
    else if (e && e.value && typeof e.value === 'object') {
      providerValue = e.value.value;
      providerName = e.value.name;
    }
    
    // Set the selected provider with both name and value
    setSelectedProvider({ name: providerName, value: providerValue });
    
    // Log the models for this provider
    console.log(`Models for provider '${providerValue}':`, providerModelsMap[providerValue]);
    
    // Clear selected model
    setSelectedModel(null);
  };
  
  const handleModelChange = (e: any) => {
    // Check if clearing the model
    if (e === null || e.value === null || e.value === undefined) {
      console.log("Clearing model selection");
      setSelectedModel(null);
      return;
    }
    
    // Handle normal selection
    const modelValue = e.value;
    const modelOption = getFilteredModels().find(m => m.value === modelValue);
    if (modelOption) {
      console.log("Setting model to:", modelOption);
      setSelectedModel(modelOption);
    }
  };
  
  const handlePromptChange = (e: { value: string | null }) => {
    if (e.value === null) {
      setSelectedPrompt(null);
      return;
    }
    
    const matchedPrompt = prompts.find(p => p.id === e.value);
    
    if (matchedPrompt) {
      setSelectedPrompt(matchedPrompt);
    } else {
      setSelectedPrompt(null);
    }
  };
  
  const handlePromptSave = () => {
    fetchPrompts();
  };
  
  const openPromptDialog = () => {
    if (selectedPrompt) {
      const promptToEdit = {
        id: selectedPrompt.id,
        name: selectedPrompt.name,
        description: selectedPrompt.description || '',
        // Use the full content array if available, otherwise fallback to wrapping the value in an array
        content: selectedPrompt.fullContent || [selectedPrompt.value]
      };
      
      setEditingPrompt(promptToEdit);
      setPromptDialogVisible(true);
    } else {
      toast.current?.show({
        severity: 'info',
        summary: 'Info',
        detail: 'Please select a prompt to edit'
      });
    }
  };
  
  const getFilteredModels = () => {
    if (!selectedProvider || !providerModelsMap[selectedProvider.value]) {
      return [];
    }
    return providerModelsMap[selectedProvider.value];
  };
  
  const getProviderIcon = (providerName: string): string => {
    // Normalize provider name for comparison
    const normalizedName = providerName.toLowerCase();
    
    // Return appropriate image path based on provider name
    if (normalizedName.includes('openai')) {
      return '/images/providers/openai.png';
    } else if (normalizedName.includes('anthropic')) {
      return '/images/providers/anthropic.png';
    } else if (normalizedName.includes('gemini')) {
      return '/images/providers/gemini.png';
    } else if (normalizedName.includes('mistral')) {
      return '/images/providers/mistral.png';
    }
    
    // Default icon
    return '/images/providers/default.png';
  };
  
  const providerItemTemplate = (option: Provider) => {
    return (
      <div className="flex align-items-center gap-2" style={{ padding: '4px 0' }}>
        <span style={{ fontWeight: 'medium', fontSize: '0.875rem' }}>{option.name}</span>
      </div>
    );
  };
  
  const providerValueTemplate = (option: Provider, props: any) => {
    if (option) {
      return <span>{option.name}</span>;
    }
    return <span>{props.placeholder}</span>;
  };
  
  // Removed model templates as they're no longer needed
  
  const promptItemTemplate = (option: Prompt) => {
    // Function to truncate text with ellipsis
    const truncateText = (text: string, maxLength: number): string => {
      if (!text || text.length <= maxLength) return text;
      return text.substring(0, maxLength) + '...';
    };
    
    return (
      <div style={{ padding: '4px 0' }}>
        <div style={{ fontWeight: 'bold', fontSize: '0.875rem' }}>{option.name}</div>
        {option.description && (
          <div style={{ 
            fontSize: '0.75rem', 
            marginTop: '2px', 
            color: 'var(--text-color-secondary)',
            whiteSpace: 'nowrap',
            overflow: 'hidden',
            textOverflow: 'ellipsis'
          }}
          title={option.description} // Show full description on hover
          >
            {truncateText(option.description, 30)}
          </div>
        )}
      </div>
    );
  };
  
  if (conversationId && conversationLoading) {
    return (
      <div className="conversation-loading">
        <ProgressSpinner strokeWidth="4" style={{ width: '50px', height: '50px' }} />
        <p>Loading conversation...</p>
      </div>
    );
  }
  return (
    <div className="h-full" style={{ position: 'relative', display: 'flex', flexDirection: 'column' }}>
      <Toast ref={toast} position="top-right" />
      
      <PromptDialog
        visible={promptDialogVisible}
        onHide={() => {
          setPromptDialogVisible(false);
          setTimeout(() => {
            if (document.activeElement instanceof HTMLElement) {
              document.activeElement.blur();
            }
            document.body.focus();
            const editButtons = document.querySelectorAll('.p-button-rounded.p-button-text');
            editButtons.forEach(btn => {
              if (btn instanceof HTMLElement) {
                btn.blur();
                btn.classList.remove('p-focus');
              }
            });
          }, 50);
        }}
        onSave={() => {
          handlePromptSave();
          setTimeout(() => {
            if (document.activeElement instanceof HTMLElement) {
              document.activeElement.blur();
            }
            document.body.focus();
          }, 50);
        }}
        editPrompt={editingPrompt}
      />
      
      <Menubar
        className="chat-settings-menubar"
        model={[]} 
        start={
          <div className="menu-container">
            <div className="dropdown-group provider-group">
              {selectedProvider && (
                <div className="provider-icon-container">
                  <img 
                    src={getProviderIcon(selectedProvider.value)} 
                    alt={`${selectedProvider.name} icon`}
                    style={{ 
                      width: '18px', 
                      height: '18px', 
                      objectFit: 'contain'
                    }}
                  />
                </div>
              )}
              <Dropdown
                value={selectedProvider ? selectedProvider.value : null}
                options={providers}
                onChange={handleProviderChange}
                optionLabel="name"
                placeholder="Select Provider"
                className="p-inputtext-sm provider-dropdown"
                panelStyle={{ fontSize: '0.875rem' }}
                loading={modelsLoading}
                showClear={!!selectedProvider}
                itemTemplate={providerItemTemplate}
                valueTemplate={providerValueTemplate}
              />
            </div>
            
            <div className="dropdown-group model-group">
              <Dropdown
                value={selectedModel ? selectedModel.value : null}
                options={getFilteredModels()}
                onChange={handleModelChange}
                optionLabel="name"
                placeholder="Select Model"
                className="p-inputtext-sm model-dropdown"
                panelStyle={{ fontSize: '0.875rem' }}
                disabled={!selectedProvider}
                showClear={!!selectedModel}
              />
            </div>
            
            <div className="menu-separator desktop-only" />
            
            <div className="dropdown-group prompt-group">
              <Dropdown
                value={selectedPrompt?.id || null}
                options={
                  prompts.length > 0 
                    ? [
                        ...prompts, 
                        { id: 'add-prompt', name: 'Add Prompt', value: 'add-prompt' }
                      ]
                    : [{ id: 'add-prompt', name: 'Add Prompt', value: 'add-prompt' }]
                }
                onChange={(e) => {
                  if (e.value === 'add-prompt') {
                    // Reset selected prompt
                    setSelectedPrompt(null);
                    // Open a fresh prompt dialog
                    setEditingPrompt({
                      name: '',
                      description: '',
                      content: ['']
                    });
                    setPromptDialogVisible(true);
                  } else {
                    handlePromptChange(e);
                  }
                }}
                optionLabel="name"
                optionValue="id"
                className="p-inputtext-sm prompt-dropdown"
                style={{ minWidth: '12rem' }}
                panelStyle={{ 
                  fontSize: '0.875rem',
                  maxHeight: '400px', /* Increased height from default */
                  overflow: 'auto'
                }}
                itemTemplate={(option) => {
                  if (option.id === 'add-prompt') {
                    return (
                      <div className="flex align-items-center" style={{ 
                        padding: '2px 4px',
                        cursor: 'pointer',
                        borderRadius: '4px',
                        transition: 'background-color 0.2s',
                        margin: '0'
                      }}>
                        <i className="pi pi-plus mr-2" style={{ fontSize: '0.875rem', color: 'var(--primary-color)' }}></i>
                        <span style={{ fontWeight: 'medium', fontSize: '0.875rem' }}>Add Prompt</span>
                      </div>
                    );
                  }
                  return promptItemTemplate(option);
                }}
                loading={isLoading}
                placeholder="Select Prompt"
                emptyMessage="No prompts available"
                showClear={!!selectedPrompt}
              />
              
              <Button
                icon="pi pi-pencil"
                className="p-button-rounded p-button-text p-button-sm edit-prompt-button"
                tooltip="Edit Selected Prompt"
                tooltipOptions={{ position: 'top' }}
                onClick={(e) => {
                  openPromptDialog();
                  if (e.currentTarget) {
                    e.currentTarget.blur();
                  }
                }}
                disabled={!selectedPrompt}
                tabIndex={-1}
                style={{ 
                  width: '2rem', 
                  height: '2rem',
                  outline: 'none'
                }}
              />
            </div>
          </div>
        }
        end={
          <Button
            label="New Chat"
            icon="pi pi-plus"
            className="p-button-sm"
            onClick={() => {
              // The simplest solution: reload the page
              window.location.href = '/chat';
            }}
            tooltip="Start a new conversation"
            tooltipOptions={{ position: 'left' }}
          />
        }
      />
      
      <div style={{ flex: 1, minHeight: 0 }}>
        {showChat ? (
          <ChatComponent 
            key={chatKey}
            provider={selectedProvider?.value || undefined}
            model={selectedModel?.value || undefined}
            systemPrompt={selectedPrompt?.value}
            promptId={selectedPrompt?.id}
            fullPromptContent={selectedPrompt?.fullContent}
            initialMessages={conversationMessages}
          />
        ) : (
          <div className="chat-placeholder">
            <div className="text-center p-4">Starting new chat...</div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Chat;