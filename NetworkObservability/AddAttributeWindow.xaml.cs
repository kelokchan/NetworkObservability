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
using System.Windows.Shapes;

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for AddAttributeWindow.xaml
    /// </summary>
    /// 
    public partial class AddAttributeWindow : Window
    {
        public string Attribute { get; set; }
        public string Value { get; set; }

        public AddAttributeWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(AddAttributeWindow_Loaded);
        }

        private void AddAttributeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            attributeTxt.Focus();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Attribute = attributeTxt.Text;
            this.Value =  valueTxt.Text;
            DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
