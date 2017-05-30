using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using NetworkObservability.resources;
using System.Windows.Threading;

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Stores a reference to the UIElement which the Canvas's context menu currently targets.
        /// </summary>
        private UIElement elementForContextMenu;
        public static MainWindow AppWindow;
        List<Node> nodeList = new List<Node>();
        Point startPoint, endPoint;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;

            visibleObserver.PreviewMouseDown += Component_MouseDown;
            invisibleObserver.PreviewMouseDown += Component_MouseDown;
            visibleNode.PreviewMouseDown += Component_MouseDown;
            invisibleNode.PreviewMouseDown += Component_MouseDown;
            endNode.PreviewMouseDown += Component_MouseDown;
            customNode.PreviewMouseDown += Component_MouseDown;
        }

        void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
           
        }

        /// <summary>
        /// Handles the Click event of both menu items in the context menu.
        /// </summary>
        void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (this.elementForContextMenu == null)
                return;

            if (e.Source == this.menuItemBringToFront ||
                e.Source == this.menuItemSendToBack)
            {
                bool bringToFront = e.Source == this.menuItemBringToFront;

                if (bringToFront)
                    this.MainCanvas.BringToFront(this.elementForContextMenu);
                else
                    this.MainCanvas.SendToBack(this.elementForContextMenu);
            }

            if (e.Source == this.menuStartArc || e.Source == this.menuEndArc)
            {
                Node selectedNode = this.elementForContextMenu as Node;
                bool startDrawing = e.Source == this.menuStartArc;

                if (startDrawing)
                {
                    startPoint = new Point(selectedNode.X, selectedNode.Y);
                    this.menuStartArc.Visibility = Visibility.Collapsed;
                    this.menuEndArc.Visibility = Visibility.Visible;
                }
                else
                {
                    endPoint = new Point(selectedNode.X, selectedNode.Y);
                    this.menuStartArc.Visibility = Visibility.Visible;
                    this.menuEndArc.Visibility = Visibility.Collapsed;

                    // Test Arc draw
                    Line arc = new Line()
                    {
                        Stroke = System.Windows.Media.Brushes.DarkGray,
                        X1 = startPoint.X,
                        X2 = endPoint.X,
                        Y1 = startPoint.Y,
                        Y2 = endPoint.Y,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        StrokeThickness = 2
                    };
                    MainCanvas.Children.Add(arc);
                }
            }
        }

        private void MainCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MainCanvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.MainCanvas.ElementBeingDragged;
            else
                this.elementForContextMenu =
                    this.MainCanvas.FindCanvasChild(e.Source as DependencyObject);
        }


        private void Component_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject();
            data.SetData("Type", sender.GetType().Name);
            DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
        }

        private void MainCanvas_Drop(object sender, DragEventArgs e)
        {
            Node node;
            Point p = e.GetPosition(MainCanvas);

            switch (e.Data.GetData("Type"))
            {
                case "VisibleObserver":
                    node = new VisibleObserver();
                    break;
                case "InvisibleObserver":
                    node = new InvisibleObserver();       
                    break;
                case "VisibleNode":
                    node = new VisibleNode();
                    break;
                case "InvisibleNode":
                    node = new InvisibleNode();
                    break;
                case "EndNode":
                    node = new EndNode();
                    break;
                case "CustomNode":
                    node = new CustomNode();
                    break;
                default:
                    throw new Exception("Invalid node type");
            }

            node.Label = "Node " + (int) Node.counter;
            node.ID = (int) Node.counter;
            node.X = p.X;
            node.Y = p.Y;
            ((Canvas)sender).Children.Add(node);

            // As the component is actually a Grid, calculation is needed to obtain the center of the Component in the background
            MainCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                double widthOffset = node.ActualWidth / 2;
                double heightOffset = node.ActualHeight / 2;
                double actualX = p.X - widthOffset;
                double actualY = p.Y - heightOffset;



                Canvas.SetLeft(node, actualX);
                Canvas.SetTop(node, actualY);

                // Set autofocus to right panel
                PropertiesPanel.DataContext = node;
                PropertiesPanel.Focus();

                return null;
            }), null);
            //Canvas.SetLeft(node, p.X);
            //Canvas.SetTop(node, p.Y);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            //Open file from anywhere
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "XML Files(*.xml)|*.xml|Textfiles(*.txt)|*.txt|All Files(*.*)|*.*";
            fileDialog.DefaultExt = ".xml";
            Nullable<bool> getFile = fileDialog.ShowDialog();

            if (getFile == true)
            {
                //TODO 
                Stream contents = fileDialog.OpenFile();
            }
        }
    }


}
