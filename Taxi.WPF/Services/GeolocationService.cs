using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.WPF.Services;
using Windows.Devices.Geolocation;

namespace Taxi.src.Services
{
    public class GeolocationService
    {
        public static async Task<Geoposition> GetLocationAsync()
        {
            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            return await geolocator.GetGeopositionAsync();
        }
    }
}
