using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Taxi.WPF.ViewModels.UsersViewModels
{
    public class DriverViewModel : ViewModelBase
    {
        public Action<string, object> CallJsAsync { get; set; }


        public async Task WebMessage(JsonElement root)
        {
            if (!root.TryGetProperty("type", out JsonElement typeElement))
                return;

            switch (typeElement.GetString())
            {
            }
        }
    }
}
