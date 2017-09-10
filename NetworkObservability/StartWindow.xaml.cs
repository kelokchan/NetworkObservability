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

namespace NetworkObservability
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        // A tuple of 2 elements.
        // first is a name of attribute, which is a key to be used by algorithms,
        // the second one is a constraint object.
        Tuple<string, Constraint<IEdge>> returnValue;

        public StartWindow()
        {
            InitializeComponent();
        }
    }
}
