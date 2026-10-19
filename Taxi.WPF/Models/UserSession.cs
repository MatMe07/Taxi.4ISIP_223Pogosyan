using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.DB;

namespace Taxi.WPF.Models
{
    public class UserSession
    {
        public UserDTO User { get; set; }
        public string access_token {  get; set; }

    }
}
