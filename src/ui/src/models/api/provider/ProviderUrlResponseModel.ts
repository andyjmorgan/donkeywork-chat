import { UserProviderType } from './UserProviderType';
import { PascalToCamelCase } from '../../../utils/caseConversion';

/**
 * Original response model matching C# property names (PascalCase)
 */
export interface OriginalProviderUrlResponseModel {
  ProviderType: UserProviderType;
  AuthorizationUrl: string;
}

/**
 * Frontend model with camelCase properties
 */
export type ProviderUrlResponseModel = PascalToCamelCase<OriginalProviderUrlResponseModel>;