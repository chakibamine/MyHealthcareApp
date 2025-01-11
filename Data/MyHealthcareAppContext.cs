using Microsoft.EntityFrameworkCore;

namespace MyHealthcareApp.Models
{
    public class MyHealthcareAppContext : DbContext
    {
        public MyHealthcareAppContext(DbContextOptions<MyHealthcareAppContext> options)
            : base(options)
        {
        }

        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicationManager> MedicationManagers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<MedicationStock> MedicationStocks { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships

            // Patient -> MedicalRecords (One-to-Many)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.MedicalRecords)
                .WithOne(m => m.Patient)
                .HasForeignKey(m => m.PatientId);

            // Patient -> Appointments (One-to-Many)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId);

            // Patient -> Bills (One-to-Many)
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Bills)
                .WithOne(b => b.Patient)
                .HasForeignKey(b => b.PatientId);

            // Doctor -> MedicalRecords (One-to-Many)
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.MedicalRecords)
                .WithOne(m => m.Doctor)
                .HasForeignKey(m => m.DoctorId);

            // Doctor -> Appointments (One-to-Many)
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId);

            // Room -> Doctor (One-to-One)
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId);

            // MedicalRecord -> Prescription (One-to-One)
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Prescription)
                .WithOne(p => p.MedicalRecord)
                .HasForeignKey<Prescription>(p => p.MedicalRecordId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete Appointments when Patient is deleted

            // Doctor -> Appointments (One-to-Many)
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete Appointments when Doctor is deleted

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Doctor)
                .WithMany()
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            base.OnModelCreating(modelBuilder);
        }
    }
}