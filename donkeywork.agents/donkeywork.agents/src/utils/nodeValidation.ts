import { Node, Edge } from 'reactflow';
import { 
  NodeTypeEnum, 
  StringFormatterNodeData, 
  ConditionalNodeData,
  ValidationResult,
  ValidationError
} from '../types/nodes';

// Validates that all handles are connected
export function validateConnections(node: Node, edges: Edge[]): ValidationResult {
  const result: ValidationResult = { valid: true, errors: [] };
  
  // For input nodes, validate that they have at least one outgoing connection
  if (node.type === 'inputNode') {
    const outgoingConnections = edges.filter(edge => edge.source === node.id);
    if (outgoingConnections.length === 0) {
      result.valid = false;
      result.errors.push({
        message: `${node.data.label} node must be connected to at least one other node`,
        severity: 'error'
      });
    }
    return result;
  }
  
  // For output nodes, validate that they have at least one incoming connection
  if (node.type === 'outputNode') {
    const incomingConnections = edges.filter(edge => edge.target === node.id);
    if (incomingConnections.length === 0) {
      result.valid = false;
      result.errors.push({
        message: `${node.data.label} node must receive input from at least one other node`,
        severity: 'error'
      });
    }
    return result;
  }
  
  // For conditional nodes, validate that all condition handles and the default handle are connected
  if (node.type === 'conditionalNode') {
    // Check incoming connections
    const incomingConnections = edges.filter(edge => edge.target === node.id);
    if (incomingConnections.length === 0) {
      result.valid = false;
      result.errors.push({
        message: `${node.data.label} node must receive input from at least one other node`,
        severity: 'error'
      });
    }
    
    // Check each condition handle and the default handle
    const conditions = node.data.conditions || [];
    const expectedHandles = [
      ...conditions.map(cond => `condition-${cond.id}`),
      'default'
    ];
    
    // Check if each expected handle has a connection
    expectedHandles.forEach(handleId => {
      const handleConnections = edges.filter(
        edge => edge.source === node.id && edge.sourceHandle === handleId
      );
      
      if (handleConnections.length === 0) {
        result.valid = false;
        if (handleId === 'default') {
          result.errors.push({
            message: `${node.data.label} node's default path is not connected`,
            severity: 'error'
          });
        } else {
          const conditionIndex = conditions.findIndex(
            cond => `condition-${cond.id}` === handleId
          );
          result.errors.push({
            message: `${node.data.label} node's condition ${conditionIndex + 1} path is not connected`,
            severity: 'error'
          });
        }
      }
    });
    
    return result;
  }
  
  // For string formatter and other standard nodes, validate input and output connections
  const incomingConnections = edges.filter(edge => edge.target === node.id);
  const outgoingConnections = edges.filter(edge => edge.source === node.id);
  
  if (incomingConnections.length === 0) {
    result.valid = false;
    result.errors.push({
      message: `${node.data.label} node must receive input from at least one other node`,
      severity: 'error'
    });
  }
  
  if (outgoingConnections.length === 0) {
    result.valid = false;
    result.errors.push({
      message: `${node.data.label} node must be connected to at least one other node`,
      severity: 'error'
    });
  }
  
  return result;
}

// Validates a string formatter node
export function validateStringFormatter(node: Node<StringFormatterNodeData>): ValidationResult {
  const result: ValidationResult = { valid: true, errors: [] };
  
  // Check if template is empty
  if (!node.data.template || node.data.template.trim() === '') {
    result.valid = false;
    result.errors.push({
      message: `${node.data.label} node must have a template defined`,
      severity: 'error'
    });
  }
  
  // Basic Scriban syntax validation
  const template = node.data.template || '';
  
  // Check for unclosed braces
  const openBraces = (template.match(/\{\{/g) || []).length;
  const closeBraces = (template.match(/\}\}/g) || []).length;
  
  if (openBraces !== closeBraces) {
    result.valid = false;
    result.errors.push({
      message: `${node.data.label} template has unclosed braces`,
      severity: 'error'
    });
  }
  
  return result;
}

// Validates a conditional node
export function validateConditional(node: Node<ConditionalNodeData>): ValidationResult {
  const result: ValidationResult = { valid: true, errors: [] };
  
  // Check if there are conditions defined
  if (!node.data.conditions || node.data.conditions.length === 0) {
    result.valid = false;
    result.errors.push({
      message: `${node.data.label} node must have at least one condition defined`,
      severity: 'error'
    });
    return result;
  }
  
  // Check each condition
  node.data.conditions.forEach((condition, index) => {
    if (!condition.expression || condition.expression.trim() === '') {
      result.valid = false;
      result.errors.push({
        message: `${node.data.label} node condition ${index + 1} is empty`,
        severity: 'error'
      });
    } else {
      // Basic Scriban syntax validation for each condition
      const expression = condition.expression.trim();
      const openBraces = (expression.match(/\{\{/g) || []).length;
      const closeBraces = (expression.match(/\}\}/g) || []).length;
      
      // Check for proper template syntax (must be wrapped in {{ }})
      if (openBraces === 0 || closeBraces === 0) {
        result.valid = false;
        result.errors.push({
          message: `${node.data.label} node condition ${index + 1} must be a valid template expression wrapped in '{{ }}' delimiters`,
          severity: 'error'
        });
      } else if (openBraces !== closeBraces) {
        result.valid = false;
        result.errors.push({
          message: `${node.data.label} node condition ${index + 1} has unclosed braces`,
          severity: 'error'
        });
      }
      
      // Check for comparison operators to ensure it's a boolean expression
      const conditionContent = expression.replace(/\{\{|\}\}/g, '').trim();
      const hasComparisonOperator = /[><=!]/.test(conditionContent) || 
                                   /\s(and|or|not|true|false)\s/.test(conditionContent) ||
                                   /\|\s*string\.contains/.test(conditionContent);
      
      if (!hasComparisonOperator && openBraces > 0 && closeBraces > 0) {
        result.valid = false;
        result.errors.push({
          message: `${node.data.label} node condition ${index + 1} should evaluate to a boolean (use comparison operators like ==, >, <, etc.)`,
          severity: 'warning'
        });
      }
    }
  });
  
  return result;
}

// Validates any node type
export function validateNode(node: Node, edges: Edge[]): ValidationResult {
  // Add node-specific content validation first
  let contentValidationResult: ValidationResult = { valid: true, errors: [] };
  
  if (node.type === 'stringFormatterNode') {
    contentValidationResult = validateStringFormatter(node as Node<StringFormatterNodeData>);
  } else if (node.type === 'conditionalNode') {
    contentValidationResult = validateConditional(node as Node<ConditionalNodeData>);
  }
  
  // Then run connection validation for all nodes
  const connectionResult = validateConnections(node, edges);
  
  // Combine results
  return {
    valid: connectionResult.valid && contentValidationResult.valid,
    errors: [...contentValidationResult.errors, ...connectionResult.errors]
  };
}

// Validates the entire agent
export function validateAgent(nodes: Node[], edges: Edge[]): ValidationResult {
  const result: ValidationResult = { valid: true, errors: [] };
  
  // Check for input and output nodes first (structural validation)
  const inputNodes = nodes.filter(node => node.type === 'inputNode');
  const outputNodes = nodes.filter(node => node.type === 'outputNode');
  
  if (inputNodes.length === 0) {
    result.valid = false;
    result.errors.push({
      message: 'Agent must include an Input node',
      severity: 'error'
    });
  } else if (inputNodes.length > 1) {
    result.valid = false;
    result.errors.push({
      message: 'Agent can only have one Input node',
      severity: 'error'
    });
  }
  
  if (outputNodes.length === 0) {
    result.valid = false;
    result.errors.push({
      message: 'Agent must include an Output node',
      severity: 'error'
    });
  } else if (outputNodes.length > 1) {
    result.valid = false;
    result.errors.push({
      message: 'Agent can only have one Output node',
      severity: 'error'
    });
  }
  
  // Check for cycles
  if (hasCycle(nodes, edges)) {
    result.valid = false;
    result.errors.push({
      message: 'Agent contains cycles (circular connections), which are not allowed',
      severity: 'error'
    });
  }
  
  // After structural validation, validate each node
  nodes.forEach(node => {
    const nodeResult = validateNode(node, edges);
    if (!nodeResult.valid) {
      result.valid = false;
      
      // Add node label to each error message for clarity
      const prefixedErrors = nodeResult.errors.map(error => ({
        message: `${node.data.label}: ${error.message}`,
        severity: error.severity
      }));
      
      result.errors.push(...prefixedErrors);
    }
  });
  
  // If there are disconnected nodes, add a summary error
  const disconnectedNodes = nodes.filter(node => {
    const hasIncoming = edges.some(edge => edge.target === node.id);
    const hasOutgoing = edges.some(edge => edge.source === node.id);
    
    // Input nodes only need outgoing, output nodes only need incoming
    if (node.type === 'inputNode') return !hasOutgoing;
    if (node.type === 'outputNode') return !hasIncoming;
    
    // All other nodes need both
    return !hasIncoming || !hasOutgoing;
  });
  
  if (disconnectedNodes.length > 0) {
    result.errors.push({
      message: `Agent has ${disconnectedNodes.length} disconnected nodes that need to be connected`,
      severity: 'error'
    });
  }
  
  return result;
}

// For backward compatibility, just call validateAgent
export function validatePipeline(nodes: Node[], edges: Edge[]): ValidationResult {
  return validateAgent(nodes, edges);
}

// Helper function to check for cycles in the graph
function hasCycle(nodes: Node[], edges: Edge[]): boolean {
  // Build adjacency list
  const graph: Record<string, string[]> = {};
  
  nodes.forEach(node => {
    graph[node.id] = [];
  });
  
  edges.forEach(edge => {
    if (graph[edge.source]) {
      graph[edge.source].push(edge.target);
    }
  });
  
  // Track visited nodes and recursion stack
  const visited: Record<string, boolean> = {};
  const recStack: Record<string, boolean> = {};
  
  // DFS function to detect cycle
  function isCyclicUtil(nodeId: string): boolean {
    // Mark current node as visited and add to recursion stack
    visited[nodeId] = true;
    recStack[nodeId] = true;
    
    // Visit all adjacent vertices
    for (const neighbor of graph[nodeId]) {
      // If not visited, recursive call
      if (!visited[neighbor] && isCyclicUtil(neighbor)) {
        return true;
      } else if (recStack[neighbor]) {
        // If visited and in recursion stack, cycle found
        return true;
      }
    }
    
    // Remove from recursion stack
    recStack[nodeId] = false;
    return false;
  }
  
  // Check all nodes
  for (const nodeId in graph) {
    if (!visited[nodeId] && isCyclicUtil(nodeId)) {
      return true;
    }
  }
  
  return false;
}
