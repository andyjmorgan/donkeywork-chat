import { UserProviderType } from './UserProviderType';

/**
 * Frontend model for provider URL responses
 */
export interface ProviderUrlResponseModel {
  providerType: UserProviderType;
  authorizationUrl: string;
}