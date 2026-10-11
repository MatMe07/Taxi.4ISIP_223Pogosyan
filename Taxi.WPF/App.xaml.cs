using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Taxi.Core.Models.Auth;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;
using Taxi.WPF.ViewModels;
using Taxi.WPF.Views;


namespace Taxi.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider serviceProvider {  get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            DotEnv.Load();

            base.OnStartup(e);

            var services = new ServiceCollection();

            services.ConfigureHttpClientDefaults(opt =>
            {
                opt.ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BaseAddress"));

                });
            });

            services.AddHttpClient<IAuthService, AuthService>();




            services.AddTransient<AuthorizationViewModel>();
            services.AddTransient<AuthorizationPage>();





            services.AddTransient<MainWindow>();





            serviceProvider = services.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
