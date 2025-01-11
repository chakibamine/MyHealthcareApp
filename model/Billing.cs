namespace MyHealthcareApp.Models
{
    public class Billing
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

        // Relationships
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}