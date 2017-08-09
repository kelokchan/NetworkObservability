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
		private Dictionary<Line, EType> lineToEdge;

		public CanvasGraph()
		{
			graph = new Graph();
			nodeToCNode = new Dictionary<INode, CanvasNode>();
			lineToEdge = new Dictionary<Line, EType>();
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

		public EType this[Line line]
		{
			get
			{
				return lineToEdge[line];
			}
			set
			{
				lineToEdge[line] = value;
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
	}
}
