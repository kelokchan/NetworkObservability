using System;
using System.Collections.Generic;
using NetworkObservabilityCore;

namespace NetworkObservability
{
    internal class ResultEdge : IEdge
    {
        public IComparable this[string key] {
            get { return Attributes[key]; }
            set { Attributes[key] = value; }
        }

        public string Id { get; set; }

        public string Label { get; set; }
        public INode From { get; set; }
        public INode To { get; set; }
        public bool IsBlocked { get; set; }
        public double Weight { get; set; }
        public IDictionary<string, IComparable> Attributes { get; set; }

        public bool Equals(IEdge other)
        {
            throw new NotImplementedException();
        }

        public bool HasAttribute(string name)
        {
            throw new NotImplementedException();
        }
    }
}