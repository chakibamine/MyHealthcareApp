namespace MyHealthcareApp.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Medicaments { get; set; }
        public string Posologie { get; set; }
        public int Duration { get; set; }
        public List<string> SideEffects { get; set; } = new List<string>();

        // One-to-One relationship with MedicalRecord
        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
    }
}