using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    namespace Core
    {
        public class Arc
        {
			protected bool blockLeftFlow, blockRightFlow;
            protected Node leftNode, rightNode;

            private readonly String id;
            private static UInt32 iDCounter = 0;
            public static int WEIGHT = 1;


            // Constructors

            public Arc(Node a, Node b)
            {
                id = String.Format("ARC{0:X8}", iDCounter++);
                Name = String.Copy(id);
				leftNode = a;
				rightNode = b;
				Blocked = false;
            }

            public Arc()
            {
				leftNode = null; rightNode = null;
                id = String.Format("ARC{0:X8}", iDCounter++);
                Name = String.Copy(id);
				Blocked = false;
            }

            // Properties

            public String ID
            {
                get { return id; }
            }

            public String Name
            {
                get; set;
            }

            public IList<Node> Nodes
            {
                get
                {
                    return Array.AsReadOnly(new Node[] { leftNode, rightNode });
                }
            }

            // Public member methods

			/// <summary>
			/// This method will link <see cref="Node"/> a and <see cref="Node"/> b with this <see cref="Arc"/>.
			/// </summary>
			/// <param name="a"><see cref="Node"/>a</param>
			/// <param name="b"><see cref="Node"/>b</param>
			/// <exception cref="OutOFNodeInterfacesException"></exception>
            public void AttachTo(Node a, Node b, bool blockLeft, bool blockRight, bool blocked)
            {
				Blocked = blocked;
				if (leftNode == null && rightNode == null)
				{
					leftNode = a;
					leftNode.AddArc(this);
					blockLeftFlow = blockLeft;
					rightNode = b;
					rightNode.AddArc(this);
					blockRightFlow = blockRight;
				}
				else
				{
					leftNode.Disconnect(this);
					leftNode = a;
					leftNode.AddArc(this);
					rightNode.Disconnect(this);
					rightNode = b;
					rightNode.AddArc(this);
				}
            }

            public void Detach()
            {
                leftNode.Disconnect(this);
                rightNode.Disconnect(this);
				leftNode = rightNode = null;
            }

            public Node AnotherEnd(Node node)
            {
				if (!Blocked)
				{
					if (leftNode.Equals(node))
					{
						return blockLeftFlow ? null : rightNode;
					}
					else
					{
						return blockRightFlow ? null : leftNode;
					}
				}
				else
				{
					return null;
				}
            }

			public bool Blocked
			{
				get;
				set;
			}

            // Equatable overriden methods

            public override bool Equals(object arc)
            {
                return (arc as Arc).id.Equals(ID);
            }

            public override int GetHashCode()
            {
                return this.id.GetHashCode();
            }
        }
    }

}
