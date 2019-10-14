#if __WPF__
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace CircleDrawer
{
    public static class CanvasBindingHelper
    {
        private static readonly CircleDescriptorToCanvasCoordinatesConverter CircleOriginToCanvasCoordinatesConverter = new CircleDescriptorToCanvasCoordinatesConverter();

        private static void EnableCanvasPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindingOperations.SetBinding(d, Canvas.LeftProperty, new Binding { Path = new PropertyPath(nameof(Circle.Descriptor)), Converter = CircleOriginToCanvasCoordinatesConverter, ConverterParameter = "Left" });
            BindingOperations.SetBinding(d, Canvas.TopProperty, new Binding { Path = new PropertyPath(nameof(Circle.Descriptor)), Converter = CircleOriginToCanvasCoordinatesConverter, ConverterParameter = "Top" });
        }

        public static readonly DependencyProperty EnableCanvasPositioningProperty = DependencyProperty.RegisterAttached("EnableCanvasPositioning", typeof(bool), typeof(CanvasBindingHelper), new PropertyMetadata(false, EnableCanvasPositioningChanged));

        public static bool GetEnableCanvasPositioning(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableCanvasPositioningProperty);
        }

        public static void SetEnableCanvasPositioning(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableCanvasPositioningProperty, value);
        }
    }
}
