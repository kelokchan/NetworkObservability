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

		public CanvasNode() : base()
		{
			Impl = new Node();
		}

		public CanvasNode(CanvasNode canvasNode)
			: base()
		{
			Impl = new ResultNode(canvasNode.Impl);
		}

        internal INode Impl
		{
			get;
			set;
		}

        public bool isInSet { get; set; }
        private bool isSelected;
        private double x;
        private double y;

        public double DisplayWidth { get; set; }
        public double DisplayHeight { get; set; }

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
                return Impl.IsObserver;
            }
            set
            {
                Impl.IsObserver = value;
                OnPropertyChanged("IsObserver");
            }
        }

        public bool IsNodeVisible
        {
            get
            {
                return Impl.IsVisible;
            }
            set
            {
                Impl.IsVisible = value;
                OnPropertyChanged("IsNodeVisible");
            }
        }

        public string Label
        {
            get
            {
                return Impl.Label;
            }
            set
            {
                Impl.Label = value;
                OnPropertyChanged("Label");
            }
        }

        public String ID
        {
            get
            {
                return Impl.Id;
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

        public CanvasNode(INode node = null) : base()
        {
            OutLines = new List<CanvasEdge>();
            InLines = new List<CanvasEdge>();
            Impl = node == null ? new Node() : node;
            InitializeComponent();
        }

        public void Call(Action<INode> func)
        {
            func(Impl);
        }

        public T Call<T>(Func<INode, T> func)
        {
            return func(Impl);
        }

        public override string ToString()
        {
            return Impl.ToString();
        }

        public CanvasNode Clone()
        {
            CanvasNode copy = new CanvasNode()
            {
                isInSet = isInSet,
                Label = Label,
                X = X,
                Y = Y,
                Impl = Impl,
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
