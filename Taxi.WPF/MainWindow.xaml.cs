using System.Windows;
using System.Windows.Controls;
using Taxi.src.Taxi.Map;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.ViewModels;
using Taxi.WPF.Views;


namespace Taxi.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow(MainViewModel viewModel,
            AuthorizationPage authorizationPage)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            viewModel.frameMain = frameMain;
            if (!AppSettings.Instance.IsSign )
            {
                frameMain.Navigate(authorizationPage);
                return;
            }
            //frameMain.Navigate(mainPage);

        }

        public void ChangePage()
        {
            (DataContext as MainViewModel).LoadNextPageRoleId(); 
            while (frameMain.CanGoBack)
            {
                frameMain.RemoveBackEntry();
            }
        }



        //public async Task PoiskAsync()
        //{
        //    var body = new { query = "москва хабар" };

        //    var request = new HttpRequestMessage(HttpMethod.Post, URL)
        //    {
        //        Content = new StringContent(
        //            JsonSerializer.Serialize(body),
        //            Encoding.UTF8,
        //            "application/json")
        //    };
        //    request.Headers.TryAddWithoutValidation("Accept", "application/json");
        //    request.Headers.TryAddWithoutValidation("Authorization", "Token " + token);

        //    var response = await httpClient.SendAsync(request);
        //    string responseText = await response.Content.ReadAsStringAsync();

        //    Debug.WriteLine((int)response.StatusCode);
        //    Debug.WriteLine(responseText);
        //}

        private async void Window_Closed(object sender, EventArgs e)
        {

            await AppCache.Cache.SaveAsync();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppSettings.Instance.IsSign) return;
            //LoadNextPageRoleId();
            ChangePage();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите закрыть окно?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            
        }
    }

}