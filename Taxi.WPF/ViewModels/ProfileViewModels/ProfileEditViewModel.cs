using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Taxi.WPF.Commands;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;
using Windows.Networking.NetworkOperators;

namespace Taxi.WPF.ViewModels.ProfileViewModels
{
    public class ProfileEditViewModel : ViewModelBase
    {

        private RelayCommand _openChangePasswordCommand;
        public RelayCommand OpenChangePasswordCommand
        {
            get
            {
                return _openChangePasswordCommand ??
                    (_openChangePasswordCommand = new RelayCommand(opj =>
                    {
                        IsChangePasswordOpen = true;
                    }));

            }
        }

        public string PasswordCurrent { get; set; }
        public string PasswordNew { get; set; }
        public string PasswordNewConfirm { get; set; }


        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
                CheckFieldsUser();
            }
        }
        private string _Login;
        public string Login
        {
            get => _Login;
            set
            {
                _Login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        private string _Phone;
        public string Phone
        {
            get => _Phone;
            set
            {
                _Phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }



        private bool CheckFilledfields()
        {
            return !string.IsNullOrWhiteSpace(PasswordCurrent) ||
                !string.IsNullOrWhiteSpace(PasswordNew) ||
                !string.IsNullOrWhiteSpace(PasswordNewConfirm);
        }
        private bool ClosePupon()
        {
            if (CheckFilledfields())
            {
                var res = MessageBox.Show("Вы действительно хотите выйти? Данные потеряются!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information);
                return res == MessageBoxResult.No;
            }
            return false;
        }

        private bool _isChangePasswordOpen = false;
        public bool IsChangePasswordOpen
        {
            get { return _isChangePasswordOpen; }
            set
            {
                if (value == false) ClearPasswords();
                _isChangePasswordOpen = value;
                OnPropertyChanged(nameof(IsChangePasswordOpen));
            }
        }

        public Action ClearPasswords {  get; set; }

        private RelayCommand _closeChangePasswordCommand;
        public RelayCommand CloseChangePasswordCommand
        {
            get
            {
                return _closeChangePasswordCommand ??
                    (_closeChangePasswordCommand = new RelayCommand(obj =>
                    {
                        var checkFiled= ClosePupon();
                        //if (!checkFiled) ClearPasswords();
                        IsChangePasswordOpen = checkFiled;
                    }));
            }
        }


        private RelayCommand _savePasswordCommand;
        public RelayCommand SavePasswordCommand
        {
            get
            {
                return _savePasswordCommand ??
                    (new RelayCommand(async obj =>
                    {
                        if (
                        !string.IsNullOrWhiteSpace(PasswordCurrent) &&
                        !string.IsNullOrWhiteSpace(PasswordNew) &&
                        !string.IsNullOrWhiteSpace(PasswordNewConfirm)
                        )
                        {
                            var result = await _auth.ChangePasswordAsync(PasswordCurrent,PasswordNew, PasswordNewConfirm);
                            if (result.IsSuccess)
                            {
                                MessageBox.Show(result.message, "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                                IsChangePasswordOpen = false;

                            }
                            else
                            {
                                MessageBox.Show(result.message, "Неудача", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }));
            }
        }
        private bool _isEnabledButton = false;
        public bool IsEnabledButton
        {
            get => _isEnabledButton;
            set
            {
                _isEnabledButton = value;
                OnPropertyChanged(nameof(IsEnabledButton));
            }

        }

        private void CheckFieldsUser()
        {
            IsEnabledButton = !string.Equals(_baseDisplayName, DisplayName);
        }

        private string _baseDisplayName;

        //private MainViewModel MainViewModel;
        private readonly AuthService _auth;
        private IUserSessionService UserSessionService;
        public ProfileEditViewModel(IAuthService authService, IUserSessionService userSession)
        {
            _auth = authService as AuthService;
            UserSessionService = userSession;

        }

        public void Load()
        {
            var us = AppSettings.Instance.userSession ?? throw new InvalidOperationException("Not authenticated");
            _baseDisplayName = $"{us.User.FirstName} {us.User.LastName} {us.User.MiddleName}";
            DisplayName = $"{us.User.FirstName} {us.User.LastName} {us.User.MiddleName}";
            Phone = us.User.Phone;
            Login = us.User.Login;
        }


    }
}
