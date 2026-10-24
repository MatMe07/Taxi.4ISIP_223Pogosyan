using LiveChartsCore.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Taxi.WPF.Views.ManagerPages;
using Windows.ApplicationModel.VoiceCommands;

namespace Taxi.WPF.ViewModels.ManagerViewModels
{
    public class ManagerViewModel : ViewModelBase
    {
        public Frame frameMain { get; set; }

        private DataCarsPage carsPage;
        private ReportsManagerPage reportsManagerPage;

        public ManagerViewModel(DataCarsPage dataCarsPage, ReportsManagerPage reportsManagerPage)
        {
            carsPage = dataCarsPage;
            this.reportsManagerPage = reportsManagerPage;
        }

        public void Load()
        {
            SelectedIndexTC = 0;
            ChangePage(0);
        }

        private void ChangePage(int pId)
        {

            switch(pId)
            {
                case 0: frameMain.Navigate(carsPage);break;
                case 5: frameMain.Navigate(reportsManagerPage);break;
            }
        }

        private int _selectedIndexTC = 0;
        public int SelectedIndexTC
        {
            get
            {
                return _selectedIndexTC;
            }
            set
            {
                _selectedIndexTC = value;
                ChangePage(value);
                OnPropertyChanged(nameof(SelectedIndexTC));
            }
        }

    }
}
