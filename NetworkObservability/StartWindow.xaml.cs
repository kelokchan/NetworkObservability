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
using NetworkObservabilityCore;
using NetworkObservabilityCore.Criteria;

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private IEnumerable<IEdge> edges;
        HashSet<String> numericAttributes;
        HashSet<String> booleanAttributes;

        // A tuple of 2 elements.
        // first is a name of attribute, which is a key to be used by algorithms,
        // the second one is a constraint object.
        public Tuple<string, Constraint<IEdge>> returnValue { get; set; }


        public StartWindow()
        {
            InitializeComponent();
        }

        public StartWindow(IEnumerable<IEdge> edges)
        {
            this.edges = edges;
            numericAttributes = new HashSet<string>();
            booleanAttributes = new HashSet<string>();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            populateAttributes();
        }

        private void constraintTypeCombo_DropDownClosed(object sender, EventArgs e)
        {
            if (constraintTypeCombo.SelectedItem == rangeCombo)
            {
                constraintReader.Visibility = Visibility.Collapsed;
                constraintRangePanel.Visibility = Visibility.Visible;
            }
            else
            {
                constraintReader.Visibility = Visibility.Visible;
                constraintRangePanel.Visibility = Visibility.Collapsed;
            }
        }

        private void populateAttributes()
        {
            foreach (var edge in edges)
            {
                foreach(var attribute in edge.Attributes)
                {
                    if (attribute.Value.GetType().FullName == typeof(double).FullName)
                    {
                        numericAttributes.Add(attribute.Key);
                    }   
                    else if (attribute.Value.GetType().FullName == typeof(bool).FullName)
                    {
                        booleanAttributes.Add(attribute.Key);
                    }
                }
            }

            populateAttributePanels();
        }

        private void populateAttributePanels()
        {
            CheckBox cb;
            foreach (var numAttribute in numericAttributes)
            {
                cb = new CheckBox()
                {
                    Margin = new System.Windows.Thickness(0, 0, 0, 5)
                };
                
                cb.Content = numAttribute;
                numericalAttributesPanel.Children.Add(cb);
      
            }

            foreach (var boolAttribute in booleanAttributes)
            {
                cb = new CheckBox();

                cb.Content = boolAttribute;
                boolAttributesPanel.Children.Add(cb);

            }
        }
    }
}
