using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.Services.HubServices;

namespace Taxi.WPF.ViewModels.UsersViewModels
{
    public class DriverViewModel : ViewModelBase
    {
        private readonly HubConnect hubConnect;
        private readonly PoezdkaService poezdkaService;
        public Action<string, object> CallJsAsync { get; set; }

        public DriverViewModel(HubConnect hubConnect, PoezdkaService poezdkaService) {
            this.hubConnect = hubConnect;
            this.poezdkaService = poezdkaService;
            AppSettings.Instance.PropertyChanged += OnAppSettingsChanged;
            hubConnect.OnMessageReceived += (message) =>
            {
                Debug.WriteLine($"uS: {message}");
            };
            //this.hubConnect.Regi

        }

        private void OnAppSettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.Instance.Theme))
            {
                CallJsAsync("changeTheme", new { Theme = AppSettings.Instance.Theme });
            }
        }
        

        public async Task WebMessage(JsonElement root)
        {
            if (!root.TryGetProperty("type", out JsonElement typeElement))
                return;

            switch (typeElement.GetString())
            {
                case "ready":
                    {
                        var response = await poezdkaService.TryToConnectAsync();
                        CallJsAsync("tryToOnline", response);
                        break;
                    }
                case "driverToOnline":
                    {
                        if (bool.TryParse( root.GetProperty("status").GetString(), out bool status))
                        {

                            try
                            {
                                await hubConnect.StartAsync();

                                await hubConnect.SendMessageAsync("message");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
                            }

                        }
                        break;
                    }
            }
        }


    }
}
