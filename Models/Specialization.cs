using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

public class Specialization
{
    public int Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(400)] public string Description { get; set; } = string.Empty;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
