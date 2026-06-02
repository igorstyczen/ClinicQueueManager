using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

public class Office
{
    public int Id { get; set; }
    [MaxLength(30)] public string Number { get; set; } = string.Empty;
    public int Floor { get; set; }
    [MaxLength(300)] public string Description { get; set; } = string.Empty;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
