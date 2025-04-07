import { UserProviderType } from './UserProviderType';

/**
 * Frontend model for provider callback responses
 */
export interface ProviderCallbackResponseModel {
  providerType: UserProviderType;
  connected: boolean;
  scopes: string[];
}