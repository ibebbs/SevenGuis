using System.ComponentModel;
using System.Drawing;

namespace CircleDrawer
{
    [Bindable(BindableSupport.Default)]
    public class Circle : INotifyPropertyChanged
    {
        private Point _origin;
        private int _diameter;

        public event PropertyChangedEventHandler PropertyChanged;

        public Circle() { }

        public Point Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Origin)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Descriptor)));
            }
        }

        public int Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Diameter)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Descriptor)));
            }
        }

        public Circle Descriptor => this;
    }
}
