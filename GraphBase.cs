using System.Collections.Generic;

namespace Borium.RDP
{
	internal class GraphBase
	{
		internal static void graph_epsilon_prune_rdp_tree(GraphBase parent_node)
		{
			if (parent_node != null)
				parent_node.epsilonPruneRdpTree();
		}

		internal int atom_number;
		internal GraphEdge next_in_edge;
		internal GraphEdge next_out_edge;

		void epsilonPruneRdpTree()
		{
			List<GraphNode> deletable = new List<GraphNode>();
			for (GraphEdge this_parent_out_edge = nextOutEdge(); this_parent_out_edge != null; this_parent_out_edge = this_parent_out_edge
					.nextOutEdge())
			{
				GraphNode child_node = destination(this_parent_out_edge);
				child_node.epsilonPruneRdpTree();
				if (child_node.getId() == 0)
				{
					GraphEdge this_child_out_edge, final_child_out_edge = null;
					/* Move child's out edges up */
					GraphNode parent_base = (GraphNode)this;
					/* Run through child edges changing their source to parent */
					for (this_child_out_edge = child_node
							.nextOutEdge(); this_child_out_edge != null; this_child_out_edge = this_child_out_edge
									.nextOutEdge())
					{
						GraphEdge edge_base = this_child_out_edge;
						edge_base.source = parent_base;
						final_child_out_edge = this_child_out_edge;
					}
					/* Move complete run of edges up to before this_parent_out_edge */
					if (final_child_out_edge != null) /* skip if there are no children */
					{
						GraphEdge initial_child_out_edge = child_node.nextOutEdge();
						GraphEdge initial_child_out_edge_base = initial_child_out_edge;
						GraphEdge final_child_out_edge_base = final_child_out_edge;
						GraphEdge parent_out_edge_base = this_parent_out_edge;
						GraphNode child_node_base = child_node;

						parent_out_edge_base.previous_out_edge.next_out_edge = initial_child_out_edge_base;
						initial_child_out_edge_base.previous_out_edge = parent_out_edge_base.previous_out_edge;

						final_child_out_edge_base.next_out_edge = parent_out_edge_base;
						parent_out_edge_base.previous_out_edge = final_child_out_edge_base;

						/* Set the child's out list to empty */
						child_node_base.next_out_edge = null;
					}
					deletable.Add(child_node);
				}
			}
			foreach (GraphNode node in deletable)
			{
				node.deleteNode();
			}
		}

		protected GraphEdge nextOutEdge()
		{
			return next_out_edge;
		}

		private GraphNode destination(GraphEdge edge)
		{
			return edge == null ? null : edge.destination;
		}
	}
}
