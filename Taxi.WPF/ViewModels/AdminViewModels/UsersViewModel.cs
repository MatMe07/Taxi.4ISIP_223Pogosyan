using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Taxi.Core.Models.DB;
using Taxi.WPF.Commands;
using Taxi.WPF.Services;
using Taxi.WPF.Views.Dialogs;

namespace Taxi.WPF.ViewModels.AdminViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private readonly UsersServices _usersServices;

        private readonly UserEditUC _userEditUC;
        public UsersViewModel(UsersServices usersServices, UserEditUC userEditUC)
        {
            _usersServices = usersServices;
            _userEditUC = userEditUC;
        }

        private ObservableCollection<UserDTO> _users = new();
        public ObservableCollection<UserDTO> Users { get; } = new ObservableCollection<UserDTO>();

        public async Task Load()
        {
            var list = await _usersServices.GetAllUsersAsync();
            Users.Clear();
            foreach (var user in list) Users.Add(user);
        }

        private AsyncRelayCommand _editUserCommand;
        public AsyncRelayCommand EditUserCommand
        {
            get => _editUserCommand ??
                (_editUserCommand = new AsyncRelayCommand(async obj =>
                {
                    if (obj is not UserDTO user) return;

                    _userEditUC.SelectUser = user;

                    var result = await DialogHost.Show(_userEditUC, "AddVehicleDialog") as (UserDTO editUser, bool isDelete)?;
                    if (result != null)
                    {
                        if (result.Value.isDelete)
                            Users.Remove(user);
                        else
                            Users[Users.IndexOf(user)] = result.Value.editUser;
                    }
                }));
        }

        private AsyncRelayCommand _deleteUserCommand;
        public AsyncRelayCommand DeleteUserCommand
        {
            get => _deleteUserCommand ??
                (_deleteUserCommand = new AsyncRelayCommand(async obj =>
                {
                    var res = MessageBox.Show(
                        "Действительно хотите удалить пользователя?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (res != MessageBoxResult.Yes)
                        return;

                    if (obj is not UserDTO user)
                        return;

                    var ok = await _usersServices.DeleteUserAsync(user.Id);
                    MessageBox.Show(
                        ok ? "Пользователь удалён" : "Не удалось удалить пользователя",
                        ok ? "Готово" : "Ошибка",
                        MessageBoxButton.OK,
                        ok ? MessageBoxImage.Information : MessageBoxImage.Warning);

                    if (ok)
                    {
                        //Users.Remove(user);
                        user.isActive = false;
                        var us = Users.FirstOrDefault(u => u.Id == user.Id);
                        if (us != null)
                        {
                            var ind = Users.IndexOf(us);
                            Users.Remove(us);
                            Users.Insert(ind, us);
                            OnPropertyChanged(nameof(Users));
                        }
                    }
                }));
        }
    }
}
