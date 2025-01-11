using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyHealthcareApp.Models
{
    public class Doctor
    {
        public Doctor()
        {
            MedicalRecords = new List<MedicalRecord>();
            Appointments = new List<Appointment>();
            
        }

        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Specialite { get; set; }
        public int Experience { get; set; }
        public double Rating { get; set; }
          [ValidateNever]
        [JsonIgnore]
        public string? Password { get; set; }

        // Relationships
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        
    }
}