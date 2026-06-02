using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Data;

/// <summary>Kontekst EF Core dla bazy SQLite clinic.db.</summary>
public class AppDbContext : DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Office> Offices => Set<Office>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Symptom> Symptoms => Set<Symptom>();
    public DbSet<AppointmentSymptom> AppointmentSymptoms => Set<AppointmentSymptom>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=clinic.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentSymptom>()
            .HasKey(x => new { x.AppointmentId, x.SymptomId });

        modelBuilder.Entity<Patient>()
            .HasIndex(x => x.Pesel)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasOne(x => x.Patient)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(x => x.Doctor)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(x => x.Specialization)
            .WithMany(x => x.Doctors)
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(x => x.Office)
            .WithMany(x => x.Doctors)
            .HasForeignKey(x => x.OfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppointmentSymptom>()
            .HasOne(x => x.Appointment)
            .WithMany(x => x.AppointmentSymptoms)
            .HasForeignKey(x => x.AppointmentId);

        modelBuilder.Entity<AppointmentSymptom>()
            .HasOne(x => x.Symptom)
            .WithMany(x => x.AppointmentSymptoms)
            .HasForeignKey(x => x.SymptomId);

        modelBuilder.Entity<Appointment>()
            .HasOne(x => x.Prescription)
            .WithOne(x => x.Appointment)
            .HasForeignKey<Prescription>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(x => x.Payment)
            .WithOne(x => x.Appointment)
            .HasForeignKey<Payment>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
