using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.WPF.Services;

namespace Taxi.WPF.ViewModels.ManagerViewModels
{
    public class ReportsManagerViewModel : ViewModelBase
    {
        VehiclesServices _vehiclesServices;
        public ReportsManagerViewModel(VehiclesServices vehiclesServices)
        {
            _vehiclesServices = vehiclesServices;

        }
        public ObservableCollection<string> SortClasses { get; } = new ObservableCollection<string>();

        private DateTime _selectStartDate = DateTime.Now;
        public DateTime SelectStartDate
        {
            get { return _selectStartDate; } 
            set { 
                _selectStartDate = value;
                OnPropertyChanged(nameof(SelectStartDate));
            }
        }
        private DateTime _selectEndDate = DateTime.Now.AddDays(5) ;
        public DateTime SelectEndDate
        {
            get { return _selectEndDate; } 
            set { 
                _selectEndDate = value;
                OnPropertyChanged(nameof(SelectEndDate));
            }
        }

        private string _selectedClassSort = "Все";
        public string SelectedClassSort
        {
            get => _selectedClassSort;
            set
            {
                _selectedClassSort = value;
                OnPropertyChanged(nameof(SelectedClassSort));
            }
        }

        private bool _clikedBtn = true;
        public bool ClikedBtn
        {
            get
            {
                return _clikedBtn;
            }
            set
            {
                _clikedBtn = value;
                OnPropertyChanged(nameof(ClikedBtn));
            }
        }

        private bool _visibleDiagramBtn = false;
        public bool VisibleDiagramBtn
        {
            get
            {
                return _visibleDiagramBtn;
            }
            set
            {
                _visibleDiagramBtn = value;
                OnPropertyChanged(nameof(VisibleDiagramBtn));
            }
        }

        public async Task Load()
        {
            //_allVehicleDTOs = await vehiclesServices.GetAllVehiclesAsync();
            var sort_cl = await _vehiclesServices.GetAllVehicleClassesAsync();
            _clikedBtn = true;
            _visibleDiagramBtn = false;
            SortClasses.Clear();
            foreach (var s in sort_cl.Select(c=>c.Name))
                SortClasses.Add(s);
            SortClasses.Insert(0, "Все");

        }
    }
}
