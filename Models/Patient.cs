using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

/// <summary>Encja pacjenta.</summary>
public class Patient
{
    public int Id { get; set; }
    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(11)] public string Pesel { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    [MaxLength(20)] public string PhoneNumber { get; set; } = string.Empty;
    [MaxLength(200)] public string Email { get; set; } = string.Empty;
    [MaxLength(300)] public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
