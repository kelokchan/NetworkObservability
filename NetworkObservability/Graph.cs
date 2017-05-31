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

                    var arc = n1.Arcs.Find(a => a.Head == n2 && a.Tail == n1);

                    if (arc != null)
                    {
                        adj[i, j] = arc;
                        AllArcs.Add(arc);
                    }
                    else
                    {
                        adj[i, j] = new Arc
                        {
                            Weigth = 99999,
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


        public List<Node> FindShortestPath(Arc[,] A, Node U)
        {
            List<Node> S = new List<Node>();
            S.Add(U);
            PriorityQueue arcs = new PriorityQueue(GetArcsOfU(U));
            int[] W = new int[AllNodes.Count];

            //shortest.Add();

            int n = A.GetLength(0);
            int temp = GetIndexOf(U);
            for (int j = 0; j < n; j++)
            {
                W[j] = A[temp, j].Weigth;
            }
            for (int i = 2; i <= n; i++)
            {
                //find smallest W[v] that is not in the S
                var v = arcs.Pop();
                if (v != null)
                {
                    S.Add(v.Head);
                    foreach (var x in AllNodes)
                    {
                        if (!S.Contains(x))
                        {
                            int temp1 = GetIndexOf(x);
                            int temp2 = GetIndexOf(v.Head);
                            if (W[temp1] > (W[temp2] + A[temp2, temp1].Weigth))
                            {
                                W[temp1] = (W[temp2] + A[temp2, temp1].Weigth);
                                arcs.Add(temp1, (W[temp2] + A[temp2, temp1].Weigth));
                            }
                        }
                    }
                }
            }


            this.costOfS = W;
            return S;
        }

        public Arc[] GetArcsOfU(Node u)
        {
            List<Arc> arcs = AllArcs;// u.Arcs;
            Arc[] output = new Arc[AllNodes.Count];
            for (int i = 0; i < AllNodes.Count; i++)
            {
                var tempArc = arcs.Find(p => p.Tail == u && p.Head == AllNodes[i]);
                if (AllNodes[i] != u)
                {
                    if (tempArc == null)
                    {
                        tempArc = new Arc
                        {
                            Tail = u,
                            Head = AllNodes[i],
                            Weigth = 99999
                        };
                    }
                    output[i] = tempArc;
                }
            }
            return output;
        }
    }
}
