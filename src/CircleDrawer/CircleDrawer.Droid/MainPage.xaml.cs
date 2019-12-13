using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CircleDrawer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int AndroidAppBarHeight = 50;

        public static IEnumerable<System.Drawing.Point> GetAreaPoint(TappedRoutedEventArgs args)
        {
            if (args.OriginalSource is UIElement element)
            {
                var location = args.GetPosition(null);
                yield return new System.Drawing.Point(Convert.ToInt32(location.X), Convert.ToInt32(location.Y));
            }
        }

        private readonly MainPageViewModel _viewModel;
        private readonly IObservable<System.Drawing.Point> _emptyAreaClicked;
        private readonly IObservable<Unit> _adjustDiameterDialogClosed;
        private readonly IObserver<Unit> _showAdjustDiameterDialog;

        public MainPage()
        {
            InitializeComponent();

            _viewModel = new MainPageViewModel();

            DataContext = _viewModel;

            _emptyAreaClicked = Observable
                .FromEventPattern<TappedEventHandler, TappedRoutedEventArgs>(handler => CirclesContainer.Tapped += handler, handler => CirclesContainer.Tapped -= handler)
                .SelectMany(pattern => GetAreaPoint(pattern.EventArgs));

            _adjustDiameterDialogClosed = Observable
                .FromEventPattern<object>(handler => AdjustDiameterDialog.Closed += handler, handler => AdjustDiameterDialog.Closed -= handler)
                .Select(_ => Unit.Default);

            _showAdjustDiameterDialog = Observer.Create<Unit>(_ => ShowAdjustDiameterDialog());
        }

        private void ShowAdjustDiameterDialog()
        {
            if (!AdjustDiameterDialog.IsOpen)
            {
                AdjustDiameterGrid.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;
                AdjustDiameterGrid.Height = 100;
                AdjustDiameterDialog.VerticalOffset = ApplicationView.GetForCurrentView().VisibleBounds.Height - (AdjustDiameterGrid.Height) - AndroidAppBarHeight;
                AdjustDiameterDialog.IsOpen = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel.Activate(_emptyAreaClicked, _showAdjustDiameterDialog, _adjustDiameterDialogClosed);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _viewModel.Deactivate();
        }

        private void RadioButton_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            // If you need the clicked element:
            // Item whichOne = senderElement.DataContext as Item;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
    }
}
