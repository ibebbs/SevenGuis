using System;
using System.Windows;

namespace Counter.Wpf
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

            _viewModel = new Common.MainPageViewModel();

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

            _viewModel.Deativate();
        }
    }
}
