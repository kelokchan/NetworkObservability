using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    class Graph
    {
        public Node Root;
        public List<Node> AllNodes = new List<Node>();
        public List<Arc> AllArcs = new List<Arc>();
        public int[] costOfS;


        public Node CreateRoot()
        {
            Root = CreateNode();
            return Root;
        }

		public void AddNode(Node node)
		{
			AllNodes.Add(node);
		}

        public Node CreateNode()
        {
            var n = new Node();
            AllNodes.Add(n);
            return n;
        }
        public Arc[,] GetAdjMatrix()
        {
            return this.CreateAdjMatrix();
        }


        // Create the basic data structure for the graph
        public Arc[,] CreateAdjMatrix()
        {
            Arc[,] adj = new Arc[AllNodes.Count, AllNodes.Count];

            for (int i = 0; i < AllNodes.Count; i++)
            {
                Node n1 = AllNodes[i];
                for (int j = 0; j < AllNodes.Count; j++)
                {
                    Node n2 = AllNodes[j];

                    var arc = n1.Arcs.Find(a => a.To == n2 && a.From == n1);

                    if (arc != null)
                    {
                        adj[i, j] = arc;
                        AllArcs.Add(arc);
                    }
                    else
                    {
                        adj[i, j] = new Arc
                        {
                            Weight = 99999,
                            Distance = 99999
                        };
                    }
                }
            }
            return adj;
        }

        public int GetIndexOf(Node u)
        {
            return AllNodes.IndexOf(u);
        }

		public Dictionary<Tuple<Node, Node>, bool> ObserveConnectivity(List<Node> observers)
		{
			var result = new Dictionary<Tuple<Node, Node>, bool>();
			var adjMatrix = GetAdjMatrix();

			foreach (var from in AllNodes)
			{
				if (observers.Contains(from))
					continue;

				Dijkstra dijkstra = new Dijkstra(this, from);

				foreach (var to in AllNodes)
				{

					foreach (var observer in observers)
					{
						bool flag = dijkstra.PathTo(to).Contains(observer);
						result[new Tuple<Node, Node>(from, to)] = flag;
					}
				}
			}

			return result;
		}


        public Arc[] GetArcsOfU(Node u)
        {
            List<Arc> arcs = AllArcs;// u.Arcs;
            Arc[] output = new Arc[AllNodes.Count];
            for (int i = 0; i < AllNodes.Count; i++)
            {
                var tempArc = arcs.Find(p => p.From == u && p.To == AllNodes[i]);
                if (AllNodes[i] != u)
                {
                    if (tempArc == null)
                    {
                        tempArc = new Arc
                        {
                            From = u,
                            To = AllNodes[i],
                            Weight = 99999
                        };
                    }
                    output[i] = tempArc;
                }
            }
            return output;
        }
    }
}
