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
using System.Collections.Generic;

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
                    this.menuStartArc.Visibility = Visibility.Collapsed;
                    this.menuEndArc.Visibility = Visibility.Visible;
                }
                else
                {
                    endNode = selectedNode;
                    this.menuStartArc.Visibility = Visibility.Visible;
                    this.menuEndArc.Visibility = Visibility.Collapsed;

                    DrawEdge(startNode, endNode);
                }
            }
        }

        private void DrawEdge(CanvasNode startNode, CanvasNode endNode)
        {
            Point startPoint, endPoint;
            startPoint = new Point(startNode.X, startNode.Y);
            endPoint = new Point(endNode.X, endNode.Y);

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

            graph.Call(graph => {
                graph.ConnectNodeToWith(startNode.nodeImpl, endNode.nodeImpl, edge.edgeImpl);
            });
            graph[edge.edgeImpl] = edge;

            MainCanvas.Children.Add(edge);
            Canvas.SetZIndex(edge, -1);

            startNode.OutLines.Add(edge);
            endNode.InLines.Add(edge);

            MainCanvas.UpdateLines(startNode);
            MainCanvas.UpdateLines(endNode);
        }

        private void DrawNode(Point p)
        {
            CanvasNode node = new CanvasNode()
            {
                X = p.X,
                Y = p.Y,
                IsSelected = true
            };

            graph.Call(graph => graph.Add(node.nodeImpl));
            graph[node.nodeImpl] = node;
            MainCanvas.Children.Add(node);
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
            Point p = e.GetPosition(MainCanvas);
            DrawNode(p);
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
            
            foreach (var pair in result)
            {
                INode from = pair.Key.Item1, to = pair.Key.Item2;
                Route route = pair.Key.Item3;
                bool isObserved = pair.Value;
                logger.Content += String.Format("Node {0} to Node {1} : {2}\n", from.Id, to.Id, isObserved ? "Observed" : "Unobserved");

                CanvasNode tempSrcNode = graph[from].Clone();
                CanvasNode tempDestNode = graph[to].Clone();
                if (tempSrcNode == null)
                {
                    throw new Exception("null Canvas Node!");
                }
                else
                {
                    if (tempSrcNode.X != 0 && tempSrcNode.Y != 0)
                    {
                        Canvas.SetTop(tempSrcNode, tempSrcNode.Y);
                        Canvas.SetLeft(tempSrcNode, tempSrcNode.X);
                        resultGraph.ResultCanvas.Children.Add(tempSrcNode);

                    }
                    else
                        throw new Exception("X and Y undefined!");
                }
                DrawOutputEdge(resultGraph, tempSrcNode, tempDestNode);
            }

            logger.Content += "Task Finished.";
            // Display the resultGraph window
            resultGraph.ResultCanvas.IsEnabled = false;
            resultGraph.Show();

		}

        private void DrawOutputEdge(ResultGraph resultGraph, CanvasNode tempSrcNode, CanvasNode tempDestNode)
        {
            CanvasEdge tempEdge = new CanvasEdge()
            {
                Stroke = Brushes.Blue,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                StrokeThickness = 3,
                X1 = tempSrcNode.X,
                Y1 = tempSrcNode.Y,
                X2 = tempDestNode.X,
                Y2 = tempDestNode.Y,
                IsDirected = ArcType.SelectedItem == DirectedArc
            };

            graph.Call(graph =>
            {
                graph.ConnectNodeToWith(tempSrcNode.nodeImpl, tempDestNode.nodeImpl, tempEdge.edgeImpl);
            });
            graph[tempEdge.edgeImpl] = tempEdge;

            resultGraph.ResultCanvas.Children.Add(tempEdge);
            Canvas.SetZIndex(tempEdge, -1);

            tempSrcNode.OutLines.Add(tempEdge);
            tempDestNode.InLines.Add(tempEdge);

            resultGraph.ResultCanvas.UpdateLines(tempSrcNode);
            resultGraph.ResultCanvas.UpdateLines(tempDestNode);
        }



        private void AddAttributeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.SelectedEdge == null) return;

            var attributeWindow = new AddAttributeWindow();
            if (attributeWindow.ShowDialog() == true)
            {
                RowDefinition rd = new RowDefinition() { Height = GridLength.Auto };
                EdgePanel.RowDefinitions.Add(rd);
                int rowIndex = EdgePanel.RowDefinitions.IndexOf(rd);

                TextBlock l = new TextBlock();
                l.Text = attributeWindow.Attribute + ":";
                EdgePanel.Children.Add(l);
                Grid.SetRow(l, rowIndex);
                Grid.SetColumn(l, 0);

                TextBox t = new TextBox();
                t.Text = attributeWindow.Value;
                EdgePanel.Children.Add(t);
                Grid.SetRow(t, rowIndex);
                Grid.SetColumn(t, 1);

                //var myBinding = new Binding();
                //myBinding.Source = MainCanvas.SelectedEdge;
                //myBinding.Mode = BindingMode.TwoWay;
                //myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                //myBinding.Path = new PropertyPath("Label");
                //t.SetBinding(TextBox.TextProperty, myBinding);

            }
        }

        private void ___Button___Delete__Click(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.SelectedNode != null)
            {
                NodePanel.DataContext = null;
                var node = MainCanvas.SelectedNode;
                MainCanvas.Children.Remove(node);

                foreach (CanvasEdge edge in node.InLines)
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
                EdgePanel.Children.Clear();
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
                CanvasGraphXML reader = new CanvasGraphXML();
                reader.Read(fileDialog.FileName);// as Graph;
                // Manipulate the local variables of this class
            }
        }
        private void ___MenuItem___Save___Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //string xmlFileContents = "Some testing string";
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML file (*.xml) | *.xml";

            if (dialog.ShowDialog() == true)
            {
                CanvasGraphXML output = new CanvasGraphXML();
                string savePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                // Save to file
                //output.Save(savePath, graph);
            }
        }
    }


}
