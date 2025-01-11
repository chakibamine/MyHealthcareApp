namespace MyHealthcareApp.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public bool Availability { get; set; }

        // Relationships
        public int? DoctorId { get; set; } 
        public virtual Doctor? Doctor { get; set; }
    }
}