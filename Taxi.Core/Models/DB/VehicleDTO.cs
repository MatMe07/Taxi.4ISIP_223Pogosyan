using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Taxi.Core.Models.DB
{
    public class VehicleDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("govNumber")]
        public string GovNumber { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("classId")]
        public int ClassId { get; set; }

        [JsonPropertyName("className")]
        public string ClassName { get; set; } = "";


        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = false;

        //public string dopuskText { get; set; }
        public bool HasActiveMaintenance { get; set; }
        public bool HasOsago { get; set; }
        public bool HasTechCheck { get; set; }
        public bool HasPermit { get; set; }
        public string AdmissionStatus =>
            !IsActive ? "Архив" :
        HasActiveMaintenance ? "На ТО" :
        !HasOsago ? "Нет ОСАГО" :
        !HasTechCheck ? "Нет диагн. карты" :
        !HasPermit ? "Нет разрешения" : "Готово";
    }
}
