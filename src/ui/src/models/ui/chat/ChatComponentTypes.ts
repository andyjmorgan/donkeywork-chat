/**
 * Props for the ChatComponent
 */
export interface ChatComponentProps {
  /**
   * The AI provider to use (e.g., 'openai', 'anthropic')
   */
  provider?: string;
  
  /**
   * The specific model to use (e.g., 'gpt-4o-mini', 'claude-3-sonnet')
   */
  model?: string;
  
  /**
   * System prompt to guide the AI behavior
   */
  systemPrompt?: string;
  
  /**
   * Optional prompt ID to use instead of including the prompt content
   */
  promptId?: string;
  
  /**
   * Full array of prompt content items
   */
  fullPromptContent?: string[];
  
  /**
   * Initial messages to populate the chat with (for continuing conversations)
   */
  initialMessages?: any[];
}

/**
 * Interface for token usage information
 */
export interface TokenUsage {
  /**
   * Number of input tokens
   */
  input: number;
  
  /**
   * Number of output tokens
   */
  output: number;
}

/**
 * Interface for the expanded sections in the chat UI
 */
export interface ExpandedSections {
  [key: string]: boolean;
}

/**
 * Interface for the chat component state
 */
export interface ChatComponentState {
  messages: any[];
  inputMessage: string;
  isLoading: boolean;
  currentAction: string;
  viewMessageDialog: boolean;
  selectedMessage: any | null;
  tokenUsage: TokenUsage | null;
  toolsUsed: string[];
  messageMetadata: Record<string, any>;
  expandedSections: Record<string, boolean>;
}