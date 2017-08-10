using NetworkObservabilityCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for CanvasNode.xaml
    /// </summary>
    public partial class CanvasNode : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal Node nodeImpl;

        // to accomodate the first 6 nodes populated on the Components tab
        public static Int64 counter = -6;
        public bool isInSet { get; set; }
        private string label;
        private int id;
        private double x;
        private double y;
        private bool isObserver = false;

        public bool IsObserver
        {
            get
            {
                return nodeImpl.IsObserver;
            }
            set
            {
                nodeImpl.IsObserver = value;
                OnPropertyChanged("IsObserver");
            }
        }

        public string Label
        {
            get
            {
                return nodeImpl.Label;
            }
            set
            {
                nodeImpl.Label = value;
                OnPropertyChanged("Label");
            }
        }

        public String ID
        {
            get
            {
                return nodeImpl.Id;
            }
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

        public CanvasNode()
        {
            nodeImpl = new Node();
            InitializeComponent();
        }

        public void Call(Action<INode> func)
        {
            func(nodeImpl);
        }

        public T Call<T>(Func<INode, T> func)
        {
            return func(nodeImpl);
        }

        public override string ToString()
        {
            return nodeImpl.ToString();
        }

        protected void OnPropertyChanged(string key)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
        }
    }
}
