using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetworkObservability
{
    class Node : Image
    {
        public Node(string name, int id)
        {
            this.Name = name;
            this.ID = id;
        }

        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
