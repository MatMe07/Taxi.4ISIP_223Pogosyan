using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Auth;
using Taxi.WPF.Commands;
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
        private RelayCommand _btnSendRegister;

        public RelayCommand BtnSendLogin
        {
            get
            {
                return _btnSendLogin ??
                    (_btnSendLogin = new RelayCommand(async opj =>
                    {
                        var userRequest = new LoginRequest(txtLoginLogin, txtPasswordLogin);
                        await _authSerivce.LoginUserAsync(userRequest);
                    }));
            }
        }
        public RelayCommand BtnSendRegister
        {
            get
            {
                return _btnSendRegister ??
                    (_btnSendRegister = new RelayCommand(opj =>
                    {
                        
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
