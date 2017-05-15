using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
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
                    this.canvas.BringToFront(this.elementForContextMenu);
                else
                    this.canvas.SendToBack(this.elementForContextMenu);
            }
        }

        private void canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // If the user right-clicks while dragging an element, assume that they want 
            // to manipulate the z-index of the element being dragged (even if it is  
            // behind another element at the time).
            if (this.canvas.ElementBeingDragged != null)
                this.elementForContextMenu = this.canvas.ElementBeingDragged;
            else
                this.elementForContextMenu =
                    this.canvas.FindCanvasChild(e.Source as DependencyObject);
        }

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop((DependencyObject)sender, ((Image)sender).Source, DragDropEffects.Copy);
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
            foreach (var format in e.Data.GetFormats())
            {
                ImageSource imageSource = e.Data.GetData(format) as ImageSource;
                if (imageSource != null)
                {
                    Image img = new Image();
                    img.Source = imageSource;

                    Point p = e.GetPosition(canvas);

                    Node node = new Node();
                    node.Height = 50;
                    node.Width = 50;
                    node.X = p.X;
                    node.Y = p.Y;
                    node.Source = imageSource;

                    ((Canvas)sender).Children.Add(node);

                    node.Label = "Node_" + nodeList.Count;
                    node.ID = nodeList.Count;

                    nodeList.Add(node);

                    Canvas.SetLeft(node, p.X);
                    Canvas.SetTop(node, p.Y);
                }
            }
        }
    }


}
