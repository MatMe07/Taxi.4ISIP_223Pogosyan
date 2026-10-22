using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.WPF.Commands;
using Taxi.WPF.Services;

namespace Taxi.WPF.ViewModels.DialogsVM
{
    public class AddCarViewModel : ViewModelBase
    {
        VehiclesServices _vehiclesServices;
        public AddCarViewModel(VehiclesServices vehiclesServices)
        {
            _vehiclesServices = vehiclesServices;
            lstYears = new List<int>();
            for (int i = 2000; i < DateTime.Now.Year+1; i++)
                lstYears.Add(i);
            //VehicleClasses = _vehiclesServices.GetAllVehicleClassesAsync().Result;

        }

        public async void Load()
        {
            var _vechicleClasses = await _vehiclesServices.GetAllVehicleClassesAsync(); 
            VehicleClasses.Clear();
            foreach(var  v in _vechicleClasses)
            {
                VehicleClasses.Add(v);
            }
        }

        private VehicleDTO _newVehicle;
        public VehicleDTO NewVehicle
        {
            get { return _newVehicle; }
            set
            {
                _newVehicle = value;
                OnPropertyChanged(nameof(NewVehicle));
            }
        }

        public List<int> lstYears {  get; set; }
        public ObservableCollection<VehicleClassDTO> VehicleClasses { get; } = new ObservableCollection<VehicleClassDTO>();


        //public RelayCommand 

    }
}
