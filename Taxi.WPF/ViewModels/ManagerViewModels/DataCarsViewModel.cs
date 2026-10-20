using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;
using Taxi.WPF.Services;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.ViewModels.ManagerViewModels
{
    public class DataCarsViewModel : ViewModelBase
    {

        ManagerService ManagerService;
        public DataCarsViewModel(IUserBaseService managerService)
        {
            ManagerService = managerService as ManagerService;
        }


        private List<VehicleDTO> vehicleDTOs;
        public async void Load()
        {
            vehicleDTOs = await ManagerService.GetAllVehiclesAsync();

        }
    }
}
