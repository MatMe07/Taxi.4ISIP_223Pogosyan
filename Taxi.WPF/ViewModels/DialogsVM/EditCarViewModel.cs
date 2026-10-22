using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.WPF.Services;

namespace Taxi.WPF.ViewModels.DialogsVM
{
    public class EditCarViewModel : ViewModelBase
    {
        VehiclesServices _vehiclesServices;

        public EditCarViewModel(VehiclesServices vehiclesServices)
        {

            _vehiclesServices = vehiclesServices;

            lstYears = new List<int>();
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
                lstYears.Add(i);
            SelectedVehicleClass = new VehicleClassDTO { Id = 1, Name = ""};

        }


        public async Task LoadAsync()
        {
            NewVehicle = new VehicleDTO(EditVehicle);
            var selClass = new VehicleClassDTO { Id=NewVehicle.Id, Name=NewVehicle.ClassName };

            var _vechicleClasses = await _vehiclesServices.GetAllVehicleClassesAsync();


            VehicleClasses.Clear();
            foreach (var v in _vechicleClasses)
            {
                VehicleClasses.Add(v);
            }
            var firstSelClass = VehicleClasses.ToList().FirstOrDefault(v => v.Name == selClass.Name);
            var ind = VehicleClasses.IndexOf(firstSelClass);
            SelectedVehicleClass = VehicleClasses[ind == -1 ? 3 : ind];

        }


        public List<int> lstYears { get; set; }
        public ObservableCollection<VehicleClassDTO> VehicleClasses { get; } = new ObservableCollection<VehicleClassDTO>();


        private VehicleClassDTO _selectedVehicleClass;
        public VehicleClassDTO SelectedVehicleClass
        {
            get
            {
                return _selectedVehicleClass;
            }
            set
            {
                _selectedVehicleClass = value;
                OnPropertyChanged(nameof(SelectedVehicleClass));
            }
        }
        public VehicleDTO EditVehicle {  get; set; }

        private VehicleDTO _newVehicle;
        public VehicleDTO NewVehicle
        {
            get
            {
                return _newVehicle;
            }
            set
            {
                _newVehicle = value;
                OnPropertyChanged(nameof(NewVehicle));  
            }
        }


    }
}
