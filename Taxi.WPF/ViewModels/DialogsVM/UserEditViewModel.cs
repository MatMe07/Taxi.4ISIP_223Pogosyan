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

namespace Taxi.WPF.ViewModels.DialogsVM
{

    public class UserEditViewModel : ViewModelBase
    {
        private readonly UsersServices _usersServices;

        public UserEditViewModel(UsersServices usersServices)
        {
            _usersServices = usersServices;
        }

        public async Task LoadAsync()
        {
            NewUser = new UserDTO
            {
                Id = EditUser.Id,
                Login = EditUser.Login,
                LastName = EditUser.LastName,
                FirstName = EditUser.FirstName,
                MiddleName = EditUser.MiddleName,
                Phone = EditUser.Phone,
                RoleId = EditUser.RoleId,
                RoleName = EditUser.RoleName,
                CreatedAt = EditUser.CreatedAt,
                isActive = EditUser.isActive
            };

            var roles = await _usersServices.GetAllRolesAsync();

            Roles.Clear();
            foreach (var r in roles)
                Roles.Add(r);

            SelectedRole = Roles.FirstOrDefault(r => r.Id == NewUser.RoleId);
        }

        private bool CheckField()
        {
            return !string.Equals(NewUser.LastName, EditUser.LastName) ||
                   !string.Equals(NewUser.FirstName, EditUser.FirstName) ||
                   !string.Equals(NewUser.MiddleName, EditUser.MiddleName) ||
                   !string.Equals(NewUser.Phone, EditUser.Phone) ||
                   NewUser.RoleId != EditUser.RoleId ||
                   NewUser.isActive != EditUser.isActive;
        }

        private AsyncRelayCommand _deleteCommand;
        public AsyncRelayCommand DeleteCommand
        {
            get => _deleteCommand ??
                (_deleteCommand = new AsyncRelayCommand(async obj =>
                {
                    var res = MessageBox.Show(
                        "Действительно хотите удалить?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (res != MessageBoxResult.Yes)
                        return;

                    var result = await _usersServices.DeleteUserAsync(NewUser.Id);
                    MessageBox.Show(
                        result ? "Пользователь удалён" : "Не удалось удалить пользователя",
                        result ? "Готово" : "Ошибка",
                        MessageBoxButton.OK,
                        result ? MessageBoxImage.Information : MessageBoxImage.Warning);

                    if (result)
                        DialogHost.Close("AddVehicleDialog", (EditUser, true));
                }));
        }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get => _cancelCommand ??
                (_cancelCommand = new RelayCommand(obj =>
                {
                    if (CheckField())
                    {
                        var res = MessageBox.Show("Вы действительно хотите выйти? Данные потеряются!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (res == MessageBoxResult.Yes)
                        {
                            DialogHost.Close("AddVehicleDialog");
                            return;
                        }
                    }
                    else
                        DialogHost.Close("AddVehicleDialog");
                }));
        }

        private AsyncRelayCommand _saveCommand;
        public AsyncRelayCommand SaveCommand
        {
            get => _saveCommand ??
                (_saveCommand = new AsyncRelayCommand(async obj =>
                {
                    if (string.IsNullOrWhiteSpace(NewUser.LastName) ||
                        string.IsNullOrWhiteSpace(NewUser.FirstName) ||
                        string.IsNullOrWhiteSpace(NewUser.Phone) ||
                        SelectedRole == null)
                    {
                        MessageBox.Show(
                            "Проверьте корректность заполнения полей.",
                            "Ошибка валидации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    var res = MessageBox.Show(
                        "Сохранить изменения?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (res != MessageBoxResult.Yes)
                        return;

                    var result = await _usersServices.UpdateUserAsync(NewUser);
                    MessageBox.Show(
                        result ? "Данные сохранены" : "Не удалось сохранить изменения",
                        result ? "Готово" : "Ошибка",
                        MessageBoxButton.OK,
                        result ? MessageBoxImage.Information : MessageBoxImage.Warning);

                    if (result)
                        DialogHost.Close("AddVehicleDialog", (NewUser, false));
                }));
        }

        public ObservableCollection<RoleDTO> Roles { get; } = new ObservableCollection<RoleDTO>();

        private RoleDTO _selectedRole;
        public RoleDTO SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                if (NewUser != null && SelectedRole != null)
                {
                    NewUser.RoleId = SelectedRole.Id;
                    NewUser.RoleName = SelectedRole.RoleName;
                }
                OnPropertyChanged(nameof(SelectedRole));
            }
        }

        public UserDTO EditUser { get; set; }

        private UserDTO _newUser;
        public UserDTO NewUser
        {
            get => _newUser;
            set
            {
                _newUser = value;
                OnPropertyChanged(nameof(NewUser));
            }
        }
    }
}