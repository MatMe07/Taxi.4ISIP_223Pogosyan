using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Commands;

namespace Taxi.WPF.ViewModels.ProfileViewModels
{
    public class ProfileEditViewModel : INotifyPropertyChanged
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

        private bool _isChangePasswordOpen = false;



        public bool IsChangePasswordOpen
        {
            get { return _isChangePasswordOpen; }
            set
            {
                _isChangePasswordOpen = value;
                //MainViewModel.ShowBlur(false);
                OnPropertyChanged(nameof(IsChangePasswordOpen));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private MainViewModel MainViewModel;
        public ProfileEditViewModel()
        {
            //MainViewModel = mainView;
        }
    }
}
