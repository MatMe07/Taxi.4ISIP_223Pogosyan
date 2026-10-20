using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Core.Models.Auth
{
    public class ChangePasswordRequst
    {
        public string PasswordCurrent {  get; set; }
        public string PasswordNew {  get; set; }
        public string PasswordNewConfirm {  get; set; }
        
    }
}
