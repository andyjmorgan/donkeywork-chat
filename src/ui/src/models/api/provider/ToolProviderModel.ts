import { ToolProviderType } from "./ToolProviderType";
import { ToolProviderAuthorizationType } from "./ToolProviderAuthorizationType";
import { ToolProviderApplicationType, ToolProviderApplicationDefinition } from "./ToolProviderApplicationType";

export interface ToolProviderModel {
  providerType: ToolProviderType;
  authorizationType: ToolProviderAuthorizationType;
  name: string;
  description: string;
  icon: string;
  isConnected: boolean;
  applications: Record<string, ToolProviderApplicationDefinition>; // Key is the serialized enum value
}