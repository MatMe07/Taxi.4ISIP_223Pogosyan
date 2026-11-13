using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.ViewModels.UsersViewModels;

namespace Taxi.WPF.Views.UserPages
{
    /// <summary>
    /// Логика взаимодействия для DriverMapPage.xaml
    /// </summary>
    public partial class DriverMapPage : Page
    {
        private DriverViewModel viewModel;

        public DriverMapPage(DriverViewModel driverViewModel)
        {
            InitializeComponent();
            viewModel = driverViewModel;
            this.viewModel.CallJsAsync = SendToWeb;
            MainWindow_Loaded();
        }
        private async void MainWindow_Loaded()
        {
            await MyWebView.EnsureCoreWebView2Async(null);
            MyWebView.WebMessageReceived += WebView_WebMessageReceived;
            MyWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;

            string htmlPath = System.IO.Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, "Views/Map");
            MyWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "localhost",
                htmlPath,
                CoreWebView2HostResourceAccessKind.Allow
                );

            Debug.WriteLine(htmlPath);



            //MyWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
            MyWebView.Source = new Uri($"http://localhost/html/driverMap.html?theme={AppSettings.Instance.Theme}");
            await MyWebView.EnsureCoreWebView2Async();
        }

        private void SendToWeb(string commandType, object data)
        {
            MyWebView.CoreWebView2.PostWebMessageAsJson(WebMessages.SendToWeb(commandType, data));
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using JsonDocument doc = JsonDocument.Parse(e.TryGetWebMessageAsString());

            JsonElement root = doc.RootElement;

            await viewModel.WebMessage(root);
        }
    }
}
