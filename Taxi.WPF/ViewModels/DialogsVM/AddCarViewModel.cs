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
    public class AddCarViewModel : ViewModelBase
    {
        VehiclesServices _vehiclesServices;
        public AddCarViewModel(VehiclesServices vehiclesServices)
        {
            _vehiclesServices = vehiclesServices;
            lstYears = new List<int>();
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
                lstYears.Add(i);
            //VehicleClasses = _vehiclesServices.GetAllVehicleClassesAsync().Result;

        }

        public async void Load()
        {
            var _vechicleClasses = await _vehiclesServices.GetAllVehicleClassesAsync();
            VehicleClasses.Clear();
            foreach (var v in _vechicleClasses)
            {
                VehicleClasses.Add(v);
            }
        }

        private VehicleDTO _newVehicle = new VehicleDTO();
        public VehicleDTO NewVehicle
        {
            get { return _newVehicle; }
            set
            {
                _newVehicle = value;
                OnPropertyChanged(nameof(NewVehicle));
            }
        }

        public List<int> lstYears { get; set; }
        public ObservableCollection<VehicleClassDTO> VehicleClasses { get; } = new ObservableCollection<VehicleClassDTO>();


        private bool CheckField()
        {

            if (string.IsNullOrWhiteSpace(NewVehicle.GovNumber) ||
                string.IsNullOrWhiteSpace(NewVehicle.Model) ||
                SelectedVehicleClass == null ||
                string.IsNullOrWhiteSpace( NewVehicle.Brand) ||
                NewVehicle.ClassId == null ||
                NewVehicle.Year == null 
                
                )
                return false;

            return true;
        }

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

        private AsyncRelayCommand _saveCommand;
        public AsyncRelayCommand SaveVehicleCommand
        {
            get => _saveCommand ??
                (_saveCommand = new AsyncRelayCommand(
                    async obj =>
                    {
                        bool check = CheckField();

                        if (!check)
                        {
                            MessageBox.Show("Заполните все поля!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        NewVehicle.ClassId = SelectedVehicleClass.Id;
                        NewVehicle.ClassName = SelectedVehicleClass.Name;


                        VehicleAddResponse res = await _vehiclesServices.AddVehicleAsync(NewVehicle);

                        if (!res.IsAdded)
                        {
                            MessageBox.Show(res.message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        MessageBox.Show(res.message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogHost.Close("AddVehicleDialog", NewVehicle);
                    }

                    ));
                
        }

    }
}
