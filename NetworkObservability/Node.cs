using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetworkObservability
{
    public class Node : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _Label;
        private int _ID;
        private double _X;
        private double _Y;

        protected Node()
        {
            
        }

        public string Label
        {
            get { return _Label; }
            set
            { _Label = value; OnPropertyChanged("Label"); }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }

        public double X
        {
            get { return _X; }
            set { _X = value; OnPropertyChanged("X"); }
        }

        public double Y
        {
            get { return _Y; }
            set { _Y = value; OnPropertyChanged("Y"); }
        }

        protected void OnPropertyChanged(string key)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
        }
    }
}
