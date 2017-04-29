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

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject(DataFormats.Serializable, "Hi");

            DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Copy);

        }

        private void Label_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            String test = (String)e.Data.GetData(DataFormats.Serializable);
            Point p = Mouse.GetPosition(this);

            Canvas.SetLeft(canvas, p.X);
            Canvas.SetTop(canvas, p.Y);


           // canvas.Children.Add(test);
        }
    }


}
