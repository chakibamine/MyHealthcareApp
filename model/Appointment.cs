using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace MyHealthcareApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Heure { get; set; }
        public string Statut { get; set; }
        public string Notes { get; set; }

        // Foreign keys
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Navigation properties (ignored during validation)
        [ValidateNever]
        [JsonIgnore]
        public Patient Patient { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public Doctor Doctor { get; set; }
    }
}