using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Commands;

namespace Taxi.WPF.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private int _SelectedItemIndex;
        public int SelectedItemIndex
        {
            get => _SelectedItemIndex;
            set
            {
                _SelectedItemIndex = value;
                OnPropertyChanged(nameof(SelectedItemIndex));
            }
        }


        public ProfileViewModel() { }   
    }
}
