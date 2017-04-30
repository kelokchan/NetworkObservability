using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability.core
{
    class Arc
    {
        private Node[] nodePoints;
        private static UInt32 iDCounter = 0;
        public static int WEIGHT = 1;
        private String ID;

        public Arc(Node a, Node b)
        {
            nodePoints = new Node[2];
            this.ID = String.Format("ARC{0:X8}", iDCounter++);
            nodePoints[0] = a;
            nodePoints[1] = b;
        }

        public Arc()
        {
            nodePoints = null;
        }

        public bool attachTo(Node a, Node b)
        {
            try {
                if (nodePoints == null)
                {
                    nodePoints = new Node[2];
                    nodePoints[0] = a;
                    a.addArc(this);
                    nodePoints[1] = b;
                    b.addArc(this);
                }
                nodePoints[0] = a;
                a.addArc(this);
                nodePoints[1] = b;
                b.addArc(this);
                return true;
            } catch(Exception e) {
                /// TODO
                /// handling exceptiion
                /// by sending a message to message queue and 
                /// the frontend should display the message
                return false;
            }

        }

        public void detach()
        {
            nodePoints[0].disconnect(this);
            nodePoints[1].disconnect(this);
        }

        public override bool Equals(object arc)
        {
            return ((Arc)arc).ID.Equals(this.ID);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }

}
