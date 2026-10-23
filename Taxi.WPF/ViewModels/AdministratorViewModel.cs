using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Taxi.WPF.Views;
using Taxi.WPF.Views.AdminPages;

namespace Taxi.WPF.ViewModels
{
    public class AdministratorViewModel
    {
        private UsersPage _usersPage;
        public AdministratorViewModel(UsersPage usersPage)
        {
            this._usersPage = usersPage;

        }

        public void Load()
        {
            frameMain.Navigate(_usersPage);
        }

        public Frame frameMain;

    }
}
