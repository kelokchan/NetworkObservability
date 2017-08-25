using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObservabilityCore;
using System.Windows.Shapes;

namespace NetworkObservability
{
	class CanvasGraph<NType, EType> where NType : INode where EType : IEdge
	{
		private Graph graph;
		private Dictionary<INode, CanvasNode> nodeToCNode;
		private Dictionary<EType, CanvasEdge> edgeToCEdge;

		public CanvasGraph()
		{
			graph = new Graph();
			nodeToCNode = new Dictionary<INode, CanvasNode>();
			edgeToCEdge = new Dictionary<EType, CanvasEdge>();
		}

		public T Call<T>(Func<Graph, T> func)
		{
			return func(graph);
		}

		public void Call(Action<Graph> func)
		{
			func(graph);
		}

		public void AddNode(CanvasNode node)
		{
			graph.Add(node.nodeImpl);
		}

        public void DeleteNode(CanvasNode node)
        {
            nodeToCNode.Remove(node.nodeImpl);
            graph.Remove(node.nodeImpl);
        }

        public void DeleteEdge(CanvasEdge edge)
        {
            edgeToCEdge.Remove((EType) edge.edgeImpl);

            INode from = edge.edgeImpl.From;
            nodeToCNode[from].OutLines.Remove(edge);
            from.Links.Remove(edge.edgeImpl);

            INode to = edge.edgeImpl.To;
            to.Links.Remove(edge.edgeImpl);
            nodeToCNode[to].InLines.Remove(edge);

            graph.Remove(edge.edgeImpl);
        }

        public CanvasEdge this[IEdge edge]
		{
			get
			{
				return edgeToCEdge[(EType) edge];
			}
			set
			{
				edgeToCEdge[(EType) edge] = value;
			}
		}

		public CanvasNode this[INode node]
		{
			get
			{
				return nodeToCNode[node];
			}
			set
			{
				nodeToCNode[node] = value;
			}
		}

        internal void ConnectNodeToWith(CanvasNode startNode, CanvasNode endNode, CanvasEdge edge)
        {
            graph.ConnectNodeToWith(startNode.nodeImpl, endNode.nodeImpl, edge.edgeImpl);
        }
    }
}
