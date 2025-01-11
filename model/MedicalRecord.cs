namespace MyHealthcareApp.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public string Diagnosis { get; set; }
        
        public DateTime Date { get; set; }

        // Relationships
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        // One-to-One relationship with Prescription
        public Prescription Prescription { get; set; }
    }
}