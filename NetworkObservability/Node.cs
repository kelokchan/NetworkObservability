using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetworkObservability
{
    public class Node : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // to accomodate the first 6 nodes populated on the Components tab
        public static Int64 counter = -6;
        public List<Arc> Arcs = new List<Arc>();
        private List<Node> Adjacency = new List<Node>();
        public bool isInSet { get; set; }
        private string label;
        private int id;
        private double x;
        private double y;

        public Node()
        {
            counter++;
        }

        public string Label
        {
            get { return label; }
            set
            { label = value; OnPropertyChanged("Label"); }
        }

        public int ID
        {
            get { return id; }
            set { id = value; OnPropertyChanged("ID"); }
        }

        public double X
        {
            get { return x; }
            set { x = value; OnPropertyChanged("X"); }
        }

        public double Y
        {
            get { return y; }
            set { y = value; OnPropertyChanged("Y"); }
        }

        /// <summary>
        /// Create a new arc, connecting this Node to the Nod passed in the parameter
        /// Also, it creates the inversed node in the passed node
        /// </summary>
        public Node AddArc(Node child, int w)
        {
            Arcs.Add(new Arc
            {
                Tail = this,
                Head = child,
                Weigth = w,
                Distance = 1
            });

            //if (!child.Arcs.Exists(a => a.Tail == child && a.Head == this))
            //{
            //    //child.AddArc(this, w);
            Adjacency.Add(child);
            //}
            return this;
        }


        public List<Node> GetAdjacents()
        {
            return this.Adjacency;
        }

        public Arc GetArc(Node a)
        {
            Arc connectedArc = Arcs.Find(q => q.Tail == this && q.Head == a);
            if (connectedArc == null)
            {
                return new Arc
                {
                    Tail = a,
                    Head = this,
                    Weigth = 0,// int.MaxValue,
                    Distance = int.MaxValue
                };
            }
            return connectedArc;
        }

        public override string ToString()
        {
            return String.Format("{0}", Label);
        }

        protected void OnPropertyChanged(string key)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
        }
    }
}
