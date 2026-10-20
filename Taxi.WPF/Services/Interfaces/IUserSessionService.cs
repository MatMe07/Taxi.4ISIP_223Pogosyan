using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Models;

namespace Taxi.WPF.Services.Interfaces
{
    public interface IUserSessionService
    {
        public UserSession CurrentSession { get; }
        public bool IsLoggedIn { get; }
        void SetSession(UserSession session);
        void Clear();
    }
}
