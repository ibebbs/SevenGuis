using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cells.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Common.MainPageViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            PrepareSheet();

            _viewModel = new Common.MainPageViewModel(new SynchronizationContextScheduler(SynchronizationContext.Current));

            DataContext = _viewModel;
        }

        private void PrepareSheet()
        {
            Sheet.Columns.Add(new DataGridTemplateColumn { CellTemplate = (DataTemplate)Resources["rowHeaderTemplate"] });

            for (char column = 'A'; column < 'Z'; column++)
            {
                Sheet.Columns.Add(new DataGridCellColumn(column) { Header = column, CellTemplate = (DataTemplate)Resources["cellTemplate"], CellEditingTemplate = (DataTemplate)Resources["cellEditingTemplate"] });
            }

            Sheet.FrozenColumnCount = 1;
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
