using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
using Taxi.WPF.ViewModels;

namespace Taxi.src.Taxi.Map
{
    /// <summary>
    /// Логика взаимодействия для MapPage.xaml
    /// </summary>
    public partial class MapPage : Page
    {
        private MapViewModel viewModel;

        public MapPage()
        {
            InitializeComponent();
            viewModel = new MapViewModel(MyWebView);
            MainWindow_Loaded();
        }
        private async void MainWindow_Loaded()
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ButtonClickAsync();

        }

    }
}
