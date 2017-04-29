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

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new List<DateTime>
            {
                DateTime.Now,
                new DateTime(2013, 02, 13),
                new DateTime(2004, 12, 31)
            };
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
            else
            {
                bool canBeDragged = DragCanvas.GetCanBeDragged(this.elementForContextMenu);
                DragCanvas.SetCanBeDragged(this.elementForContextMenu, !canBeDragged);
                (e.Source as MenuItem).IsChecked = !canBeDragged;
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
                    img.Height = 50;
                    img.Width = 50;
                    img.Source = imageSource;

                    ((Canvas)sender).Children.Add(img);

                    Point p = e.GetPosition(canvas);
                    
                    Canvas.SetLeft(img, p.X);
                    Canvas.SetTop(img, p.Y);
                }
            }
        }
    }


}
