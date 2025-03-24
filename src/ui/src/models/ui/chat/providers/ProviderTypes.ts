/**
 * Interface for an AI provider
 */
export interface Provider {
  /**
   * Display name of the provider
   */
  name: string;
  
  /**
   * Unique identifier for the provider
   */
  value: string;
}

/**
 * Interface for an AI model
 */
export interface Model {
  /**
   * Display name of the model
   */
  name: string;
  
  /**
   * Unique identifier for the model
   */
  value: string;
  
  /**
   * The provider that offers this model
   */
  provider: string;
}

/**
 * Interface for a system prompt
 */
export interface Prompt {
  /**
   * Unique identifier for the prompt
   */
  id?: string;
  
  /**
   * Display name of the prompt
   */
  name: string;
  
  /**
   * The actual prompt content
   */
  value: string;
  
  /**
   * Optional description of the prompt
   */
  description?: string;
}