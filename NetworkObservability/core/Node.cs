using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability.core
{
    class Node
    {
        private String ID;
        private static UInt32 iDCounter = 0;
        public const int DEFAULT_CAPACITY = 2;
        private List<Arc> connections;
        private int availableInterfaces;

        public Node(int capacity = DEFAULT_CAPACITY)
        {
            connections = new List<Arc>();
            this.availableInterfaces = capacity;
            this.ID = String.Format("NODE{0:X8}",  iDCounter++);
        }

        public Node(Arc[] adjacencies, Nullable<int> capacity = null)
        {
            connections = new List<Arc>(adjacencies);
            availableInterfaces = capacity ?? DEFAULT_CAPACITY;
        }

        public Arc[] GetAdjacencies
        {
            get
            {
                return connections.ToArray();
            }
        }

        public void addArc(Arc arc)
        {
            if (availableInterfaces != 0) {
                connections.Add(arc);
                --availableInterfaces;
            } else {
                throw new OutOFNodeInterfaces();
            }
        }

        public Arc linkTo(Node node, int type = 0) // this can be overload in derivatives for selecting certain type of arc
        {
            var arc = new Arc(this, node);
            if (availableInterfaces != 0 && node.availableInterfaces != 0) {
                --availableInterfaces;
                --node.availableInterfaces;
                return arc;
            } else {
                return null;
            }
        }

        public void disconnect(Arc arc)
        {
            connections.Remove(arc);
            ++this.availableInterfaces;
        }

        public int GetNumberInterfaces()
        {
            return connections.Count;
        }

        public override bool Equals(object node)
        {
            return ((Node)node).ID.Equals(this.ID);
        }
    }
}
