using System;
using static Borium.RDP.GraphNode;
using static Borium.RDP.GraphEdge;

namespace Borium.RDP
{
	internal class Graph<NodeData, EdgeData> where NodeData : GraphNode where EdgeData : GraphEdge
	{
#if false
		/** The number of the next graph to be created */
		private static int graph_next_graph_count = 1;

		/** The list of active graph structures */
		static Graph<? extends GraphNode, ? extends GraphEdge> graph_list = new Graph<>();

		GraphNode root;
		Graph<? extends GraphNode, ? extends GraphEdge> next_graph;
		Graph<? extends GraphNode, ? extends GraphEdge> previous_graph;
		GraphNode next_node;
		String id;
		int atom_number;

		public void insertNode(GraphNode node)
		{
			node.atom_number = graph_next_node_count++;
			node.next_out_edge = null;
			/* Now insert after node_or_graph */
			/* look at rest of list */
			node.next_node = next_node;
			/* point previous at this node */
			next_node = node;
			/* point backlink at base pointer */
			node.previous_node = null;
			/* point next node back at us */
			if (node.next_node != null)
			{
				node.next_node.previous_node = node;
			}
		}

		void insertGraph(String id)
		{
			atom_number = graph_next_graph_count++;
			next_node = null;
			graph_next_node_count = graph_next_edge_count = 1;
			// Now insert at destination of graph_list
			next_graph = graph_list.next_graph;
			graph_list.next_graph = this;
			previous_graph = graph_list;
			// if rest of list is non-null... point next node back at us
			if (next_graph != null)
			{
				next_graph.previous_graph = this;
			}
			this.id = id;
		}

		void setRoot(GraphNode root)
		{
			this.root = root;
		}
#endif
	}
}
