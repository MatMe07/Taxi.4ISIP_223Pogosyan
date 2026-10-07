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
using Taxi.src.Taxi.Map.Services;

namespace Taxi.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel viewModel;

        public MainWindow()
        {
            DotEnv.Load();
            InitializeComponent();
            viewModel = new MainViewModel(MyWebView);
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await AppCache.Cache.LoadAsync();

            await MyWebView.EnsureCoreWebView2Async(null);
            MyWebView.WebMessageReceived += WebView_WebMessageReceived;

            string htmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");
            Debug.WriteLine(htmlPath);

            MyWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.TryGetWebMessageAsString()))
            {
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("type", out JsonElement typeElement))
                    return;

                switch (typeElement.GetString())
                {
                    case "ready":
                        await viewModel.SetDefaultFromAsync();
                        break;

                    case "destinationSelected":
                        await viewModel.SelectKudaAsync(root.GetProperty("address").GetString());
                        break;
                }
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ButtonClickAsync();

        }


        private async void Window_Closed(object sender, EventArgs e)
        {

            await AppCache.Cache.SaveAsync();
        }
    }

}