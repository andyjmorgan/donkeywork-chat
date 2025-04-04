import { UserProviderType } from './UserProviderType';
import { PascalToCamelCase } from '../../../utils/caseConversion';

/**
 * Original response model matching C# property names (PascalCase)
 */
export interface OriginalProviderCallbackResponseModel {
  ProviderType: UserProviderType;
  Connected: boolean;
  Scopes: string[];
}

/**
 * Frontend model with camelCase properties
 */
export type ProviderCallbackResponseModel = PascalToCamelCase<OriginalProviderCallbackResponseModel>;