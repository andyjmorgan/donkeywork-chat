import ApiBase from './apiBase';
import {BaseNodeData, NodeTypeEnum} from "../../types/nodes";

export interface AgentNode {
  id: string;
  label: string;
  nodeType: NodeTypeEnum;
  position: AgentNodePosition;
  metadata: BaseNodeData; // this depends on the node type enum.
}

export interface AgentSummaryModel {
  id?: string;
  name: string;
  description: string;
  tags: string[];
  executionCount?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface AgentNodePosition {
    x: number,
    y: number,
}
export interface AgentModel {
    id: string;
    name: string;
    description: string;
    tags: string[];
    nodeEdges: AgentNodeEdgeModel[];
    nodes: AgentNode[];
    agent: {
        name: string;
        description: string;
    };
}

export interface AgentNodeEdgeModel {
    id?: string;
    sourceNodeId: string,
    targetNodeId: string,
    sourceNodeHandle: string | null,
    targetNodeHandle: string | null,
}

export interface GetAgentsResponse {
  agents: AgentSummaryModel[];
  totalCount: number;
}

export class AgentService extends ApiBase {
  private static instance: AgentService;

  private constructor() {
    super();
  }

  static getInstance(): AgentService {
    if (!AgentService.instance) {
      AgentService.instance = new AgentService();
    }
    return AgentService.instance;
  }

  async getAgents(page: number = 1, pageSize: number = 10): Promise<GetAgentsResponse> {
    const response = await fetch(`${this.getApiBaseUrl()}/agents?page=${page}&pageSize=${pageSize}`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get agents: ${response.status}`);
    }

    // Convert response to our expected format
    const data = await response.json();
    
    // Backend API returns data as { totalCount, agents } which matches our interface
    // Ensure we always return a valid response even if server format changes
    return {
      agents: Array.isArray(data.agents) ? data.agents : (data.items || []),
      totalCount: data.totalCount || 0
    };
  }

  async deleteAgent(id: string): Promise<void> {
    const response = await fetch(`${this.getApiBaseUrl()}/agents/${id}`, {
      method: 'DELETE',
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to delete agent: ${response.status}`);
    }
  }
  
  // Implement the getAgent method to load an agent by ID
  async getAgent(id: string): Promise<AgentModel> {
    console.log('Loading agent data with ID:', id);
    
    const response = await fetch(`${this.getApiBaseUrl()}/agents/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Cache-Control': 'no-cache'
      },
      credentials: 'include'
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Failed to load agent:', errorText);
      throw new Error(`Failed to load agent: ${response.status} - ${errorText}`);
    }
    
    return await response.json();
  }
  
  // Implement the saveAgent method to save an agent
  async saveAgent(agentData: AgentModel): Promise<AgentModel> {
    console.log('Saving agent data:', agentData);
    
    // Always use POST for saving agents
    const method = 'POST';
    const url = `${this.getApiBaseUrl()}/agents`;
    
    const response = await fetch(url, {
      method,
      headers: {
        'Content-Type': 'application/json',
        'Cache-Control': 'no-cache'
      },
      credentials: 'include',
      body: JSON.stringify(agentData)
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Failed to save agent:', errorText);
      throw new Error(`Failed to save agent: ${response.status} - ${errorText}`);
    }
    
    return await response.json();
  }
}

// Export a singleton instance
const agentService = AgentService.getInstance();
export { agentService };