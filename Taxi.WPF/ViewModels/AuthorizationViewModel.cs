using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Taxi.Core.Models.Auth;
using Taxi.WPF.Commands;
using Taxi.WPF.Helpers;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {

        private readonly AuthService _authSerivce;

        public AuthorizationViewModel(IAuthService authService)
        {
            _authSerivce = (AuthService?)authService;
        }

        public string txtLoginLogin {  get; set; }
        public string txtPasswordLogin { get; set; }

        private RelayCommand _btnSendLogin;

        public string txtLoginRegister { get; set; }
        public string txtFirstNameRegister { get; set; }
        public string txtLastNameRegister { get; set; }
        public string txtMiddleRegister { get; set; }
        public string txtEmailRegister { get; set; }
        public string txtPasswordRegister { get; set; }
        public string txtConfirmPasswordRegister { get; set; }

        private RelayCommand _btnSendRegister;

        public RelayCommand BtnSendLogin
        {
            get
            {
                return _btnSendLogin ??
                    (_btnSendLogin = new RelayCommand(async opj =>
                    {
                        if (!StrHelper.CheckTxt(txtLoginLogin) || !StrHelper.CheckTxt(txtPasswordLogin))
                        {
                            MessageBox.Show("Необходимо ввести к");
                            return;
                        }
                        var userRequest = new LoginRequest(txtLoginLogin, txtPasswordLogin);
                        var result = await _authSerivce.LoginUserAsync(userRequest);
                        if (result == null)
                        {
                            MessageBox.Show("Такого пользователя нет");
                            return;
                        }
                    }));
            }
        }
        public RelayCommand BtnSendRegister
        {
            get
            {
                return _btnSendRegister ??
                    (_btnSendRegister = new RelayCommand(async opj =>
                    {
                        if (!StrHelper.CheckTxt(txtLoginRegister) 
                            || !StrHelper.CheckTxt(txtPasswordRegister) 
                            || !StrHelper.CheckTxt(txtLastNameRegister)
                            || !StrHelper.CheckTxt(txtFirstNameRegister)
                            || !StrHelper.CheckTxt(txtMiddleRegister)
                            || !StrHelper.CheckTxt(txtEmailRegister)
                            || !StrHelper.CheckTxt(txtConfirmPasswordRegister)
                            )
                        {
                            MessageBox.Show("Необходимо ввести к");
                            return;
                        }
                        if (txtConfirmPasswordRegister != txtPasswordRegister)
                        {
                            MessageBox.Show("Пароли не совпадают");
                            return;
                        }
                        var newUserReguest = new RegisterRequest(txtLoginRegister, txtPasswordRegister, txtLastNameRegister, 
                            txtFirstNameRegister, txtMiddleRegister, txtEmailRegister);
                        var result = await _authSerivce.RegisterNewUserAsync(newUserReguest);
                        if (result == null)
                        {
                            MessageBox.Show("Пользователь с таким логином уже зарегистрирован!");
                            return;
                        }
                    }));
            }
        }






        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
