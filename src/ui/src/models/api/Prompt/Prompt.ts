/**
 * Represents a variable in a prompt
 */
export interface PromptVariable {
  /**
   * The name of the variable
   */
  name: string;
  
  /**
   * The description of the variable
   */
  description: string;
  
  /**
   * The default value of the variable
   */
  defaultValue?: string;
  
  /**
   * Whether the variable is required
   */
  required: boolean;
}

/**
 * The role of a message in a prompt
 */
export enum PromptMessageRole {
  System = "system",
  User = "user",
  Assistant = "assistant"
}

/**
 * Content type for a message
 */
export enum ContentType {
  Text = "text",
  Resource = "resource"
}

/**
 * Base content for a message
 */
export interface BaseContent {
  /**
   * The type of content
   */
  type: ContentType;
}

/**
 * Text content
 */
export interface TextContent extends BaseContent {
  type: ContentType.Text;
  
  /**
   * The text content
   */
  text: string;
}

/**
 * Resource item for resource content
 */
export interface ResourceItem {
  /**
   * The source of the resource
   */
  source: string;
  
  /**
   * The type of the resource
   */
  type: string;
}

/**
 * Resource content
 */
export interface ResourceContent extends BaseContent {
  type: ContentType.Resource;
  
  /**
   * The resource items
   */
  items: ResourceItem[];
}

/**
 * Type for the content of a message
 */
export type MessageContent = TextContent | ResourceContent;

/**
 * Represents a message in a prompt
 */
export interface PromptMessage {
  /**
   * The role of the message
   */
  role: PromptMessageRole;
  
  /**
   * The content of the message
   */
  content: MessageContent[];
}