using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for ResultGraph.xaml
    /// </summary>
    public partial class ResultGraph : Window
    {
        private string xmlFileContents = "Some testing string";

        public ResultGraph()//I might pass the content to the constructor
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML file (*.xml) | *.xml";

            if(dialog.ShowDialog() == true)
            {
                string savePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                File.WriteAllText(dialog.FileName, xmlFileContents);
            }
        }
    }
}
