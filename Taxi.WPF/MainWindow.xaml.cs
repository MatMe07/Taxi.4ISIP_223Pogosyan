using System.Windows;
using System.Windows.Controls;
using Taxi.src.Taxi.Map;
using Taxi.WPF.Models;
using Taxi.WPF.Views;


namespace Taxi.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MapPage mapPage;

        public MainWindow(AuthorizationPage authorizationPage, MapPage mapPage)
        {
            InitializeComponent();
            if (!AppSettings.Instance.IsSign)
                frameMain.Navigate(authorizationPage);
            else
            {
                frameMain.Navigate(mapPage);
            }
                this.mapPage = mapPage;

        }


        public void ChangePage(Page page)
        {
            frameMain.Navigate(page);
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