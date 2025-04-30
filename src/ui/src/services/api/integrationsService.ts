import ApiBase from './apiBase';
import { ProviderUrlResponseModel } from '../../models/api/provider/ProviderUrlResponseModel';
import { ProviderCallbackResponseModel } from '../../models/api/provider/ProviderCallbackResponseModel';
import { GenericProviderConfigurationModel } from '../../models/api/provider/GenericProviderTypes';
import { toCamelCaseKeys } from '../../utils/caseConversion';
import { ToolProviderType } from '../../models/api/provider/ToolProviderType'; 
import { GetToolProvidersModel } from '../../models/api/provider/GetToolProvidersModel';

class IntegrationsService extends ApiBase {
  private static instance: IntegrationsService;

  private constructor() {
    super();
  }

  static getInstance(): IntegrationsService {
    if (!IntegrationsService.instance) {
      IntegrationsService.instance = new IntegrationsService();
    }
    return IntegrationsService.instance;
  }

  async getProviderAuthUrl(providerType: ToolProviderType, redirectUrl?: string): Promise<ProviderUrlResponseModel> {
    const url = redirectUrl ? 
      `${this.getApiBaseUrl()}/Integrations/authorizeUrl/${providerType}?redirectUrl=${encodeURIComponent(redirectUrl)}` : 
      `${this.getApiBaseUrl()}/Integrations/authorizeUrl/${providerType}`;
    
    const response = await fetch(url, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get provider auth URL: ${response.status}`);
    }

    const data = await response.json();
    return {
      providerType: data.ProviderType || data.providerType,
      authorizationUrl: data.AuthorizationUrl || data.authorizationUrl
    };
  }

  async getToolProviders(): Promise<GetToolProvidersModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/Integrations`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get tool providers: ${response.status}`);
    }

    const data = await response.json();
    return toCamelCaseKeys(data) as GetToolProvidersModel;
  }

  async getGenericProviderConfiguration(providerType: ToolProviderType): Promise<GenericProviderConfigurationModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/Integrations/${providerType}`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get provider configuration: ${response.status}`);
    }

    const data = await response.json();
    
    // IMPORTANT: Don't convert property keys to camelCase for GenericProviderConfiguration
    // because we need to preserve the exact casing from the server
    const configData = {} as GenericProviderConfigurationModel;
    
    // Only convert the main object properties to camelCase (providerType and isEnabled)
    configData.providerType = data.ProviderType || data.providerType;
    configData.isEnabled = data.IsEnabled !== undefined ? data.IsEnabled : (data.isEnabled || false);
    configData.properties = {};
    
    // Keep Properties as-is with original casing
    const properties = data.Properties || data.properties || {};
    Object.entries(properties).forEach(([key, value]) => {
      // Store the properties with the exact same keys and structure
      configData.properties[key] = value as any;
    });
    
    return configData;
  }

  async upsertGenericProviderConfiguration(config: GenericProviderConfigurationModel): Promise<void> {
    try {
      // Convert to PascalCase if necessary for the API
      const apiConfig: {
        ProviderType: string;
        IsEnabled: boolean;
        Properties: Record<string, any>;
      } = {
        ProviderType: config.providerType,
        IsEnabled: config.isEnabled,
        Properties: {}
      };
      
      // Preserve exact property keys from the original config
      if (config.properties) {
        Object.entries(config.properties).forEach(([key, prop]) => {
          // Use the exact key and preserve all original property names
          apiConfig.Properties[key] = {
            // Use original property values to maintain exact casing
            ...(prop as any),
            // Ensure values are updated
            Value: prop.value
          };
        });
      }
      
      const response = await fetch(`${this.getApiBaseUrl()}/Integrations`, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
          'Cache-Control': 'no-cache'
        },
        body: JSON.stringify(apiConfig)
      });
  
      if (!response.ok) {
        let errorMessage = `Failed to save provider configuration: ${response.status}`;
        
        try {
          const errorText = await response.text();
          console.error('API Error Response:', errorText);
          
          if (errorText) {
            // Try to parse as JSON if possible
            try {
              const errorJson = JSON.parse(errorText);
              errorMessage = errorJson.title || errorJson.message || errorJson.error || errorText;
            } catch {
              // If not JSON, use the text directly
              errorMessage += ` - ${errorText}`;
            }
          }
        } catch (parseError) {
          console.error('Error parsing error response:', parseError);
        }
        
        throw new Error(errorMessage);
      }
    } catch (error) {
      console.error('Error in upsertGenericProviderConfiguration:', error);
      throw error;
    }
  }

  async deleteProviderIntegration(providerType: ToolProviderType): Promise<void> {
    try {
      const response = await fetch(`${this.getApiBaseUrl()}/Integrations/${providerType}`, {
        method: 'DELETE',
        credentials: 'include',
        headers: {
          'Cache-Control': 'no-cache'
        }
      });

      if (!response.ok) {
        let errorMessage = `Failed to delete provider integration: ${response.status}`;
        
        try {
          const errorText = await response.text();
          console.error('API Error Response:', errorText);
          
          if (errorText) {
            try {
              const errorJson = JSON.parse(errorText);
              errorMessage = errorJson.title || errorJson.message || errorJson.error || errorText;
            } catch {
              errorMessage += ` - ${errorText}`;
            }
          }
        } catch (parseError) {
          console.error('Error parsing error response:', parseError);
        }
        
        throw new Error(errorMessage);
      }
    } catch (error) {
      console.error('Error in deleteProviderIntegration:', error);
      throw error;
    }
  }
  
  async handleCallback(providerType: ToolProviderType, code: string, redirectUrl: string): Promise<ProviderCallbackResponseModel> {
    const callbackUrl = `${this.getApiBaseUrl()}/Integrations/callback/${providerType}?code=${encodeURIComponent(code)}&redirectUrl=${encodeURIComponent(redirectUrl)}`;
    
    const response = await fetch(callbackUrl, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });
    
    if (!response.ok) {
      let errorMessage = 'Failed to connect to provider';
      
      try {
        const errorData = await response.json();
        errorMessage = errorData.detail || errorMessage;
      } catch (jsonError) {
        errorMessage = `Error ${response.status}: ${response.statusText}`;
      }
      
      throw new Error(errorMessage);
    }
    
    return await response.json() as ProviderCallbackResponseModel;
  }
}

// Export a singleton instance as the default export
const integrationsService = IntegrationsService.getInstance();
export { integrationsService };
export default IntegrationsService.getInstance();