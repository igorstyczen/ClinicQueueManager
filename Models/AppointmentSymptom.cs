namespace ClinicQueueManager.Models;

public class AppointmentSymptom
{
    public int AppointmentId { get; set; }
    public int SymptomId { get; set; }

    public Appointment? Appointment { get; set; }
    public Symptom? Symptom { get; set; }
}
