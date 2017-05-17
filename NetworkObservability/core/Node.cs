using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    namespace Core
    {
        public class Node
        {
            private readonly String id;
            private static UInt32 iDCounter = 0;
            public const int DEFAULT_CAPACITY = 2;
            private HashSet<Arc> connections;
            private UInt32 availableInterfaces;

            // Constructors

            public Node(UInt32 capacity = DEFAULT_CAPACITY)
            {
                connections = new HashSet<Arc>();
                availableInterfaces = capacity;
                id = String.Format("NODE{0:X8}", iDCounter++);
                Name = String.Copy(id);
            }

            public Node(Arc[] adjacencies, UInt32 capacity = DEFAULT_CAPACITY)
            {
                connections = new HashSet<Arc>(adjacencies);
                availableInterfaces = capacity;
                id = String.Format("NODE{0:X8}", iDCounter++);
                Name = String.Copy(id);
            }

            // Properties

            public String Name
            {
                get; set;
            }

            public IEnumerable<Arc> GetAdjacencies
            {
                get
                {

                    return connections.Where(arc => arc.AnotherEnd(this) != null);
                }
            }

            public String ID
            {
                get { return id; }
            }

            /// <summary>
            /// Get total current connections of this node.
            /// </summary>
            public int ConnectedInterfaces
            {
                get { return connections.Count; }
            }

            // public member methods

            // <summary>
            // This function is called by Arc.attachTo
            // </summary>
            /// <exception cref="OutOFNodeInterfacesException">Thrown when Node is running out of interfaces.</exception>
            public void AddArc(Arc arc)
            {
                if (availableInterfaces != 0)
                {
                    connections.Add(arc);
                    --availableInterfaces;
                }
                else
                {
                    throw new OutOFNodeInterfacesException();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public void LinkTo(Node node, int type = 0) // this can be overload in derivatives for selecting certain type of arc
            {
                var arc = new Arc(this, node);
                if (availableInterfaces != 0 && node.availableInterfaces != 0)
                {
                    AddArc(arc);
                    node.AddArc(arc);
                }
                else
                {
                    throw new OutOFNodeInterfacesException();
                }
            }

            /// <summary>
            /// This method is used to remove connection with arc.
            /// 
            /// </summary>
            /// <param name="arc"></param>
            public void Disconnect(Arc arc)
            {
                connections.Remove(arc);
            }

            // Equatable overriden mehtods

            /// <summary>
            /// This method overrides the Equals method from <see cref="Node"/>.
            /// </summary>
            /// <param name="node"><see cref="Node"/></param>
            /// <returns>hash</returns>
            public override bool Equals(object node)
            {
                return (node as Node).ID.Equals(this.ID);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        /// <summary>
        /// This is a comparer of Node regarding to the Nodename
        /// </summary>
        class NodeNameComparer : IEqualityComparer<Node>
        {
            public bool Equals(Node x, Node y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(Node node)
            {
                return node.Name.GetHashCode();
            }
        }
    }
}
