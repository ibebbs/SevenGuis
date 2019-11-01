using System;
using System.Collections.Generic;
#if __WPF__
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace CircleDrawer
{
    public static class Extensions
    {
#if __WPF__        
        private static IEnumerable<System.Drawing.Point> GetAreaPoint(MouseButtonEventArgs args)
        {
            switch (args.OriginalSource)
            {
                case Control control:
                    var location = args.GetPosition(control);
                    yield return new System.Drawing.Point(Convert.ToInt32(location.X), Convert.ToInt32(location.Y));
                    break;
            }
        }
#else
        public static IEnumerable<System.Drawing.Point> GetTappedPoint(this TappedRoutedEventArgs args)
        {
            switch (args.OriginalSource)
            {
                case Canvas canvas:
                    var canvasLocation = args.GetPosition(canvas);
                    yield return new System.Drawing.Point(Convert.ToInt32(canvasLocation.X), Convert.ToInt32(canvasLocation.Y));
                    break;
                case ListView listView:
                    var itemsPanel = listView.ItemsPanelRoot;
                    var listViewLocation = args.GetPosition(itemsPanel);
                    yield return new System.Drawing.Point(Convert.ToInt32(listViewLocation.X), Convert.ToInt32(listViewLocation.Y));
                    break;
            }
        }
#endif
    }
}
