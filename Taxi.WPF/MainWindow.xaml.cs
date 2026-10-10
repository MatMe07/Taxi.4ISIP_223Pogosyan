using dotenv.net;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Taxi.Core.ViewModels;
using Taxi.src.Taxi.Map;
using Taxi.src.Taxi.Map.Helpers;
using Taxi.WPF.Views;

namespace Taxi.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //private MainViewModel viewModel;

        public MainWindow()
        {
            DotEnv.Load();
            InitializeComponent();
            //viewModel = new MainViewModel(MyWebView);
            //frameMain.Navigate(new MapPage());
            frameMain.Navigate(new AuthorizationPage());
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //MainWindow_Loaded()
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
    }

}