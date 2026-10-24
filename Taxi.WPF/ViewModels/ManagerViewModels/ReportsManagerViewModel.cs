using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.Core.Models.Export;
using Taxi.WPF.Commands;
using Taxi.WPF.Services;
using Windows.Foundation.Collections;

namespace Taxi.WPF.ViewModels.ManagerViewModels
{
    public class ReportsManagerViewModel : ViewModelBase
    {
        VehiclesServices _vehiclesServices;
        PoezdkaService _poezdkaService;
        GenerationFilesService _generationFilesService;
        public ReportsManagerViewModel(VehiclesServices vehiclesServices, PoezdkaService poezdkaService, GenerationFilesService generationFilesService)
        {
            _vehiclesServices = vehiclesServices;
            _poezdkaService = poezdkaService;
            _generationFilesService = generationFilesService;

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

        private string _selectedClassSort;
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

        public List<PoezdkaDTO> poezdkaDTOs { get; set; }


        public decimal TotalEarning => _rangePoezdka?.Sum(p => p.Earnings) ?? 0;

        public int TotalCountOrder  => _rangePoezdka?.Sum(p => p.OrderId) ?? 0;

        public ObservableCollection<decimal> Values1 { get; set; } = new ObservableCollection<decimal>();

        public ObservableCollection<string> Labels { get; } = new ObservableCollection<string>();

        public Func<double, string> Labeler { get; set; } =
            value => value.ToString();

        private List<PoezdkaDTO> _rangePoezdka;

        private RelayCommand _makeDiagrammsCommand;
        public RelayCommand MakeDiagrammsCommand
        {
            get => _makeDiagrammsCommand ??
                (_makeDiagrammsCommand = new RelayCommand(obj =>
                {
                    if (poezdkaDTOs == null) return;
                    _rangePoezdka = poezdkaDTOs.Where(p => p.StartedAt >= SelectStartDate && p.FinishedAt <= SelectEndDate).ToList();
                    if (SelectedClassSort != "Все") _rangePoezdka = _rangePoezdka.Where(p => p.VehicleClassName == SelectedClassSort).ToList();
                    var gosNumbers = _rangePoezdka.Select(s => s.VehicleGosNumber);
                    Labels.Clear();
                    foreach (var g in gosNumbers)
                        Labels.Add(g);

                    var viruchkaVehicles = _rangePoezdka.Select(s => s.Earnings);
                    Values1.Clear();
                    foreach (var v in viruchkaVehicles) Values1.Add(v);

                    OnPropertyChanged(nameof(TotalCountOrder));
                    OnPropertyChanged(nameof(TotalEarning));
                    ClikedBtn = false;
                    VisibleDiagramBtn = true;
                }));
        }

        private string AskUserFIlePath(string filter, string defaultFilt)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { 
                Filter= filter,
                DefaultExt = defaultFilt,
                FileName="ВыручкаЗаОпределенныйПериод"
            
            };
            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null ;
        }

        private async Task GenExcelAsync(string filePath)
        {

            var request = new ExportExcelRequest
            {
                fileName = filePath,
                ColumnNames = new List<string> { "Госномер", "Выручка", "Заказов"},
                DataEarnings = _rangePoezdka.Select(p => new DataEarnings { Earnings = p.Earnings, GosNumber = p.VehicleGosNumber, totalOrder = _rangePoezdka.Count(a => a.OrderId == p.OrderId) }).ToList()
            };

            byte[] excelBytes = await _generationFilesService.GenerateExcelAsync(request);
            Debug.WriteLine(excelBytes);

            await File.WriteAllBytesAsync(filePath, excelBytes);
        }


        private async Task GenPDFAsync(string filePath)
        {

            var request = new ExportPDFRequest
            {

            };
            byte[] excelBytes = await _generationFilesService.GeneratePdfAsync(request);
            await File.WriteAllBytesAsync(filePath, excelBytes);
        }
        private async Task GenWordAsync(string filePath)
        {

            var request = new ExportWordRequest
            {

            };
            byte[] excelBytes = await _generationFilesService.GenerateWordAsync(request);
            await File.WriteAllBytesAsync(filePath, excelBytes);
        }




        private AsyncRelayCommand _exportToFile;
        public AsyncRelayCommand ExportToFileCommand
        {
            get => _exportToFile ??
                (_exportToFile = new AsyncRelayCommand(async obj =>
                {
                    string askUsFilePath;
                    switch (obj)
                    {
                        case "Excel":
                            {
                                askUsFilePath = AskUserFIlePath("Excel (*.xlsx)|*xlsx", ".xlsx");
                                if (askUsFilePath == null)
                                {
                                    return;
                                }
                                await GenExcelAsync(askUsFilePath);
                                break;
                            }
                        case "PDF":
                            {
                                askUsFilePath = AskUserFIlePath("PDF (*.pdf)|*pdf", ".pdf");
                                if (askUsFilePath == null)
                                {
                                    return;
                                }
                                await GenPDFAsync(askUsFilePath);

                                break;
                            }
                        case "Word":
                            {
                                askUsFilePath = AskUserFIlePath("Word (*.docx)|*docx", ".docx");
                                if (askUsFilePath == null)
                                {
                                    return;
                                }

                                await GenWordAsync(askUsFilePath);

                                break;
                            }

                    }
                }
                ));
        }

        public async Task Load()
        {
            poezdkaDTOs = await _poezdkaService.GetAllPoezdkasAsync();
            var sort_cl = await _vehiclesServices.GetAllVehicleClassesAsync();
            ClikedBtn = true;
            VisibleDiagramBtn = false;

            SortClasses.Clear();
            foreach (var s in sort_cl.Select(c=>c.Name))
                SortClasses.Add(s);
            SortClasses.Insert(0, "Все");

            SelectedClassSort = "Все";

        }
    }
}
