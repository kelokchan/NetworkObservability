using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    public class Arc
    {
        public int Weigth { get; set; }
        public Node Tail { get; set; }
        public Node Head { get; set; }
        public bool flagged { get; set; } = false;
        public int Distance { get; set; }
        private static int id = 0;
        private int arcId;
        public Arc()
        {
            arcId = id;
            id++;
        }
        public int GetId()
        {
            return arcId;
        }

        public override string ToString()
        {
            return Weigth.ToString();
        }
    }
}
