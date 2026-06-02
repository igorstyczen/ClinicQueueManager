using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

public class Doctor
{
    public int Id { get; set; }
    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(20)] public string PhoneNumber { get; set; } = string.Empty;
    [MaxLength(200)] public string Email { get; set; } = string.Empty;

    public int SpecializationId { get; set; }
    public int OfficeId { get; set; }

    public Specialization? Specialization { get; set; }
    public Office? Office { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
