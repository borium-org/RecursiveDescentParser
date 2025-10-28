namespace Borium.RDP
{
	internal class GraphEdge : GraphBase
	{
		/// <summary>
		/// The number of the next edge to be created
		/// </summary>
		internal static int graph_next_edge_count = 1;

		GraphNode destination;
		GraphNode source;
		GraphBase previous_in_edge;
		GraphBase previous_out_edge;

#if false
		void deleteEdge()
		{
			/* unlink this edge from the out list */
			/* make next node point back to our in */
			if (next_out_edge != null)
			{
				next_out_edge.previous_out_edge = previous_out_edge;
			}
			/* point in node at our out */
			previous_out_edge.next_out_edge = next_out_edge;
			/* unlink this edge from the in list */
			/* make next node point back to our in */
			if (next_in_edge != null)
			{
				next_in_edge.previous_in_edge = previous_in_edge;
			}
			/* point in node at our out */
			if (previous_in_edge != null)
			{
				previous_in_edge.next_in_edge = next_in_edge;
			}
		}
#endif

		internal GraphEdge insertEdgeAfterFinal(GraphNode destination_node, GraphNode source_node)
		{
			GraphNode source_node_base = source_node;
			GraphNode destination_node_base = destination_node;
			GraphBase temp_edge;

			atom_number = graph_next_edge_count++;

			/* source and out-edge processing */
			for (temp_edge = source_node; temp_edge.next_out_edge != null; temp_edge = temp_edge.next_out_edge)
			{
			}

			next_out_edge = temp_edge.next_out_edge; /* look at rest of list */
			temp_edge.next_out_edge = this; /* point previous at this node */
			/* point backlink at source_base pointer */
			previous_out_edge = temp_edge;

			if (next_out_edge != null)
			{
				/* point next node back at us */
				next_out_edge.previous_out_edge = this;
			}
			source = source_node_base;
			/* destination and in-edge processing */
			for (temp_edge = destination_node; temp_edge.next_out_edge != null; temp_edge = temp_edge.next_out_edge)
			{
			}
			next_in_edge = temp_edge.next_in_edge; /* look at rest of list */
			/* point previous at this node */
			destination_node_base.next_in_edge = this;
			/* point backlink at destination_base pointer */
			previous_in_edge = temp_edge;
			if (next_in_edge != null)
			{
				/* point next node back at us */
				next_in_edge.previous_in_edge = this;
			}
			destination = destination_node_base;
			return this;
		}
	}
}
