namespace FantoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            DocsWebView.Navigated += OnDocsWebViewNavigated;
        }

        private void OnDocsWebViewNavigated(object? sender, WebNavigatedEventArgs e)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}
