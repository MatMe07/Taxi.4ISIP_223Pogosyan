using System.Windows;
using System.Windows.Controls;
using Taxi.WPF.ViewModels;
using Taxi.WPF.Views.ProfilePages;

namespace Taxi.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage(ProfileViewModel profileView, ProfileEditPage profileEditPage)
        {
            InitializeComponent();
            this.DataContext = profileView;
            frameProfilePage.Navigate(profileEditPage);

        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selectTabItem = sender as TabControl;
            
            //if ()
            //lst.Clear();
            if (selectTabItem != null)
            {
                selectTabItem.SelectedIndex = 0;
                e.Handled = true;   
                return;

                switch (selectTabItem.Tag.ToString())
                {
                    case "Мои отзывы":
                        {
                            //frameProfilePage.NavigationService.Navigate(new ProfileMyReviewsPage());
                            break;
                        }
                    case "Главная":
                        {
                            //frameProfilePage.NavigationService.Navigate(new ProfileMainPage());

                            break;
                        }
                    case "Уведомления":
                        {
                            //frameProfilePage.NavigationService.Navigate(new ProfileNotificationPage());

                            break;
                        }

                }
            }
        }

        //public void BackToMainPage()
        //{
        //    tabControlPage.SelectedIndex = 0;
        //}


        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            //frameProfilePage.NavigationService.Navigate(new ProfileEditPage(BackToMainPage));
            //tabControlPage.SelectedIndex = -1;

        }

        private void btnExitAcc_Click(object sender, RoutedEventArgs e)
        {
            {
                //var result = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?",
                //    "Подтверждение выхода",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Question);

                //if (result == MessageBoxResult.Yes)
                //{
                //    WorkDataBase.Instanse.User = null;
                //    var newMainWindow = new MainWindow();
                //    if (newMainWindow.AdminTabItem != null)
                //        newMainWindow.Show();
                //    else
                //    {
                //         Application.Current.Shutdown();
                //    }

                //        Window.GetWindow(this).Close();
                //    Application.Current.MainWindow = newMainWindow;

                //}
            }
        }

        private void tabControlPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
