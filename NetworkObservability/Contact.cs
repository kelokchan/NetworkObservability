using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    public class Contact
    {
        public Contact(string firstname, string lastname)
        {
            Firstname = firstname;
            Lastname = lastname;
        }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public override string ToString()
        {
            return string.Format("Firstname: {0}, Lastname: {1}",Firstname, Lastname);
        }
    }
}
