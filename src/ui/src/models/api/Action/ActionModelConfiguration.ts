/**
 * Frontend UI model configuration for an action
 * This is used for the UI state but gets converted to BackendActionModelConfiguration before API calls
 */
export interface ActionModelConfiguration {
  /**
   * The provider to use for this action
   */
  provider: string;
  
  /**
   * The model to use for this action
   */
  model: string;
  
  /**
   * Model parameters as key-value pairs
   * This can include parameters like temperature, maxTokens, etc.
   */
  parameters: Record<string, string>;
}