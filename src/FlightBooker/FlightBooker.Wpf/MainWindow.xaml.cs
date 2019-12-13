using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows;

namespace FlightBooker.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Common.MainPageViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new Common.MainPageViewModel(new SynchronizationContextScheduler(SynchronizationContext.Current));

            DataContext = _viewModel;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _viewModel.Activate();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _viewModel.Deactivate();
        }
    }
}
