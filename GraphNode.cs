namespace Borium.RDP
{
	internal abstract class GraphNode : GraphBase
	{
#if false
		/** The number of the next node to be created */
		static int graph_next_node_count = 1;

		GraphNode next_node;
		GraphNode previous_node;

		void deleteNode()
		{
			// delete out edges
			while (next_out_edge != null)
			{
				next_out_edge.deleteEdge();
			}
			// delete in edges
			while (next_in_edge != null)
			{
				next_in_edge.deleteEdge();
			}
			// now unlink this node
			if (next_node != null)
			{
				next_node.previous_node = previous_node;
			}
			// point in node at our out
			previous_node.next_node = next_node;
		}

		abstract int getId();

		void insertNode(GraphNode node)
		{
			node.atom_number = graph_next_node_count++;
			node.next_out_edge = null;
			/* Now insert after node_or_graph */
			/* look at rest of list */
			node.next_node = next_node;
			/* point previous at this node */
			next_node = node;
			/* point backlink at base pointer */
			node.previous_node = this;
			/* point next node back at us */
			if (node.next_node != null)
			{
				node.next_node.previous_node = node;
			}
		}
#endif
	}
}
