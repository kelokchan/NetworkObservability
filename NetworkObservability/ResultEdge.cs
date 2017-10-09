using System;
using System.Collections.Generic;
using NetworkObservabilityCore;

namespace NetworkObservability
{
    internal class ResultEdge : IEdge
    {
		private static int r_id = 0;

		internal ResultEdge(IEdge original)
		{
			Id = original.Id;
			Label = original.Label;
			From = original.From;
			To = original.To;
			IsBlocked = original.IsBlocked;
			NumericAttributes = new Dictionary<String, Double>(original.NumericAttributes);
			DescriptiveAttributes = new Dictionary<String, String>(original.DescriptiveAttributes);
		}

		internal ResultEdge()
		{
			Id = Label = string.Format("RE{0}", r_id++);
			NumericAttributes = new Dictionary<String, Double>();
			DescriptiveAttributes = new Dictionary<String, String>();
		}

        public double this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Id { get; set; }

        public string Label { get; set; }
        public INode From { get; set; }
        public INode To { get; set; }
        public bool IsBlocked { get; set; }
        public IDictionary<string, double> NumericAttributes { get; set; }
        public IDictionary<string, string> DescriptiveAttributes { get; set; }

        public bool Equals(IEdge other)
        {
			return Id == other.Id;
        }

		public override bool Equals(object other)
		{
			return other is IEdge && Equals(other as IEdge);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
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