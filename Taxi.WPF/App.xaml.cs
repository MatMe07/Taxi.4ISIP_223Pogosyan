using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System.Windows;
using Taxi.Core.Models.Auth;
using Taxi.src.Services;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;
using Taxi.WPF.ViewModels;
using Taxi.WPF.ViewModels.AdminViewModels;
using Taxi.WPF.ViewModels.DialogsVM;
using Taxi.WPF.ViewModels.ManagerViewModels;
using Taxi.WPF.ViewModels.ProfileViewModels;
using Taxi.WPF.Views;
using Taxi.WPF.Views.AdminPages;
using Taxi.WPF.Views.Dialogs;
using Taxi.WPF.Views.ManagerPages;
using Taxi.WPF.Views.ProfilePages;


namespace Taxi.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider serviceProvider {  get; private set; }
        protected async override void OnStartup(StartupEventArgs e)
        {
            DotEnv.Load();

            base.OnStartup(e);


            await AppCache.Cache.LoadAsync();

            ExcelPackage.License.SetNonCommercialPersonal("MatMe");


            var services = new ServiceCollection();

            services.AddTransient<MyDelegatingHandler>();
            services.AddSingleton<ITokenStore, TokenStore>();
            services.AddSingleton<IUserSessionService, UserSessionService>();

            services.ConfigureHttpClientDefaults(opt =>
            {
                opt.ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BaseAddress"));
                });

                opt.AddHttpMessageHandler<MyDelegatingHandler>();

            });

            services.AddHttpClient<IAuthService, AuthService>();
            services.AddHttpClient<IGeocodeService, GeocodeService>();
            services.AddHttpClient<IRoutingService, RoutingService>();

            services.AddSingleton<ConfigurationService>();
            services.AddSingleton<IConfigurationService>(sp=>sp.GetRequiredService<ConfigurationService>());


            services.AddTransient<VehiclesServices>();
            services.AddTransient<PoezdkaService>();
            services.AddTransient<GenerationFilesService>();
            services.AddTransient<UsersServices>();


            services.AddTransient<AuthorizationViewModel>();
            services.AddTransient<AuthorizationPage>();

            services.AddTransient<MapViewModel>();
            services.AddTransient<MapPage>();


            services.AddSingleton<ConfigurationService>();



            services.AddTransient<ProfileViewModel>();
            services.AddTransient<ProfilePage>();

            services.AddTransient<ProfileEditViewModel>();
            services.AddTransient<ProfileEditPage>();




            services.AddTransient<ManagerViewModel>();
            services.AddTransient<ManagerAutoPage>();

            services.AddTransient<DataCarsViewModel>();
            services.AddTransient<DataCarsPage>();

            services.AddTransient<AddCarsUC>();
            services.AddTransient<AddCarViewModel>();

            services.AddTransient<EditCarUC>();
            services.AddTransient<EditCarViewModel>();

            services.AddTransient<ReportsManagerPage>();
            services.AddTransient<ReportsManagerViewModel>();




            services.AddTransient<UsersViewModel>();
            services.AddTransient<UsersPage>();

            services.AddTransient<AdministratorViewModel>();
            services.AddTransient<AdministratorPage>();

            services.AddTransient<UserEditViewModel>();
            services.AddTransient<UserEditUC>();




            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();



            serviceProvider = services.BuildServiceProvider();

            var configuration = serviceProvider.GetRequiredService<ConfigurationService>();

            configuration.Load();


            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
