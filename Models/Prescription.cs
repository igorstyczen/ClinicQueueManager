using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

public class Prescription
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    [MaxLength(200)] public string MedicineName { get; set; } = string.Empty;
    [MaxLength(100)] public string Dosage { get; set; } = string.Empty;
    [MaxLength(500)] public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Appointment? Appointment { get; set; }
}
