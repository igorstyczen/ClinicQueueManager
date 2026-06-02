using System.ComponentModel.DataAnnotations;

namespace ClinicQueueManager.Models;

public class Symptom
{
    public int Id { get; set; }
    [MaxLength(150)] public string Name { get; set; } = string.Empty;
    public int PriorityPoints { get; set; }

    public ICollection<AppointmentSymptom> AppointmentSymptoms { get; set; } = new List<AppointmentSymptom>();
}
