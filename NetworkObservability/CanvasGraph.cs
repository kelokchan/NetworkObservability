using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObservabilityCore;
using System.Windows.Shapes;

namespace NetworkObservability
{
	class CanvasGraph
	{
		private Dictionary<INode, CanvasNode> nodeToCNode;
		private Dictionary<IEdge, CanvasEdge> edgeToCEdge;

        public HashSet<string> CommonAttributes { get; set; }

        #region Constructors
        public CanvasGraph()
			: this(new Graph())
		{
		}

		public CanvasGraph(IGraph graph)
		{
			Impl = graph;
			nodeToCNode = new Dictionary<INode, CanvasNode>();
			edgeToCEdge = new Dictionary<IEdge, CanvasEdge>();
            CommonAttributes = new HashSet<string>();
        }
		#endregion

		public T Call<T>(Func<IGraph, T> func)
		{
			return func(Impl);
		}

		public void Call(Action<IGraph> func)
		{
			func(Impl);
		}

        public void Remove(CanvasNode node)
        {
            nodeToCNode.Remove(node.Impl);
            Impl.Remove(node.Impl);
        }

        public void Clear()
        {
            nodeToCNode.Clear();
            edgeToCEdge.Clear();
            Impl = new Graph();
        }

        public void Remove(CanvasEdge edge)
        {
            edgeToCEdge.Remove(edge.Impl);

            INode from = edge.Impl.From;
            nodeToCNode[from].OutLines.Remove(edge);
            from.ConnectOut.Remove(edge.Impl);

            INode to = edge.Impl.To;
            to.ConnectOut.Remove(edge.Impl);
            nodeToCNode[to].InLines.Remove(edge);

            Impl.Remove(edge.Impl);
        }

		internal IGraph Impl
		{
			get;
			set;
		}

        public CanvasEdge this[IEdge edge]
		{
			get
			{
				return edgeToCEdge[edge];
			}
			set
			{
				edgeToCEdge[edge] = value;
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
