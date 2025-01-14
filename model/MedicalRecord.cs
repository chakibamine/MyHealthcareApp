using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyHealthcareApp.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; }
        
        [ValidateNever]
        [JsonIgnore]
        public virtual Patient Patient { get; set; }    
        [ValidateNever]
        [JsonIgnore]
        public Doctor Doctor { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public Prescription Prescription { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public Appointment? Appointment { get; set; }
    }
}