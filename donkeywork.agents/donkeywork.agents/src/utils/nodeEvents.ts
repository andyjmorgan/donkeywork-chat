// Simple event bus for node-related events
type EventCallback = (nodeId: string, newData: any) => void;

class NodeEventBus {
  private static instance: NodeEventBus;
  private updateCallbacks: EventCallback[] = [];

  private constructor() {}

  public static getInstance(): NodeEventBus {
    if (!NodeEventBus.instance) {
      NodeEventBus.instance = new NodeEventBus();
    }
    return NodeEventBus.instance;
  }

  public subscribe(callback: EventCallback): () => void {
    this.updateCallbacks.push(callback);
    
    // Return an unsubscribe function
    return () => {
      this.updateCallbacks = this.updateCallbacks.filter(cb => cb !== callback);
    };
  }

  public emitUpdate(nodeId: string, newData: any): void {
    this.updateCallbacks.forEach(callback => callback(nodeId, newData));
  }
}

export const nodeEvents = NodeEventBus.getInstance();