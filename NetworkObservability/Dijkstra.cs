using System;
using System.Collections.Generic;
using PriorityQueue;

namespace NetworkObservability
{
	class Dijkstra
    {
		readonly List<Node> nodes;
		private double[] dist;
		private int[] prev;
		Dictionary<Node, int> dict;

		public Dijkstra(Graph g, Node src)
		{
			List<AuxNode<double, int>> auxNodes = new List<AuxNode<double, int>>();
			nodes = g.AllNodes;
			var pq = new PriorityQueue<AuxNode<double, int>>(nodes.Count);
			dict = Dictionarize(nodes);
			dist = new double[nodes.Count];
			prev = new int[nodes.Count];

			for (int i = 0; i != nodes.Count; ++i)
			{
				if (src != nodes[i])
				{
					dist[i] = Double.PositiveInfinity;
				}
				else
				{
					dist[i] = 0;
				}
				prev[i] = int.MinValue;
				var auxNode = new AuxNode<double, int>(dist[i], i);
				auxNodes.Add(auxNode);
				pq.Enqueue(auxNode);

			}


			while (pq.Count != 0)
			{
				var auxNode = pq.Dequeue();
				var arcs = nodes[auxNode.Item].Arcs;
				foreach (Arc arc in arcs)
				{
					var idxFrom = dict[arc.From];
					var idxTo = dict[arc.To];
					var newDist = dist[idxFrom] + arc.Weight;
					if (newDist < dist[idxTo])
					{
						dist[idxTo] = newDist;
						prev[idxTo] = idxFrom;
						var newKey = new AuxNode<double, int>(newDist, idxTo);
						pq.DecreaseKey(auxNodes[idxTo], newKey);
						auxNodes[idxTo] = newKey;
					}
				}
			}
		}

		public Node[] PathTo(Node to)
		{
			Stack<Node> path = new Stack<Node>();
			path.Push(to);
			var prevIndex = prev[dict[to]];
			while (prevIndex >= 0)
			{
				path.Push(nodes[prevIndex]);
				prevIndex = prev[prevIndex];
			}

			return path.ToArray();
		}

		private Dictionary<Node, int> Dictionarize(IList<Node> nodes)
		{
			var dict = new Dictionary<Node, int>();

			for (int i = 0; i != nodes.Count; ++i)
				dict[nodes[i]] = i;

			return dict;
		}

		#region AuxiliaryClass

		private class AuxNode<TKey, TItem> : IComparable<AuxNode<TKey, TItem>>
			//, IEquatable<AuxNode<TKey, TItem>> 
			where TKey : IComparable<TKey>
		{
			public TKey Weight { get; set; }
			public TItem Item { get; set; }

			public AuxNode(TKey weight, TItem item)
			{
				Weight = weight;
				Item = item;
			}
			
			int IComparable<AuxNode<TKey, TItem>>.CompareTo(AuxNode<TKey, TItem> other)
			{
				return Weight.CompareTo(other.Weight);
			}

			/*
			bool IEquatable<AuxNode<TKey, TItem>>.Equals(AuxNode<TKey, TItem> other)
			{
				return Weight.CompareTo(other.Weight) == 0 &&
					Item.Equals(other.Item);
			}
			*/
		}

		#endregion
	}
}
