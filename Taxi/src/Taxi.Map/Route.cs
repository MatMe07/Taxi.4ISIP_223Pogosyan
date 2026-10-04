using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Taxi.src.Taxi.Map
{
    public class RoutePoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Address { get; set; }
    }
    public class RouteInfo
    {
        public double DistanceMeters { get; set; }
        public double DurationSeconds { get; set; }
        public List<double[]> Coordinates { get; set; } = new List<double[]>();
    }


    public class Polyline
    {
        [JsonPropertyName("points")]
        public List<List<double>> Points { get; set; }
    }

    public class RouteSegment
    {
        [JsonPropertyName("length")]
        public double Length { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("waiting_duration")]
        public double WaitingDuration { get; set; }

        [JsonPropertyName("polyline")]
        public Polyline Polyline { get; set; }
    }
}
