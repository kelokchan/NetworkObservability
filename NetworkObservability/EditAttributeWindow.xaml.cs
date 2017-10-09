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
    /// Interaction logic for EditAttributeWindow.xaml
    /// </summary>
    public partial class EditAttributeWindow : Window
    {
        public string Attribute { get; set; }

        public bool IsNumeric { get; set; }

        public string Value { get; set; }

        public bool ApplyAll { get; private set; }

        public EditAttributeWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(EditAttributeWindow_Loaded);
        }

        private void EditAttributeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            attributeTxt.Text = Attribute;
            valueTxt.Text = Value;
            valueTxt.Focus();
        }

        private void valueTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsNumeric)
            {
                double parsedValue;

                if (valueTxt.Text.Length != 0 && !double.TryParse(valueTxt.Text, out parsedValue))
                {
                    valueTxt.Text = "";
                }
            }
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Value = valueTxt.Text.ToString();
            this.ApplyAll = applyAllCheckBox.IsChecked.Value;
            DialogResult = true;
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
