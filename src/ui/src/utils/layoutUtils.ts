import dagre from '@dagrejs/dagre';
import { Node, Edge } from 'reactflow';

// Default node dimensions
const nodeWidth = 180;
const nodeHeight = 40;

/**
 * Applies a layout algorithm to position nodes in an organized way
 * 
 * @param nodes - Array of ReactFlow nodes
 * @param edges - Array of ReactFlow edges
 * @param direction - Layout direction: 'TB' (top to bottom) or 'LR' (left to right)
 * @returns Object containing the repositioned nodes and original edges
 */
export const getLayoutedElements = (
  nodes: Node[],
  edges: Edge[],
  direction: 'TB' | 'LR' = 'TB'
): { nodes: Node[]; edges: Edge[] } => {
  if (!nodes.length) return { nodes, edges };

  // Create a new directed graph
  const dagreGraph = new dagre.graphlib.Graph();
  dagreGraph.setDefaultEdgeLabel(() => ({}));
  
  // Set graph direction and spacing
  dagreGraph.setGraph({ 
    rankdir: direction,
    ranker: 'network-simplex',
    marginx: 50,
    marginy: 50,
    nodesep: 80,
    edgesep: 30,
    ranksep: 80
  });

  // Add nodes to the graph with their dimensions
  nodes.forEach((node) => {
    // Use custom dimensions if available, otherwise use defaults
    const width = node.width || nodeWidth;
    const height = node.height || nodeHeight;
    dagreGraph.setNode(node.id, { width, height });
  });

  // Add edges to the graph
  edges.forEach((edge) => {
    dagreGraph.setEdge(edge.source, edge.target);
  });

  // Calculate the layout (positions)
  dagre.layout(dagreGraph);

  // Transform node positions to ReactFlow format
  const layoutedNodes = nodes.map((node) => {
    const nodeWithPosition = dagreGraph.node(node.id);
    
    return {
      ...node,
      // We need to account for the node dimensions since Dagre positions the center
      position: {
        x: nodeWithPosition.x - (node.width || nodeWidth) / 2,
        y: nodeWithPosition.y - (node.height || nodeHeight) / 2,
      },
    };
  });

  return { nodes: layoutedNodes, edges };
};