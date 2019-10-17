using System;
using System.Windows;

namespace Crud.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainPageViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainPageViewModel();

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
