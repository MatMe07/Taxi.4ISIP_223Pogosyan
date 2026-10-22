using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Taxi.Core.Models.DB;
using Taxi.WPF.Commands;
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

        private bool CheckField()
        {

            return !string.Equals(NewVehicle.Brand, EditVehicle.Brand) ||
                !string.Equals(NewVehicle.Model, EditVehicle.Model) ||
                NewVehicle.ClassId != EditVehicle.ClassId ||
                NewVehicle.Year !=  EditVehicle.Year || 
                NewVehicle.IsActive !=  EditVehicle.IsActive;
        }


        private AsyncRelayCommand _deleteCommand;
        public AsyncRelayCommand DeleteCommand
        {
            get => _deleteCommand ??
                (_deleteCommand = new AsyncRelayCommand(async obj =>
                {
                    var res = MessageBox.Show(
                        "Действительно ходите удалить?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (res != MessageBoxResult.Yes)
                        return;


                    var result = await _vehiclesServices.DeleteVehicleAsync(NewVehicle);
                    MessageBox.Show(
                        result.message,
                        result.IsDeleted ? "Готово" : "Ошибка",
                        MessageBoxButton.OK,
                        result.IsDeleted ? MessageBoxImage.Information : MessageBoxImage.Warning);

                    if (result.IsDeleted)
                        DialogHost.Close("AddVehicleDialog", (EditVehicle, true));

                }));
        }


        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get => _cancelCommand ??
                (_cancelCommand = new RelayCommand(obj =>
                {
                    if (CheckField())
                    {
                        var res = MessageBox.Show("Вы действительно хотите выйти? Данные потеряются!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (res == MessageBoxResult.Yes)
                        {
                            DialogHost.Close("AddVehicleDialog");
                            return;
                        }

                    }
                    else
                        DialogHost.Close("AddVehicleDialog");

                }));
        }



        private AsyncRelayCommand _saveCommand;
        public AsyncRelayCommand SaveCommand
        {
            get => _saveCommand ??
                (_saveCommand = new AsyncRelayCommand(async obj =>
                {
                    if (!CheckField())
                    {
                        MessageBox.Show(
                            "Проверьте корректность заполнения полей.",
                            "Ошибка валидации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    var res = MessageBox.Show(
                        "Сохранить изменения?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (res != MessageBoxResult.Yes)
                        return;


                    var result = await _vehiclesServices.UpdateVehicleAsync(NewVehicle);
                    MessageBox.Show(
                        result.message,
                        result.IsUpdateded ? "Готово" : "Ошибка",
                        MessageBoxButton.OK,
                        result.IsUpdateded ? MessageBoxImage.Information : MessageBoxImage.Warning);

                    if (result.IsUpdateded)
                        DialogHost.Close("AddVehicleDialog", (NewVehicle, false));

                }));
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
                if (NewVehicle != null && SelectedVehicleClass != null)
                {
                    NewVehicle.ClassName = SelectedVehicleClass.Name;
                    NewVehicle.ClassId = SelectedVehicleClass.Id;

                }
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
