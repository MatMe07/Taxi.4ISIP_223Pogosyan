using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Taxi.Core.Models.Auth;
using Taxi.WPF.Commands;
using Taxi.WPF.Helpers;
using Taxi.WPF.Models;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;
using Taxi.WPF.Views;
using Windows.Devices.Usb;

namespace Taxi.WPF.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {

        private readonly AuthService _authSerivce;

        public ConfigurationService _configurationSerivce;
        public AuthorizationViewModel(IAuthService authService)
        {
            _authSerivce = (AuthService?)authService;
        }

        public Action ClearPasswords;


        private Guid _captchaId;
        private string _txtLoginLogin;
        public string txtLoginLogin
        {
            get { return _txtLoginLogin; }
            set
            {
                _txtLoginLogin = value;
                OnPropertyChanged(nameof(txtLoginLogin));
            }
        }
        public string txtPasswordLogin { get; set; }

        private RelayCommand _btnSendLogin;




        private string _txtLoginRegister;
        public string txtLoginRegister
        {
            get { return _txtLoginRegister; }
            set
            {
                _txtLoginRegister = value;
                OnPropertyChanged(nameof(txtLoginRegister));
            }
        }

        private string _txtFirstNameRegister;
        public string txtFirstNameRegister
        {
            get { return _txtFirstNameRegister; }
            set
            {
                _txtFirstNameRegister = value;
                OnPropertyChanged(nameof(txtFirstNameRegister));
            }
        }

        private string _txtLastNameRegister;
        public string txtLastNameRegister
        {
            get { return _txtLastNameRegister; }
            set
            {
                _txtLastNameRegister = value;
                OnPropertyChanged(nameof(txtLastNameRegister));
            }
        }

        private string _txtMiddleRegister;
        public string txtMiddleRegister
        {
            get { return _txtMiddleRegister; }
            set
            {
                _txtMiddleRegister = value;
                OnPropertyChanged(nameof(txtMiddleRegister));
            }
        }

        private string _txtPhoneRegister;
        public string txtPhoneRegister
        {
            get { return _txtPhoneRegister; }
            set
            {
                _txtPhoneRegister = value;
                OnPropertyChanged(nameof(txtPhoneRegister));
            }
        }
        public string txtPasswordRegister { get; set; }
        
        public string txtConfirmPasswordRegister {  get; set; }

        private RelayCommand _btnSendRegister;


        private bool _IsCaptchaVisible = false;
        public bool IsCaptchaVisible
        {
            get { return _IsCaptchaVisible; }

            set
            {
                if (_IsCaptchaVisible != value)
                {
                    _IsCaptchaVisible = value;
                    OnPropertyChanged(nameof(IsCaptchaVisible));
                }
            }
        }
        private string _txtCaptchaLogin = "";
        public string txtCaptchaLogin
        {
            get { return _txtCaptchaLogin; }
            set
            {
                _txtCaptchaLogin = value;
                OnPropertyChanged(nameof(txtCaptchaLogin));
            }
        }

        private ImageSource _captchaImage;
        public ImageSource CaptchaImage
        {
            get => _captchaImage;
            set
            {
                _captchaImage = value;
                OnPropertyChanged(nameof(CaptchaImage));
            }
        }

        public RelayCommand BtnSendLogin
        {
            get
            {
                return _btnSendLogin ??
                    (_btnSendLogin = new RelayCommand(async opj =>
                    {
                        if (!StrHelper.CheckTxt(txtLoginLogin) || !StrHelper.CheckTxt(txtPasswordLogin))
                        {
                            MessageBox.Show("Необходимо заполнить все поля");
                            return;
                        }

                        var userRequest = new LoginRequest
                        {
                            login = txtLoginLogin,
                            password = txtPasswordLogin,
                            captchaId = _captchaId,
                            captchaText = txtCaptchaLogin
                        };
                        var result = await _authSerivce.LoginUserAsync(userRequest);

                        if (!result.IsSuccess)
                        {
                            MessageBox.Show(result.message);
                            _captchaId = result.CaptchaId;
                            if (result.IsCaptchaRequired)
                            {
                                CaptchaImage = null;
                                byte[] imgBytes = Convert.FromBase64String(result.CaptchaImageBase64);
                                using var ms = new MemoryStream(imgBytes);

                                var bimap = new BitmapImage();
                                bimap.BeginInit();
                                bimap.StreamSource = ms;
                                bimap.CacheOption = BitmapCacheOption.OnLoad;
                                bimap.EndInit();

                                CaptchaImage = bimap;

                                IsCaptchaVisible = true;
                                return;
                            }
                        }

                        IsCaptchaVisible = false;
                        CaptchaImage = null;


                        //AppSettings.Instance.userSession.User = result.User;

                        AppCache.Cache.Set(nameof(AppSettings), AppSettings.Instance);
                        
                        ChangePage();
                    }));
            }
        }

        private void ChangePage()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ChangePage();
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
                            || !StrHelper.CheckTxt(txtPhoneRegister)
                            || !StrHelper.CheckTxt(txtConfirmPasswordRegister)
                            )
                        {
                            MessageBox.Show("Необходимо заполнить все поля");
                            return;
                        }


                        if (!string.Equals(txtConfirmPasswordRegister, txtPasswordRegister, StringComparison.Ordinal))
                        {
                            MessageBox.Show("Пароли не совпадают");
                            return;
                        }


                        var newUserReguest = new RegisterRequest(txtLoginRegister, txtPasswordRegister, txtLastNameRegister, 
                            txtFirstNameRegister, txtMiddleRegister, txtPhoneRegister);

                        var result = await _authSerivce.RegisterNewUserAsync(newUserReguest);

                        if (result.login == null)
                        {

                            MessageBox.Show(result.message);
                            return;
                        }

                        SelectItemTabControl = 0;
                        txtLoginLogin = txtLoginRegister;

                        txtLoginRegister = "";
                        txtFirstNameRegister = "";
                        txtLastNameRegister = "";
                        txtMiddleRegister = "";
                        txtPhoneRegister = "";
                        ClearPasswords();
                    }));
            }
        }

        private int _selectItemTabControl = 0;
        public int SelectItemTabControl
        {
            get { return _selectItemTabControl; }
            set
            {
                _selectItemTabControl = value;
                OnPropertyChanged(nameof(SelectItemTabControl));
            }
        }





    }
}
