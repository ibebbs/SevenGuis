using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CircleDrawer
{
    public class CircleDescriptorToCanvasCoordinatesConverter : IValueConverter
    {
        private object Convert(Circle circle, object parameter)
        {
            switch (parameter)
            {
                case string text when text.ToLower() == "left": return circle.Origin.X - (circle.Diameter / 2);
                case string text when text.ToLower() == "top": return circle.Origin.Y - (circle.Diameter / 2);
                default: return DependencyProperty.UnsetValue;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case Circle circle: return Convert(circle, parameter);
                default: return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
