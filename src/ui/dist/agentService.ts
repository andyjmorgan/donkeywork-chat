import ApiBase from './apiBase';

export interface AgentNode {
  id: string;
  type: string;
  label: string;
  nodeType: string; 
  position: {
    x: number;
    y: number;
  };
  immutable: boolean;
  // Additional properties based on node type
  template?: string;          // For StringFormatter nodes
  conditions?: Array<{        // For Conditional nodes
    id: string;
    expression: string;
  }>;
}

export interface AgentEdge {
  id: string;
  source: string;
  target: string;
  sourceHandle?: string | null;
  targetHandle?: string | null;
}

export interface AgentModel {
  id?: string;
  name: string;
  description: string;
  tags: string[];
  nodes: AgentNode[];
  edges: AgentEdge[];
}

export interface GetAgentsResponse {
  items: AgentModel[];
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

    return await response.json();
  }

  async getAgent(id: string): Promise<AgentModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/agents/${id}`, {
      credentials: 'include',
      headers: {
        'Cache-Control': 'no-cache'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get agent: ${response.status}`);
    }

    return await response.json();
  }

  async createAgent(agent: AgentModel): Promise<AgentModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/agents`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
        'Cache-Control': 'no-cache'
      },
      body: JSON.stringify(agent)
    });

    if (!response.ok) {
      throw new Error(`Failed to create agent: ${response.status}`);
    }

    return await response.json();
  }

  async updateAgent(id: string, agent: AgentModel): Promise<AgentModel> {
    const response = await fetch(`${this.getApiBaseUrl()}/agents/${id}`, {
      method: 'PUT',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
        'Cache-Control': 'no-cache'
      },
      body: JSON.stringify(agent)
    });

    if (!response.ok) {
      throw new Error(`Failed to update agent: ${response.status}`);
    }

    return await response.json();
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
}

// Export a singleton instance
const agentService = AgentService.getInstance();
export { agentService };