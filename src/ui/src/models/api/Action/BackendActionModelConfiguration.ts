/**
 * Model configuration for an action that matches the backend structure
 * This is a representation of the C# ActionModelConfiguration class
 */
export interface BackendActionModelConfiguration {
  /**
   * The provider type enum value (OpenAi, Anthropic, Ollama, Gemini)
   */
  providerType: string;
  
  /**
   * The model name to use for this action
   */
  modelName: string;
  
  /**
   * Whether the model should stream responses
   */
  streaming?: boolean;
  
  /**
   * Additional configuration options as key-value pairs
   * This can include parameters like temperature, maxTokens, etc.
   */
  metadata?: Record<string, string>;
}