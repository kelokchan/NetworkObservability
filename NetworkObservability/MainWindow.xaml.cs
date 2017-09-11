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
using NetworkObservabilityCore.Utils;

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
        CanvasGraph graph = new CanvasGraph();
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

        private void DrawEdge(CanvasNode startNode, CanvasNode endNode, CanvasEdge edge = null)
        {
            Point startPoint, endPoint;
            startPoint = new Point(startNode.X, startNode.Y);
            endPoint = new Point(endNode.X, endNode.Y);

            if (edge == null)
            {
                edge = new CanvasEdge()
                {
                    Stroke = Brushes.DarkGray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    StrokeThickness = 3,
                    X1 = startNode.X,
                    Y1 = startNode.Y,
                    X2 = endNode.X,
                    Y2 = endNode.Y,
                    IsDirected = ArcType.SelectedItem == DirectedArc
                };

                graph.Call(graph =>
                {
                    graph.ConnectNodeToWith(startNode.Impl, endNode.Impl, edge.Impl);
                });
                graph[edge.Impl] = edge;
            } else
            {
                edge.Stroke = Brushes.DarkGray;
                 edge.HorizontalAlignment = HorizontalAlignment.Center;
                edge.VerticalAlignment = VerticalAlignment.Center;
            }


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

            graph.Call(graph => graph.Add(node.Impl));
            graph[node.Impl] = node;
            MainCanvas.Children.Add(node);
            MainCanvas.SelectedNode = node;

            // As the component is actually a Grid, calculation is needed to obtain the center of the Component in the background
            MainCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                double widthOffset = node.ActualWidth / 2;
                double heightOffset = node.ActualHeight / 2;
                double actualX = p.X - widthOffset;
                double actualY = p.Y - heightOffset;

                node.DisplayWidth = node.ActualWidth;
                node.DisplayHeight = node.ActualHeight;
                
                Canvas.SetLeft(node, actualX);
                Canvas.SetTop(node, actualY);

                NodePanel.DataContext = node;
                return null;
            }), null);
        }

        private void DrawNode(CanvasNode node)
        {


            double widthOffset = node.DisplayWidth / 2;// ;
            double heightOffset = node.DisplayHeight / 2; // node.ActualHeight / 2;
            double actualX = node.X - widthOffset;
            double actualY = node.Y - heightOffset;

            MainCanvas.Children.Add(node);
            Canvas.SetLeft(node, actualX); //actualX);
            Canvas.SetTop(node, actualY);// actualY);

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
            StartWindow startWindow = new StartWindow();

            logTab.IsSelected = true;
            logger.Content = "";
            logger.Content += "\nStart Checking observability....\n";

            var observers = graph.Call(graph => graph.AllNodes.Values.Where(node => node.IsObserver)).ToArray();

            var result = new ConnectivityObserver().ObserveConnectivity(graph.Impl, observers, startWindow.returnValue);

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
            CanvasEdge tempEdge = new CanvasEdge(true)
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
                graph.ConnectNodeToWith(tempSrcNode.Impl, tempDestNode.Impl, tempEdge.Impl);
            });
            graph[tempEdge.Impl] = tempEdge;

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
                String attributeName = attributeWindow.Attribute;
                String attributeValue = attributeWindow.Value;

                bool boolValue;
                double numValue;

                RowDefinition rd = new RowDefinition() { Height = GridLength.Auto };
                EdgePanel.RowDefinitions.Add(rd);
                int rowIndex = EdgePanel.RowDefinitions.IndexOf(rd);

                TextBlock attributeTxtBlock = new TextBlock();
                attributeTxtBlock.Text = attributeName + ":";
                EdgePanel.Children.Add(attributeTxtBlock);
                Grid.SetRow(attributeTxtBlock, rowIndex);
                Grid.SetColumn(attributeTxtBlock, 0);

                TextBox valueTxtBox = new TextBox();
                EdgePanel.Children.Add(valueTxtBox);
                Grid.SetRow(valueTxtBox, rowIndex);
                Grid.SetColumn(valueTxtBox, 1);

                if (attributeWindow.boolRadio.IsChecked == true)
                {
                    boolValue = Boolean.TryParse(attributeValue, out boolValue) ? boolValue : false;
                    MainCanvas.SelectedEdge.Impl.Attributes[attributeName] = boolValue;
                    valueTxtBox.Text = boolValue.ToString();
                }
                else if (attributeWindow.numRadio.IsChecked == true)
                {
                    numValue = Double.TryParse(attributeValue, out numValue) ? numValue : 0.0;
                    MainCanvas.SelectedEdge.Impl.Attributes[attributeName] = numValue;
                    valueTxtBox.Text = numValue.ToString();
                }
                else
                {
                    MainCanvas.SelectedEdge.Impl.Attributes[attributeName] = attributeValue;
                    valueTxtBox.Text = attributeValue;
                }
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

                graph.Remove(node);
                MainCanvas.SelectedNode = null;
            }
            else if (MainCanvas.SelectedEdge != null)
            {
                EdgePanel.DataContext = null;
                EdgePanel.Children.Clear();
                var edge = MainCanvas.SelectedEdge;
                MainCanvas.Children.Remove(edge);

                graph.Remove(edge);
                MainCanvas.SelectedEdge = null;
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (graph != null)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to save the current graph before openning a new one?", "Current graph is not saved!",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveToFile();
                    graph.Clear();
                    MainCanvas.Children.Clear();
                    OpenFromFile();
                }
                else if (result == MessageBoxResult.No)
                {
                    OpenFromFile();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // do nothing
                }
            }
        }


        private void ___MenuItem___Save___Click(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }



        private void SaveToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML file (*.xml) | *.xml";

            if (dialog.ShowDialog() == true)
            {
                CanvasGraphXML output = new CanvasGraphXML();
                output.Save(dialog.FileName, graph);
            }
        }

        private void OpenFromFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "XML Files(*.xml)|*.xml|Textfiles(*.txt)|*.txt|All Files(*.*)|*.*";
            fileDialog.DefaultExt = ".xml";
            Nullable<bool> getFile = fileDialog.ShowDialog();

            if (getFile == true)
            {
                CanvasGraphXML reader = new CanvasGraphXML();
                //CanvasGraph cGraph = new CanvasGraph();
                try
                {
                    graph = reader.Load((fileDialog.FileName).ToString());
                    foreach(var node in graph.Impl.AllNodes.Values)
                    {
                        CanvasNode tempNode = graph[node];

                        DrawNode(tempNode);
                    }
                    foreach (var edge in graph.Impl.AllEdges.Values)
                    {
                        CanvasNode fromNode = graph[edge.From];
                        CanvasNode toNode = graph[edge.To];

                        DrawEdge(fromNode, toNode, graph[edge]);
                    }

                }
                catch (ArgumentNullException err)
                {
                    MessageBox.Show("Failed to load the invalid XML file.\nPlease use a valid one");
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
    }


}
