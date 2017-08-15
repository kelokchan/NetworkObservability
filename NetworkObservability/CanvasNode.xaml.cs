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

        public bool isInSet { get; set; }
        private bool isSelected;
        private double x;
        private double y;

        // Connections out from the current node
        public List<CanvasEdge> OutLines { get; private set; }
        // Connection into the current node
        public List<CanvasEdge> InLines { get; private set; }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

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

        public bool IsNodeVisible
        {
            get
            {
                return nodeImpl.IsVisible;
            }
            set
            {
                nodeImpl.IsVisible = value;
                OnPropertyChanged("IsNodeVisible");
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

        public CanvasNode() : base()
        {
            OutLines = new List<CanvasEdge>();
            InLines = new List<CanvasEdge>();
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

        public CanvasNode Clone()
        {
            CanvasNode copy = new CanvasNode()
            {
                isInSet = isInSet,
                Label = Label,
                X = X,
                Y = Y,
                nodeImpl = nodeImpl,
                IsNodeVisible = IsNodeVisible,
                IsObserver = IsObserver,
            };
            return copy;
        }

        protected void OnPropertyChanged(string key)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
        }
    }
}
