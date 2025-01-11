namespace MyHealthcareApp.Models
{
    public class MedicationStock
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int Quantite { get; set; }
        public string DateExpiration { get; set; }
        public string StorageLocation { get; set; }
        public bool IsCritical { get; set; }
    }
}