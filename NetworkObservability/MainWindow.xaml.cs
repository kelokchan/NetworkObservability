using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using NetworkObservabilityCore;
using System.Windows.Media;
using System.Windows.Data;

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
		CanvasGraph<INode, IEdge> graph = new CanvasGraph<INode, IEdge>();

        Point startPoint, endPoint;
        CanvasNode startNode, endNode;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;

            canvasNodeButton.PreviewMouseDown += Component_MouseDown;
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
                CanvasNode selectedNode = this.elementForContextMenu as CanvasNode;
                bool startDrawing = e.Source == this.menuStartArc;

                if (startDrawing)
                {
					startNode = selectedNode;
                    startPoint = new Point(startNode.X, startNode.Y);
                    this.menuStartArc.Visibility = Visibility.Collapsed;
                    this.menuEndArc.Visibility = Visibility.Visible;
                }
                else
                {
                    endNode = selectedNode;
                    endPoint = new Point(endNode.X, endNode.Y);
                    this.menuStartArc.Visibility = Visibility.Visible;
                    this.menuEndArc.Visibility = Visibility.Collapsed;

                    CanvasEdge edge = new CanvasEdge()
                    {
                        Stroke = Brushes.DarkGray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        StrokeThickness = 2,
                        X1 = startNode.X,
                        Y1 = startNode.Y,
                        X2 = endNode.X,
                        Y2 = endNode.Y,
                        IsDirected = ArcType.SelectedItem == DirectedArc
                    };

                    graph.ConnectNodeToWith(startNode, selectedNode, edge);
                    graph.Call(graph => {
						graph.ConnectNodeToWith(startNode.nodeImpl, selectedNode.nodeImpl, edge.edgeImpl);
					});
					graph[edge.edgeImpl] = edge;

                    MainCanvas.Children.Add(edge);
                    Canvas.SetZIndex(edge, -1);

                    startNode.OutLines.Add(edge);
                    endNode.InLines.Add(edge);

                    MainCanvas.UpdateLines(startNode);
                    MainCanvas.UpdateLines(endNode);
                }
            }
        }

        private void MainCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MainCanvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.MainCanvas.ElementBeingDragged;
            else
                this.elementForContextMenu = this.MainCanvas.FindCanvasChild(e.Source as DependencyObject);
        }


        private void Component_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject();
            data.SetData("Type", sender.GetType().Name);
            DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
        }

        private void MainCanvas_Drop(object sender, DragEventArgs e)
        {
            CanvasNode node = new CanvasNode();
            Point p = e.GetPosition(MainCanvas);

            node.X = p.X;
            node.Y = p.Y;
            node.IsSelected = true;

			graph.AddNode(node);
			graph[node.nodeImpl] = node;
            (sender as Canvas).Children.Add(node);
            MainCanvas.SelectedNode = node;

            // As the component is actually a Grid, calculation is needed to obtain the center of the Component in the background
            MainCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                double widthOffset = node.ActualWidth / 2;
                double heightOffset = node.ActualHeight / 2;
                double actualX = p.X - widthOffset;
                double actualY = p.Y - heightOffset;

                Canvas.SetLeft(node, actualX);
                Canvas.SetTop(node, actualY);

                NodePanel.DataContext = node;
                return null;
            }), null);
            //Canvas.SetLeft(node, p.X);
            //Canvas.SetTop(node, p.Y);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
		{
            // Create a resultGraph instance
            ResultGraph resultGraph = new ResultGraph();

			logTab.IsSelected = true;
            logger.Content = "";
			logger.Content += "\nStart Checking observability....\n";

			var observers = graph.Call(graph => graph.AllNodes.Values.Where(node => node.IsObserver)).ToArray();

			var result = graph.Call(graph => graph.ObserveConnectivity(observers));

			logger.Content += "Observation Completed.\n";

            // This section needs to be manipulated -------------------------//
            //CanvasNode tempNode = new CanvasNode();
            //tempNode.X = 400;
            //tempNode.Y = 400;
            //resultGraph.ResultCanvas.Children.Add(tempNode);
            //---------------------------------------------------------------//

            foreach (var pair in result)
            {
                INode from = pair.Key.Item1, to = pair.Key.Item2;
                bool isObserved = pair.Value;
                logger.Content += String.Format("Node {0} to Node {1} : {2}\n", from.Id, to.Id, isObserved ? "Observed" : "Unobserved");

                CanvasNode tempNode = graph[from].Clone();
                if (tempNode == null)
                {
                    throw new Exception("null Canvas Node!");
                }
                else
                {
                    if (tempNode.X != 0 && tempNode.Y != 0)
                    {
                        Canvas.SetTop(tempNode, tempNode.Y);
                        Canvas.SetLeft(tempNode, tempNode.X);
                        resultGraph.ResultCanvas.Children.Add(tempNode);
                    }
                    else
                        throw new Exception("X and Y undefined!");
                }

            }

            logger.Content += "Task Finished.";
            // Display the resultGraph window
            resultGraph.ResultCanvas.IsEnabled = false;
            resultGraph.Show();

		}

        private void AddAttributeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.SelectedEdge == null) return;

            var attributeWindow = new AddAttributeWindow();
            if (attributeWindow.ShowDialog() == true)
            {
                EdgePanel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                TextBlock l = new TextBlock();
                l.Text = attributeWindow.Attribute + ":";
                TextBox t = new TextBox();
                t.Text = attributeWindow.Value;

                //var myBinding = new Binding();
                //myBinding.Source = MainCanvas.SelectedEdge;
                //myBinding.Mode = BindingMode.TwoWay;
                //myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                //myBinding.Path = new PropertyPath("Label");
                //t.SetBinding(TextBox.TextProperty, myBinding);

                Grid.SetRow(l, EdgePanel.Children.Count - 1);
                Grid.SetColumn(l, 0);
                Grid.SetRow(t, EdgePanel.Children.Count - 1);
                Grid.SetColumn(t, 1);
                EdgePanel.Children.Add(l);
                EdgePanel.Children.Add(t);
            }
        }


        private void ___Button___Delete__Click(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.SelectedNode != null)
            {
                NodePanel.DataContext = null;
                var node = MainCanvas.SelectedNode;
                MainCanvas.Children.Remove(node);

                foreach(CanvasEdge edge in node.InLines)
                {
                    MainCanvas.Children.Remove(edge);
                }
                foreach (CanvasEdge edge in node.OutLines)
                {
                    MainCanvas.Children.Remove(edge);
                }

                graph.DeleteNode(node);
                MainCanvas.SelectedNode = null;
            }
            else if (MainCanvas.SelectedEdge != null)
            {
                EdgePanel.DataContext = null;
                var edge = MainCanvas.SelectedEdge;
                MainCanvas.Children.Remove(edge);
            
                graph.DeleteEdge(edge);
                MainCanvas.SelectedEdge = null;
            }
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
        private void ___MenuItem___Save___Click(object sender, RoutedEventArgs e)
        {
            // TODO
            string xmlFileContents = "Some testing string";
        SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML file (*.xml) | *.xml";

            if (dialog.ShowDialog() == true)
            {
                string savePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                File.WriteAllText(dialog.FileName, xmlFileContents);
            }
        }
    }


}
