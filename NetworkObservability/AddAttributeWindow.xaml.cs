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
        public bool ApplyAll { get; set; }

        public AddAttributeWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(AddAttributeWindow_Loaded);
            numRadio.Checked += RadioButton_Checked;
            boolRadio.Checked += RadioButton_Checked;
            txtRadio.Checked += RadioButton_Checked;
        }

        private void AddAttributeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            attributeTxt.Focus();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Attribute = attributeTxt.Text;
            if (boolRadio.IsChecked == true)
            {
                this.Value = valueBool.IsChecked == true ? "1" : "0";
            }
            else
            {
                this.Value = valueTxt.Text.ToString();
            }
            this.ApplyAll = applyAllCheckBox.IsChecked.Value;
            DialogResult = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.Content.ToString().Equals("Boolean"))
            {
                valueBool.Visibility = Visibility.Visible;
                valueTxt.Visibility = Visibility.Hidden;
            }
            else
            {
                valueBool.Visibility = Visibility.Hidden;
                valueTxt.Visibility = Visibility.Visible;
            }
        }

        private void valueTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (numRadio.IsChecked == true)
            {
                double parsedValue;

                if (!double.TryParse(valueTxt.Text, out parsedValue))
                {
                    valueTxt.Text = "";
                }
            }
        }
    }
}
