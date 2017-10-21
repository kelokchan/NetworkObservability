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
using System.Collections.ObjectModel;

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        //HashSet<String> CommonAttributes;
        Dictionary<string, double> CommonAttributes;

        // A tuple of 2 elements.
        // first is a name of attribute, which is a key to be used by algorithms,
        // the second one is a constraint object.
        public Tuple<string, Constraint<IEdge>> returnValue { get; set; }

        public ObservableCollection<CheckedListItem<BindingCriteria>> BindingCriterion { get; set; }

        private BindingCriteria _selectedCriterion;
        public BindingCriteria SelectionCriterion
        {
            get { return _selectedCriterion; }
            set
            {
                _selectedCriterion = value;               
            }
        }

        public StartWindow()
        {
            InitializeComponent();
        }   

        public StartWindow(Dictionary<string, double> CommonAttributes)
        {
            InitializeComponent();

            this.CommonAttributes = CommonAttributes;
            BindingCriterion = new ObservableCollection<CheckedListItem<BindingCriteria>>();
            foreach(KeyValuePair<string, double> attribute in this.CommonAttributes)
            {
                BindingCriteria bc = new BindingCriteria(attribute.Key);
                BindingCriterion.Add(new CheckedListItem<BindingCriteria>(bc));
            }

            this.weightCombo.ItemsSource = CommonAttributes;

            DataContext = this;
        }

        private void constraintTypeCombo_DropDownClosed(object sender, EventArgs e)
        {
            ToggleInputVisiblity(constraintTypeCombo.SelectedIndex);
        }

        private void ToggleInputVisiblity(int index)
        {
            if (index == 3)
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

        private void CommonAttrList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionCriterion = ((CheckedListItem<BindingCriteria>) CommonAttrList.SelectedItem).Item;
            this.EditPanel.DataContext = SelectionCriterion;
            ToggleInputVisiblity(SelectionCriterion.SelectedIndex);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            HashSet<ICriterion> criteria = new HashSet<ICriterion>();
            foreach(CheckedListItem<BindingCriteria> checkbox in CommonAttrList.Items)
            {
                if (checkbox.IsChecked)
                {
                    BindingCriteria criterion = checkbox.Item;

                    switch (criterion.SelectedIndex)
                    {
                        case 0:
                            criteria.Add(new GreaterThanCriterion(criterion.Attribute, criterion.Value1));
                            break;
                        case 1:
                            criteria.Add(new LessThanCriterion(criterion.Attribute, criterion.Value1));
                            break;
                        case 2:
                            criteria.Add(new EqualCriterion(criterion.Attribute, criterion.Value1));
                            break;
                        case 3:
                            criteria.Add(new RangeCriterion(criterion.Attribute, criterion.Value1, criterion.Value2));
                            break;
                    }
                }
            }

            Constraint<IEdge> constraint = new Constraint<IEdge>(criteria);
            returnValue = Tuple.Create(weightCombo.SelectedItem.ToString(), constraint);
            DialogResult = true;
        }
    }
}
