using System;
using System.Collections.Generic;
using NetworkObservabilityCore;

namespace NetworkObservability
{
	internal class ResultNode : INode
	{
		private static int r_id = 0;

		internal ResultNode(INode original)
		{
			Id = original.Id;
			Label = original.Label;
			ConnectIn = new List<IEdge>(original.ConnectIn);
			ConnectOut = new List<IEdge>(original.ConnectOut);
			IsObserver = original.IsObserver;
			IsObserverInclusive = original.IsObserverInclusive;
			IsVisible = original.IsVisible;
			IsBlocked = original.IsBlocked;
			NumericAttributes = new Dictionary<String, Double>(original.NumericAttributes);
			DescriptiveAttributes = new Dictionary<String, String>(original.DescriptiveAttributes);
		}

		internal ResultNode()
		{
			Id = Label = string.Format("RN{0}", r_id++);
			ConnectIn = new List<IEdge>();
			ConnectOut = new List<IEdge>();
			IsVisible = true;
			NumericAttributes = new Dictionary<String, Double>();
			DescriptiveAttributes = new Dictionary<String, String>();
		}

		public string Id
		{
			get;
			set;
		}

		public string Label
		{
			get;
			set;
		}

		public List<IEdge> ConnectOut
		{
			get;
			set;
		}

		public List<IEdge> ConnectIn
		{
			get;
			set;
		}

		public bool IsObserver
		{
			get;
			set;
		}

		public bool IsObserverInclusive
		{
			get;
			set;
		}

		public bool IsVisible
		{
			get;
			set;
		}

		public bool IsBlocked
		{
			get;
			set;
		}

		public IDictionary<string, double> NumericAttributes
		{
			get;
			set;
		}

		public IDictionary<string, string> DescriptiveAttributes
		{
			get;
			set;
		}

		public double this[string key]
		{
			get
			{
				return NumericAttributes[key];
			}

			set
			{
				NumericAttributes[key] = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is INode && Equals(obj as INode);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public bool Equals(INode other)
		{
			return Id == other.Id;
		}

		public bool HasDescriptiveAttribute(string name)
		{
			return DescriptiveAttributes.ContainsKey(name);
		}

		public bool HasNumericAttribute(string name)
		{
			return NumericAttributes.ContainsKey(name);
		}
	}
}
