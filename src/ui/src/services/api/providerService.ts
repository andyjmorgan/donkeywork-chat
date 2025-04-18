import ApiBase from './apiBase';
import { UserProviderType } from '../../models/api/provider/UserProviderType';
import { UserProviderResponseModel } from '../../models/api/provider/UserProviderResponseModel';
import { ProviderUrlResponseModel } from '../../models/api/provider/ProviderUrlResponseModel';
import { ProviderCallbackResponseModel } from '../../models/api/provider/ProviderCallbackResponseModel';
import { 
  GenericProviderType, 
  GenericProviderConfigurationModel, 
  GenericProvidersModel,
  GenericProviderPropertyModel 
} from '../../models/api/provider/GenericProviderTypes';
import { toCamelCaseKeys } from '../../utils/caseConversion';

class ProviderService extends ApiBase {
  private static instance: ProviderService;

  private constructor() {
    super();
  }

  static getInstance(): ProviderService {
    if (!ProviderService.instance) {
      ProviderService.instance = new ProviderService();
    }
    return ProviderService.instance;
  }

  async getProviderAuthUrl(providerType: UserProviderType, redirectUrl?: string): Promise<ProviderUrlResponseModel> {
    const url = redirectUrl ? 
      `${this.getApiBaseUrl()}/Provider/authorizeUrl/${providerType}?redirectUrl=${encodeURIComponent(redirectUrl)}` : 
      `${this.getApiBaseUrl()}/Provider/authorizeUrl/${providerType}`;
    
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
      // Handle either PascalCase or camelCase property names in the API response
      providerType: data.ProviderType || data.providerType,
      authorizationUrl: data.AuthorizationUrl || data.authorizationUrl
    };
  }

  async getUserProviders(): Promise<UserProviderResponseModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/Provider`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get user providers: ${response.status}`);
    }

    const data = await response.json();
    
    // Ensure we have the expected format
    const formattedData = {
      ProviderConfiguration: data.ProviderConfiguration || data.providerConfiguration || {}
    };
    
    return formattedData;
  }

  async deleteUserProvider(providerType: UserProviderType): Promise<void> {
    const response = await fetch(`${this.getApiBaseUrl()}/Provider/${providerType}`, {
      method: 'DELETE',
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to delete user provider: ${response.status}`);
    }
  }

  async getGenericProviders(): Promise<GenericProvidersModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/GenericProvider`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get generic providers: ${response.status}`);
    }

    const data = await response.json();
    return toCamelCaseKeys(data) as GenericProvidersModel;
  }

  async getGenericProviderConfiguration(providerType: GenericProviderType): Promise<GenericProviderConfigurationModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/GenericProvider/${providerType}`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get provider configuration: ${response.status}`);
    }

    const data = await response.json();
    console.log('Original API response:', data);
    
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
      // This preserves casing like "BaseUrl" and "ApiKey"
      configData.properties[key] = value as GenericProviderPropertyModel;
    });
    
    console.log('Processed configuration with preserved casing:', configData);
    return configData;
  }

  async upsertGenericProviderConfiguration(config: GenericProviderConfigurationModel): Promise<void> {
    try {
      // Convert to PascalCase if necessary for the API
      const apiConfig: {
        ProviderType: GenericProviderType;
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
      
      console.log('Sending to API:', apiConfig);
      
      const response = await fetch(`${this.getApiBaseUrl()}/GenericProvider`, {
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
      
      console.log('Provider configuration saved successfully');
    } catch (error) {
      console.error('Error in upsertGenericProviderConfiguration:', error);
      throw error;
    }
  }

  async deleteGenericProviderConfiguration(providerType: GenericProviderType): Promise<void> {
    try {
      const response = await fetch(`${this.getApiBaseUrl()}/GenericProvider/${providerType}`, {
        method: 'DELETE',
        credentials: 'include',
        headers: {
          'Cache-Control': 'no-cache'
        }
      });

      if (!response.ok) {
        let errorMessage = `Failed to delete provider configuration: ${response.status}`;
        
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
      
      console.log('Provider configuration deleted successfully');
    } catch (error) {
      console.error('Error in deleteGenericProviderConfiguration:', error);
      throw error;
    }
  }
}

// Export a singleton instance as the default export
const providerService = ProviderService.getInstance();
export { providerService };
export default ProviderService.getInstance();