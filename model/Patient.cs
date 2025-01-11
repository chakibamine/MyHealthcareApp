using System.Collections.Generic;

namespace MyHealthcareApp.Models
{
    public class Patient
    {
        public Patient()
        {
            MedicalRecords = new List<MedicalRecord>();
            Appointments = new List<Appointment>();
            Bills = new List<Billing>();
        }

        public int Id { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Relationships
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Billing> Bills { get; set; }
    }
}