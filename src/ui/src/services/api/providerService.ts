import ApiBase from './apiBase';
import { UserProviderType } from '../../models/api/provider/UserProviderType';
import { UserProviderResponseModel } from '../../models/api/provider/UserProviderResponseModel';
import { ProviderUrlResponseModel } from '../../models/api/provider/ProviderUrlResponseModel';
import { ProviderCallbackResponseModel } from '../../models/api/provider/ProviderCallbackResponseModel';
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
    return toCamelCaseKeys(data);
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
}

// Export a singleton instance as the default export
const providerService = ProviderService.getInstance();
export { providerService };
export default ProviderService.getInstance();