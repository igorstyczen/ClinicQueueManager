using ClinicQueueManager.Models;

namespace ClinicQueueManager.Helpers;

public class AppointmentGridRow
{
    public int Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string VisitType { get; set; } = string.Empty;
    public string Patient { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;
    public string Doctor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;
    public AppointmentPriority PriorityEnum { get; set; }
}
