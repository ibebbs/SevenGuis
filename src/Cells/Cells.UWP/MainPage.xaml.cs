using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Reactive.Concurrency;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cells
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Common.MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            PrepareSheet();

            _viewModel = new Common.MainPageViewModel(new CoreDispatcherScheduler(Dispatcher));

            DataContext = _viewModel;
        }

        private void PrepareSheet()
        {
            Sheet.Columns.Add(new DataGridTemplateColumn { CellTemplate = (DataTemplate)Resources["rowHeaderTemplate"] });

            for (char column = 'A'; column < 'Z'; column++)
            {
                Sheet.Columns.Add(new DataGridCellColumn(column) { Header = column, Tag = column, CellTemplate = (DataTemplate)Resources["cellTemplate"], CellEditingTemplate = (DataTemplate)Resources["cellEditingTemplate"] });
            }

            Sheet.FrozenColumnCount = 1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel.Activate();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _viewModel.Deactivate();
        }
    }
}
