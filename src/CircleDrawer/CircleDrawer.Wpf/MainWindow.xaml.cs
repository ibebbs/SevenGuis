using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CircleDrawer.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IEnumerable<System.Drawing.Point> GetAreaPoint(MouseButtonEventArgs args)
        {
            switch (args.OriginalSource)
            {
                case Control control:
                    var controlLocation = args.GetPosition(control);
                    yield return new System.Drawing.Point(Convert.ToInt32(controlLocation.X), Convert.ToInt32(controlLocation.Y));
                    break;
                case Canvas canvas:
                    var canvasLocation = args.GetPosition(canvas);
                    yield return new System.Drawing.Point(Convert.ToInt32(canvasLocation.X), Convert.ToInt32(canvasLocation.Y));
                    break;
            }
        }

        private readonly MainPageViewModel _viewModel;
        private readonly IObservable<System.Drawing.Point> _emptyAreaClicked;
        private readonly IObservable<Unit> _adjustDiameterDialogClosed;
        private readonly IObserver<Unit> _showAdjustDiameterDialog;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainPageViewModel();

            DataContext = _viewModel;

            _emptyAreaClicked = Observable
                .FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(handler => CirclesContainer.MouseLeftButtonUp += handler, handler => CirclesContainer.MouseLeftButtonUp -= handler)
                .SelectMany(pattern => GetAreaPoint(pattern.EventArgs));

            _adjustDiameterDialogClosed = Observable
                .FromEventPattern(handler => AdjustDiameterDialog.Closed += handler, handler => AdjustDiameterDialog.Closed -= handler)
                .Select(_ => Unit.Default);

            _showAdjustDiameterDialog = Observer.Create<Unit>(_ => ShowAdjustDiameterDialog());
        }

        private void ShowAdjustDiameterDialog()
        {
            if (!AdjustDiameterDialog.IsOpen)
            {
                AdjustDiameterDialog.Child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                AdjustDiameterDialog.VerticalOffset = 0 - AdjustDiameterDialog.Child.DesiredSize.Height;
                AdjustDiameterDialog.IsOpen = true;
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _viewModel.Activate(_emptyAreaClicked, _showAdjustDiameterDialog, _adjustDiameterDialogClosed);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _viewModel.Deactivate();
        }

        //private void RadioButton_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        //{
        //    FrameworkElement senderElement = sender as FrameworkElement;
        //    // If you need the clicked element:
        //    // Item whichOne = senderElement.DataContext as Item;
        //    FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
        //    flyoutBase.ShowAt(senderElement);
        //}
    }
}
