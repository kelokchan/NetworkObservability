using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    public class BindingCriteria
    {
        public BindingCriteria(string attribute, int selectedIndex = 0, double value1 = 0, double value2 = 0)
        {
            Attribute = attribute;
        }

        public string Attribute { get; set; }

        public int SelectedIndex { get; set; }

        public double Value1 { get; set; }

        public double Value2 { get; set; }
    }
}
