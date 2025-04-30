import ApiBase from './apiBase';
import { UserProviderType } from '../../models/api/provider/UserProviderType';
import { UserProviderResponseModel } from '../../models/api/provider/UserProviderResponseModel';
import { ProviderUrlResponseModel } from '../../models/api/provider/ProviderUrlResponseModel';
import { ProviderCallbackResponseModel } from '../../models/api/provider/ProviderCallbackResponseModel';
import { 
  GenericProviderType, 
  GenericProviderConfigurationModel, 
  GenericProvidersModel
} from '../../models/api/provider/GenericProviderTypes';
import { toCamelCaseKeys } from '../../utils/caseConversion';
import { integrationsService } from './integrationsService';
import { ToolProviderType } from '../../models/api/provider/ToolProviderType';

/**
 * @deprecated Use integrationsService instead. This service will be removed in a future version.
 */
class ProviderService extends ApiBase {
  private static instance: ProviderService;

  private constructor() {
    super();
    console.warn('ProviderService is deprecated. Please use integrationsService instead.');
  }

  static getInstance(): ProviderService {
    if (!ProviderService.instance) {
      ProviderService.instance = new ProviderService();
    }
    return ProviderService.instance;
  }

  /**
   * @deprecated Use integrationsService.getProviderAuthUrl instead
   */
  async getProviderAuthUrl(providerType: UserProviderType, redirectUrl?: string): Promise<ProviderUrlResponseModel> {
    console.warn('This method is deprecated. Use integrationsService.getProviderAuthUrl instead.');
    return integrationsService.getProviderAuthUrl(providerType as unknown as ToolProviderType, redirectUrl);
  }

  /**
   * @deprecated Use integrationsService.getToolProviders instead
   */
  async getUserProviders(): Promise<UserProviderResponseModel> {
    console.warn('This method is deprecated. Use integrationsService.getToolProviders instead.');
    
    // This is a compatibility layer
    const toolProviders = await integrationsService.getToolProviders();
    
    // Create a format that's compatible with the old UserProviderResponseModel
    const providerConfig: Record<string, string[]> = {};
    
    toolProviders.toolProviders.forEach(provider => {
      if (provider.isConnected && 
          (provider.providerType === ToolProviderType.Microsoft || 
           provider.providerType === ToolProviderType.Google || 
           provider.providerType === ToolProviderType.Discord)) {
        
        const scopes: string[] = [];
        
        // Collect scopes from all applications
        Object.values(provider.applications).forEach(app => {
          if (app.application) {
            scopes.push(...(app.scopes || []));
          }
        });
        
        providerConfig[provider.providerType] = scopes;
      }
    });
    
    return {
      ProviderConfiguration: providerConfig
    };
  }

  /**
   * @deprecated Use integrationsService.deleteProviderIntegration instead
   */
  async deleteUserProvider(providerType: UserProviderType): Promise<void> {
    console.warn('This method is deprecated. Use integrationsService.deleteProviderIntegration instead.');
    return integrationsService.deleteProviderIntegration(providerType as unknown as ToolProviderType);
  }

  /**
   * @deprecated Use integrationsService.getToolProviders instead
   */
  async getGenericProviders(): Promise<GenericProvidersModel> {
    console.warn('This method is deprecated. Use integrationsService.getToolProviders instead.');
    
    // This is a compatibility layer
    const toolProviders = await integrationsService.getToolProviders();
    
    // Create a format that's compatible with the old GenericProvidersModel
    const genericProviders: GenericProvidersModel = {
      providers: []
    };
    
    toolProviders.toolProviders.forEach(provider => {
      if (provider.authorizationType === 'Static') {
        genericProviders.providers.push({
          name: provider.name,
          type: provider.providerType as any,
          description: provider.description,
          isConnected: provider.isConnected,
          isEnabled: true, // Assume enabled if available
          image: provider.icon,
          tags: [],
          capabilities: {}
        });
      }
    });
    
    return genericProviders;
  }

  /**
   * @deprecated Use integrationsService.getGenericProviderConfiguration instead
   */
  async getGenericProviderConfiguration(providerType: GenericProviderType): Promise<GenericProviderConfigurationModel> {
    console.warn('This method is deprecated. Use integrationsService.getGenericProviderConfiguration instead.');
    return integrationsService.getGenericProviderConfiguration(providerType as unknown as ToolProviderType);
  }

  /**
   * @deprecated Use integrationsService.upsertGenericProviderConfiguration instead
   */
  async upsertGenericProviderConfiguration(config: GenericProviderConfigurationModel): Promise<void> {
    console.warn('This method is deprecated. Use integrationsService.upsertGenericProviderConfiguration instead.');
    return integrationsService.upsertGenericProviderConfiguration(config);
  }

  /**
   * @deprecated Use integrationsService.deleteProviderIntegration instead
   */
  async deleteGenericProviderConfiguration(providerType: GenericProviderType): Promise<void> {
    console.warn('This method is deprecated. Use integrationsService.deleteProviderIntegration instead.');
    return integrationsService.deleteProviderIntegration(providerType as unknown as ToolProviderType);
  }
}

// Export a singleton instance as the default export
const providerService = ProviderService.getInstance();
export { providerService };
export default ProviderService.getInstance();