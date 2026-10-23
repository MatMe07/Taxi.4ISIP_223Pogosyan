using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.WPF.Services;
using Windows.Foundation.Collections;

namespace Taxi.WPF.ViewModels.ManagerViewModels
{
    public class ReportsManagerViewModel : ViewModelBase
    {
        VehiclesServices _vehiclesServices;
        PoezdkaService _poezdkaService;
        public ReportsManagerViewModel(VehiclesServices vehiclesServices, PoezdkaService poezdkaService)
        {
            _vehiclesServices = vehiclesServices;
            _poezdkaService = poezdkaService;

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

        private bool _clikedBtn = false;
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



        private bool _visibleDiagramBtn = true;
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

        public List<PoezdkaDTO> poezdkaDTOs { get; set; }


        public decimal TotalEarning => poezdkaDTOs?.Sum(p => p.Earnings) ?? 0;

        public int TotalCountOrder  => poezdkaDTOs?.Sum(p => p.OrderId) ?? 0;

        public ObservableCollection<decimal> Values1 { get; set; } = new ObservableCollection<decimal>();

        public ObservableCollection<string> Labels { get; } = new ObservableCollection<string>();

        public Func<double, string> Labeler { get; set; } =
            value => value.ToString();

        public async Task Load()
        {
            poezdkaDTOs = await _poezdkaService.GetAllPoezdkasAsync();
            var sort_cl = await _vehiclesServices.GetAllVehicleClassesAsync();
            _clikedBtn = true;
            _visibleDiagramBtn = false;
            SortClasses.Clear();
            foreach (var s in sort_cl.Select(c=>c.Name))
                SortClasses.Add(s);
            SortClasses.Insert(0, "Все");

            var gosNumbers = poezdkaDTOs.Select(s => s.VehicleGosNumber);
            Labels.Clear();
            foreach (var g in gosNumbers)
                Labels.Add(g);

            var viruchkaVehicles = poezdkaDTOs.Select(s => s.Earnings);
            Values1.Clear();
            foreach ( var v in viruchkaVehicles) Values1.Add(v);

            OnPropertyChanged(nameof(TotalCountOrder));
            OnPropertyChanged(nameof(TotalEarning));
        }
    }
}
