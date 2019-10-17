using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TemperatureConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Common.MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            _viewModel = new Common.MainPageViewModel();

            DataContext = _viewModel;
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
