using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Models;
using Taxi.WPF.Services.Interfaces;

namespace Taxi.WPF.Services
{
    public class UserSessionService : IUserSessionService
    {
        public UserSession? CurrentSession { get; private set; }

        public bool IsLoggedIn => CurrentSession?.access_token.AccesToken != null;


        public void Clear()
        {
            CurrentSession = null;
        }

        public void SetSession(UserSession session)
        {
            CurrentSession = session;
        }

    }
}
