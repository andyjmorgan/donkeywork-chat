// Types for Generic Provider API

export enum GenericProviderType {
  Swarmpit = 'Swarmpit'
}

export interface GenericProviderPropertyModel {
  key: string;
  friendlyName: string;
  type: GenericProviderPropertyType;
  value?: any;
  required: boolean;
}

export enum GenericProviderPropertyType {
  String = 0,
  Secret = 1,
  Boolean = 2,
  Integer = 3,
  Double = 4
}

export interface GenericProviderConfigurationModel {
  providerType: GenericProviderType;
  isEnabled: boolean;
  properties: Record<string, GenericProviderPropertyModel>;
}

export interface GenericProviderDefinition {
  name: string;
  type: GenericProviderType;
  description: string;
  isConnected: boolean;
  isEnabled: boolean;
  image: string;
  tags: string[];
  capabilities: Record<string, string>;
}

export interface GenericProvidersModel {
  providers: GenericProviderDefinition[];
}