using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObservabilityCore;
using System.Xml.Linq;
using System.Reflection;
using NetworkObservabilityCore.Utils;

namespace NetworkObservability
{
	class CanvasGraphXML : NetworkObservabilityCore.Xml.GraphXML
	{
		private CanvasGraph cgraph;

		#region Constructors
		public CanvasGraphXML() : base()
		{
		}

		#endregion

		public void Save(String path, CanvasGraph graph)
		{
			cgraph = graph;
			XElement root = new XElement("NetworkObservability");
			DumpTo(graph.Call(graphImpl => graphImpl), ref root);
			File.Add(root);

			// New way to save
			File.Save(path);
		}

		protected override XElement CreateXElement(INode node)
		{
			XElement xelement = base.CreateXElement(node);
			CanvasNode cnode = cgraph[node];
			var position = new XElement("Position", new XElement("X", cnode.X),
													new XElement("Y", cnode.Y));
            var width = new XElement("Width", cnode.DisplayWidth);
            var height = new XElement("Height", cnode.DisplayHeight);

			xelement.Add(position);
            xelement.Add(width);
            xelement.Add(height);

			return xelement;
		}

		protected override XElement CreateXElement(IEdge edge)
		{
			XElement xelement = base.CreateXElement(edge);
			CanvasEdge cedge = cgraph[edge];
			var position = new XElement("Position", new XElement("X1", cedge.X1),
													new XElement("Y1", cedge.Y1),
													new XElement("X2", cedge.X2),
													new XElement("Y2", cedge.Y2));
			xelement.Add(position);

			return xelement;
		}

		public CanvasGraph Load(String path)
		{
			File = XDocument.Load(path);
			cgraph = new CanvasGraph();
			cgraph.Impl = Dump(File.Root);

			return cgraph;
		}

		protected override INode LoadNode(XElement xnode)
		{
			INode node =  base.LoadNode(xnode);
			var position = xnode.Element("Position");
			CanvasNode cnode = new CanvasNode(node);
			cnode.X = Convert.ToDouble(position.Element("X").Value);
			cnode.Y = Convert.ToDouble(position.Element("Y").Value);
            cnode.DisplayWidth = Convert.ToDouble(xnode.Element("Width").Value);
            cnode.DisplayHeight = Convert.ToDouble(xnode.Element("Height").Value);

            cgraph[node] = cnode;

			return node;
		}

		protected override FromToThrough<string, string, IEdge> LoadEdge(XElement xedge)
		{
			var tuple = base.LoadEdge(xedge);
			var position = xedge.Element("Position");
			CanvasEdge cedge = new CanvasEdge()
			{
				X1 = Convert.ToDouble(position.Element("X1").Value),
				Y1 = Convert.ToDouble(position.Element("Y1").Value),
				X2 = Convert.ToDouble(position.Element("X2").Value),
				Y2 = Convert.ToDouble(position.Element("Y2").Value),
				Impl = tuple.Through
			};
			cgraph[tuple.Through] = cedge;

			return tuple;
		}

	}
}
