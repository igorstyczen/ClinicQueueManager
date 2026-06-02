using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Zaplanowana;
    [MaxLength(300)] public string Reason { get; set; } = string.Empty;
    [MaxLength(100)] public string VisitType { get; set; } = "Konsultacja ogólna";
    public AppointmentPriority Priority { get; set; } = AppointmentPriority.Niski;
    public int DurationMinutes { get; set; } = 30;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public ICollection<AppointmentSymptom> AppointmentSymptoms { get; set; } = new List<AppointmentSymptom>();
    public Prescription? Prescription { get; set; }
    public Payment? Payment { get; set; }
}
