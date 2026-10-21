using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Taxi.Core.Models.DB;
using Taxi.WPF.Commands;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;
using Taxi.WPF.Views.Dialogs;

namespace Taxi.WPF.ViewModels.ManagerViewModels
{
    public class DataCarsViewModel : ViewModelBase
    {

        ManagerService ManagerService;
        AddCarsUC AddCars;
        EditCarUC EditCarUC;
        public DataCarsViewModel(IUserBaseService managerService, AddCarsUC addCars, EditCarUC editCarUC)
        {
            ManagerService = managerService as ManagerService;
            AddCars = addCars;
            EditCarUC = editCarUC;
        }

        private string _search = "";
        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                SearchFilt();
                OnPropertyChanged(nameof(Search));
            }
        }

        private string _selectedClassSort = "Все";
        public string SelectedClassSort
        {
            get => _selectedClassSort;
            set
            {
               _selectedClassSort = value;
                SearchFilt();
                OnPropertyChanged(nameof(SelectedClassSort));
            }
        }


        private string _selectedDopuskSort = "Все";
        public string SelectedDopuskSort
        {
            get => _selectedDopuskSort;
            set
            {
                _selectedDopuskSort = value;
                SearchFilt();
                OnPropertyChanged(nameof(SelectedDopuskSort));
            }
        }


        public ObservableCollection<string> SortClasses { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> SortDopusks { get; } = new ObservableCollection<string>();

        private List<VehicleDTO> _allVehicleDTOs;
        public ObservableCollection<VehicleDTO> VehicleDTOs { get; } = new ObservableCollection<VehicleDTO>();

        private AsyncRelayCommand _btnAddCar;
        public AsyncRelayCommand BtnAddCar
        {
            get
            {
                return _btnAddCar ??
                    (_btnAddCar = new AsyncRelayCommand(
                        async opj=>
                        {
                            //var view = new AddCarsUC();
                            await DialogHost.Show(AddCars, "AddVehicleDialog");
                        }
                        ));
            }
        }


        private AsyncRelayCommand _btnEditCar;
        public AsyncRelayCommand BtnEditCar
        {
            get
            {
                return _btnEditCar ??
                    (_btnEditCar = new AsyncRelayCommand(
                        async obj=>
                        {
                            //var view = new EditCarUC();
                            Debug.WriteLine("----------------------------------------");
                            Debug.WriteLine(obj);
                            Debug.WriteLine("----------------------------------------");
                            await DialogHost.Show(EditCarUC, "AddVehicleDialog");
                        }
                        ));
            }
        }


        private void SearchFilt()
        {
            var list = _allVehicleDTOs.Where(v => v.GovNumber.ToLower().Contains(Search.ToLower()) || v.Model.ToLower().Contains(Search?.ToLower()));

            if (SelectedClassSort != "Все")
            {
                list = list.Where(v=>v.ClassName ==  SelectedClassSort);
            }
            if (SelectedDopuskSort != "Все")
            {
                list = list.Where(v=>v.AdmissionStatus == SelectedDopuskSort);
            }

            VehicleDTOs.Clear();
            foreach (VehicleDTO v in list)
                VehicleDTOs.Add(v);
        }

        public async void Load()
        {
            _allVehicleDTOs = await ManagerService.GetAllVehiclesAsync();
            //SortClasses = new ObservableCollection<string>(_allVehicleDTOs.Select(s => s.ClassName).Distinct());
            //VehicleDTOs = new ObservableCollection<VehicleDTO>( await ManagerService.GetAllVehiclesAsync());
            SortClasses.Clear();
            foreach(string s in _allVehicleDTOs.Select(s => s.ClassName).Distinct())
                SortClasses.Add(s);
            SortClasses.Insert(0, "Все");


            SortDopusks.Clear();
            foreach(string s in _allVehicleDTOs.Select(s => s.AdmissionStatus).Distinct())
                SortDopusks.Add(s);
            SortDopusks.Insert(0, "Все");



            VehicleDTOs.Clear();
            foreach(VehicleDTO v in _allVehicleDTOs)
                VehicleDTOs.Add(v);
        }
    }
}
