import { UserProviderType } from './UserProviderType';

/**
 * Response model matching C# property names (PascalCase)
 * We're keeping the original casing to match the API
 */
export interface UserProviderResponseModel {
  ProviderConfiguration: Record<UserProviderType, string[]>;
}