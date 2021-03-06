﻿using Microsoft.Win32;
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
using System.Diagnostics;
using NetworkObservabilityCore.Algorithms;

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

        Dictionary<string, double> edgeNumAttrList = new Dictionary<string, double>();
        Dictionary<string, string> edgeDescAttrList = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;

            canvasNodeButton.PreviewMouseDown += Component_MouseDown;          
        }

        /// <summary>
        /// Handles the Click event of both menu items in the context menu.
        /// </summary>
        void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (this.elementForContextMenu == null || this.elementForContextMenu is CanvasEdge) return;

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
                    if (selectedNode == startNode)
                    {
                        MessageBox.Show("Can't connect to the same node", "Error");
                        return;
                    }

                    endNode = selectedNode;
                    this.menuStartArc.Visibility = Visibility.Visible;
                    this.menuEndArc.Visibility = Visibility.Collapsed;

                    DrawEdge(startNode, endNode);
                }
            }
        }

        /// <summary>
        /// Draw the edge on the graph that connects 2 nodes passed, if edge param is null, create a new edge
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="edge"></param>
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

                // Add attributes to the edge
                foreach (KeyValuePair<string, double> attr in graph.CommonAttributes)
                {
                    if (!edge.Impl.HasNumericAttribute(attr.Key))
                        edge.Impl.NumericAttributes.Add(attr.Key, attr.Value);
                }

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
                edge.StrokeThickness = 3;
            }


            MainCanvas.Children.Add(edge);
            Canvas.SetZIndex(edge, -1);

            startNode.OutLines.Add(edge);
            endNode.InLines.Add(edge);

            MainCanvas.UpdateLines(startNode);
            MainCanvas.UpdateLines(endNode);
        }

        /// <summary>
        /// Draw node on the graph based on the mouse pointer location
        /// </summary>
        /// <param name="p"></param>
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

        /// <summary>
        /// Draw node on the graph based on the given node (for xml loading)
        /// </summary>
        /// <param name="node"></param>
        private void DrawNode(CanvasNode node, Canvas canvas = null)
        {

            double widthOffset = node.DisplayWidth / 2; ;
            double heightOffset = node.DisplayHeight / 2;
            double actualX = node.X - widthOffset;
            double actualY = node.Y - heightOffset;

            if (canvas != null)
            {
                canvas.Children.Add(node);
            } else
            {
                MainCanvas.Children.Add(node);
            }

            Canvas.SetLeft(node, node.X);
            Canvas.SetTop(node, node.Y);
        }

        /// <summary>
        /// Display the context menu whenever a component on the canvas is right clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MainCanvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.MainCanvas.ElementBeingDragged;
            else
                this.elementForContextMenu = this.MainCanvas.FindCanvasChild(e.Source as DependencyObject);
        }

        /// <summary>
        /// Enable drag and drop of node button to the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Component_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject();
            data.SetData("Type", sender.GetType().Name);
            DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
        }

        /// <summary>
        /// Component drop onto canvas listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_Drop(object sender, DragEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            DrawNode(p);
        }

        /// <summary>
        /// Display constraint window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // Create a resultGraph instance
            StartWindow startWindow = new StartWindow(graph.CommonAttributes);

            if (startWindow.ShowDialog() != true)
            {
				MessageBox.Show("Task imcompleted.\nAborted.", "Algorithm not running.");
            }
            else
            {
                logTab.IsSelected = true;
                logger.Content = "";
                logger.Content += "\nStart Checking observability....\n";

                var observers = graph.Call(graph => graph.AllNodes.Values.Where(node => node.IsObserver)).ToArray();

				AllPaths algorithm = new AllPaths();
                var result = new ConnectivityObserver().Observe(graph.Impl, observers, startWindow.returnValue, algorithm);

                logger.Content += "Observation Completed.\n";
                ResultGraph resultGraph = new ResultGraph();

                foreach (var pair in result)
                {
                    INode from = pair.Key.From, to = pair.Key.To;
					IEnumerable<Route> observedRoutes = pair.Value.Item1;
					IEnumerable<Route> unobservedRoutes = pair.Value.Item2;
                    if (observedRoutes.Count() > 0 )
                    {
                        Route shortestObRoute = observedRoutes.OrderBy(p => p.PathCost).First();
                        CanvasNode tempSrcNode = new CanvasNode(graph[from]);
                        CanvasNode tempDestNode = new CanvasNode(graph[to]);
						if (resultGraph.CGraph.Call(graph => !graph.Contains(from)))
						{
							resultGraph.CGraph.Call(graph => graph.Add(from));
							resultGraph.CGraph[from] = tempSrcNode;
						}
						else
						{
							tempSrcNode = resultGraph.CGraph[from];
						}

						if (resultGraph.CGraph.Call(graph => !graph.Contains(to)))
						{
							resultGraph.CGraph.Call(graph => graph.Add(to));
							resultGraph.CGraph[to] = tempDestNode;
						}
						else
						{
							tempDestNode = resultGraph.CGraph[to];
						}

                        double shortestDistance = Double.MaxValue;
                        if(unobservedRoutes.Count() > 0)
                        {
                            Route shortestUnobRoute = unobservedRoutes.OrderBy(p => p.PathCost).First();
                            shortestDistance = shortestUnobRoute.PathCost;
                        }

                        if ((from.IsVisible && to.IsVisible) && shortestObRoute.PathCost < shortestDistance)
                        {
                            //resultGraph.logger.Content += String.Format("\nNode {0} to Node {1} : observed\n", from.Id, to.Id);
                            //logger.Content += String.Format("The path from Node {0} to Node {1} is : {2}\n", from.Id, to.Id, shortestObRoute);
							resultGraph.observedLog.Content += String.Format("{0} (This is the shortest path)\n", shortestObRoute);
							AddOutputEdge(resultGraph, tempSrcNode, tempDestNode, unobservedRoutes.Count() == 0);
                        } else
						{
							resultGraph.observedLog.Content += String.Format("{0}\n", shortestObRoute);
						}
					}

					//               foreach (Route through in observedRoutes)
					//{
					//	logger.Content += String.Format("Node {0} to Node {1} : observed\n", from.Id, to.Id);
					//	logger.Content += String.Format("The path from Node {0} to Node {1} is : {2}\n", from.Id, to.Id, through);

					//CanvasNode tempSrcNode = new CanvasNode(graph[from]);
					//CanvasNode tempDestNode = new CanvasNode(graph[to]);

					//DrawNode(tempSrcNode, resultGraph.ResultCanvas);
					//DrawNode(tempDestNode, resultGraph.ResultCanvas);
					//                  if (from.IsVisible && to.IsVisible)
					//                  {
					//                      DrawOutputEdge(resultGraph, tempSrcNode, tempDestNode);
					//                  }
					//}

					foreach (Route through in unobservedRoutes)
					{
				//		logger.Content += String.Format("\nNode {0} to Node {1} : not observed\n", from.Id, to.Id);
						resultGraph.unobservedLog.Content += String.Format("{0}\n", through);
					}
                }

				foreach (var node in resultGraph.CGraph.Call(graph => graph.AllNodes.Values))
				{
					var cnode = resultGraph.CGraph[node];
					DrawNode(cnode, resultGraph.ResultCanvas);
				}
				foreach (var edge in resultGraph.CGraph.Call(graph => graph.AllEdges.Values))
				{
					var cedge = resultGraph.CGraph[edge];

					DrawOutputEdge(resultGraph, cedge);
				}


                logger.Content += "Task Finished.";
                // Display the resultGraph window
                resultGraph.Show();
            }

		}

        /// <summary>
        /// Add edge to the graph, avoid duplicates
        /// </summary>
        /// <param name="cgraph"></param>
        /// <param name="cnode"></param>
        private void AddIfNotContain(CanvasGraph cgraph, CanvasNode cnode)
		{
			INode resultNode = cnode.Impl;
			//if (resultNode.GetType() != typeof(ResultNode))
			//	throw new Exception("AddIfNotContain Error!");
			
			if (cgraph.Call(graph => !graph.Contains(resultNode)))
				cgraph.Call(graph => graph.Add(resultNode));
		}

        /// <summary>
        /// Draw result edge onto the result graph
        /// </summary>
        /// <param name="resultGraph"></param>
        /// <param name="canvasEdge"></param>
		private void DrawOutputEdge(ResultGraph resultGraph, CanvasEdge canvasEdge)
		{
			resultGraph.ResultCanvas.Children.Add(canvasEdge);
			Canvas.SetZIndex(canvasEdge, -1);

			CanvasNode tempSrcNode = resultGraph.CGraph[canvasEdge.Impl.From];
			CanvasNode tempDestNode = resultGraph.CGraph[canvasEdge.Impl.To];
			tempSrcNode.OutLines.Add(canvasEdge);
			tempDestNode.InLines.Add(canvasEdge);

			resultGraph.ResultCanvas.UpdateLines(tempSrcNode);
			resultGraph.ResultCanvas.UpdateLines(tempDestNode);
		}

        /// <summary>
        /// Add result edge onto the result graph
        /// </summary>
        /// <param name="resultGraph"></param>
        /// <param name="srcNode"></param>
        /// <param name="destNode"></param>
        /// <param name="fullyObserved"></param>
		private void AddOutputEdge(ResultGraph resultGraph, CanvasNode srcNode, CanvasNode destNode, bool fullyObserved)
		{
			CanvasEdge edge = new CanvasEdge(isResult: true)
			{
				Stroke = fullyObserved ? Brushes.Green : Brushes.DarkOrange,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				StrokeThickness = 3,
				X1 = srcNode.X,
				Y1 = srcNode.Y,
				X2 = destNode.X,
				Y2 = destNode.Y,
				IsDirected = ArcType.SelectedItem == DirectedArc,
			};

			resultGraph.CGraph.Call(g =>
			{
				g.ConnectNodeToWith(srcNode.Impl, destNode.Impl, edge.Impl);
			});
			resultGraph.CGraph[edge.Impl] = edge;
		}

        /// <summary>
        /// Display the attributes on the side panel when a canvas edge is selected
        /// </summary>
        /// <param name="edge"></param>
        public void PopulateAttributesPanel(CanvasEdge edge)
        {
            edgeNumAttrList.Clear();
            edgeDescAttrList.Clear();

            foreach (var a in edge.Impl.NumericAttributes)
            {
                edgeNumAttrList[a.Key] = a.Value;
            }

            foreach (var a in edge.Impl.DescriptiveAttributes)
            {
                edgeDescAttrList[a.Key] = a.Value;
            }

            NumericAttrList.ItemsSource = null;
            NumericAttrList.ItemsSource = edgeNumAttrList;

            DescAttrList.ItemsSource = null;
            DescAttrList.ItemsSource = edgeDescAttrList;
        }

        /// <summary>
        /// Display attribute window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAttributeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.SelectedEdge == null) return;

            var attributeWindow = new AddAttributeWindow();

            if (attributeWindow.ShowDialog() == true)
            {
                String attributeName = attributeWindow.Attribute;
                String attributeValue = attributeWindow.Value;
                bool applyToAll = attributeWindow.ApplyAll;

                double numValue;

                if (applyToAll)
                {
                    foreach (var edge in graph.Impl.AllEdges.Values)
                    {
                        if (attributeWindow.numRadio.IsChecked == true || attributeWindow.boolRadio.IsChecked == true)
                        {
                            numValue = Double.TryParse(attributeValue, out numValue) ? numValue : 0.0;
                            if (!edge.HasNumericAttribute(attributeName))
                                edge[attributeName] = numValue;

                            graph.CommonAttributes[attributeName] = numValue;
                        }
                        else
                        {
                            if (!edge.HasDescriptiveAttribute(attributeName))
                                edge.DescriptiveAttributes[attributeName] = attributeValue;
                        }
                    }
                }
                else
                {
                    if (attributeWindow.numRadio.IsChecked == true || attributeWindow.boolRadio.IsChecked == true)
                    {
                        numValue = Double.TryParse(attributeValue, out numValue) ? numValue : 0.0;
                        if (!MainCanvas.SelectedEdge.Impl.HasNumericAttribute(attributeName))
                            MainCanvas.SelectedEdge.Impl[attributeName] = numValue;
                    }
                    else
                    {
                        if (!MainCanvas.SelectedEdge.Impl.HasDescriptiveAttribute(attributeName))
                            MainCanvas.SelectedEdge.Impl.DescriptiveAttributes[attributeName] = attributeValue;
                    }
                }

                PopulateAttributesPanel(MainCanvas.SelectedEdge);
            }
        }

        /// <summary>
        /// Delete selected component from the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                var edge = MainCanvas.SelectedEdge;
                MainCanvas.Children.Remove(edge);

                graph.Remove(edge);
                MainCanvas.SelectedEdge = null;
            }
        }

        /// <summary>
        /// Allow user to load xml file. User is prompted to save unsaved graph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!MainCanvas.IsEmpty())
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
            else
            {
                OpenFromFile();
            }
        }

        /// <summary>
        /// XML Save listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ___MenuItem___Save___Click(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        /// <summary>
        /// Save the graph onto xml file
        /// </summary>
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

        /// <summary>
        /// Delete edge numeric attribute 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumAttributeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string key = ((Button)sender).Tag as string;

            MessageBoxResult result = MessageBox.Show("Delete this attribute from all other edges?", null, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                foreach(var edge in graph.Impl.AllEdges.Values)
                {
                    edge.NumericAttributes.Remove(key);
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MainCanvas.SelectedEdge.Impl.NumericAttributes.Remove(key);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                // do nothing
            }

            graph.CommonAttributes.Remove(key);

            PopulateAttributesPanel(MainCanvas.SelectedEdge);
        }

        /// <summary>
        /// Clear the graph if not empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuReset_Click(object sender, RoutedEventArgs e)
        {
            if (!MainCanvas.IsEmpty())
            {
                MessageBoxResult result = MessageBox.Show("Would you like to save the current graph before openning a new one?", "Current graph is not saved!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveToFile();
                }
                else if (result == MessageBoxResult.No)
                {
                    MainCanvas.Children.Clear();
                    graph = new CanvasGraph();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // do nothing
                }
            }
            else
            {
                MainCanvas.Children.Clear();
                graph = new CanvasGraph();
            }
        }

        /// <summary>
        /// Delete edge descriptive attribute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescAttributeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string key = ((Button)sender).Tag as string;

            MessageBoxResult result = MessageBox.Show("Delete this attribute from all other edges?", null, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                foreach (var edge in graph.Impl.AllEdges.Values)
                {
                    edge.DescriptiveAttributes.Remove(key);
                }
            }
            else if (result == MessageBoxResult.No)
            {
                MainCanvas.SelectedEdge.Impl.DescriptiveAttributes.Remove(key);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                // do nothing
            }

            PopulateAttributesPanel(MainCanvas.SelectedEdge);
        }

        /// <summary>
        /// Edit edit numeric attribute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumAttributeEditButton_Click(object sender, RoutedEventArgs e)
        {
            var pair = (KeyValuePair<string, double>) ((Button)sender).Tag;
            string key = pair.Key;
            double prevValue = pair.Value;

            var EditAttributeWindow = new EditAttributeWindow()
            {
                Attribute = key,
                Value = prevValue.ToString(),
                IsNumeric = true
            };

            if (EditAttributeWindow.ShowDialog() == true)
            {
                double newValue = Double.TryParse(EditAttributeWindow.Value, out newValue) ? newValue : prevValue;

                if (EditAttributeWindow.ApplyAll)
                {
                    foreach (var edge in graph.Impl.AllEdges.Values)
                    {
                        edge.NumericAttributes[key] = newValue;
                    }
                }
                else
                {
                    this.MainCanvas.SelectedEdge.Impl.NumericAttributes[key] = newValue;
                }

                PopulateAttributesPanel(this.MainCanvas.SelectedEdge);
            }
        }

        /// <summary>
        /// Edit edit descriptive attribute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescAttributeEditButton_Click(object sender, RoutedEventArgs e)
        {
            var pair = (KeyValuePair<string, string>)((Button)sender).Tag;
            string key = pair.Key;
            string prevValue = pair.Value;

            var EditAttributeWindow = new EditAttributeWindow()
            {
                Attribute = key,
                Value = prevValue.ToString(),
                IsNumeric = false
            };

            if (EditAttributeWindow.ShowDialog() == true)
            {
                if (EditAttributeWindow.ApplyAll)
                {
                    foreach (var edge in graph.Impl.AllEdges.Values)
                    {
                        edge.DescriptiveAttributes[key] = EditAttributeWindow.Value;
                    }
                }
                else
                {
                    this.MainCanvas.SelectedEdge.Impl.DescriptiveAttributes[key] = EditAttributeWindow.Value;
                }
                PopulateAttributesPanel(this.MainCanvas.SelectedEdge);
            }
        }

        /// <summary>
        /// Open and load graph from xml
        /// </summary>
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
                    MainCanvas.Children.Clear();
                    graph = reader.Load((fileDialog.FileName).ToString());
                    foreach(var node in graph.Impl.AllNodes.Values)
                    {
                        DrawNode(graph[node]);
                    }
                    foreach (var edge in graph.Impl.AllEdges.Values)
                    {
                        CanvasNode fromNode = graph[edge.From];
                        CanvasNode toNode = graph[edge.To];

                        DrawEdge(fromNode, toNode, graph[edge]);
                    }

                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Failed to load the invalid XML file.\nPlease use a valid one");
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        /// <summary>
        /// Terminate the application when main window is closed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!MainCanvas.IsEmpty())
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Current graph will be lost if you haven't saved it yet.", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    base.OnClosing(e);
                    Application.Current.Shutdown();
                }
                else
                {
                    e.Cancel = true;
                }// end else/if
            }

           

        }
    }


}
