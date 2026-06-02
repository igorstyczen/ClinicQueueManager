using ClinicQueueManager.Models;

namespace ClinicQueueManager.Services;

/// <summary>Oblicza priorytet wizyty (triage) na podstawie objawów i wieku pacjenta.</summary>
public interface IPriorityCalculatorService
{
    AppointmentPriority CalculatePriority(Patient patient, IEnumerable<Symptom> symptoms);
}

public class PriorityCalculatorService : IPriorityCalculatorService
{
    public AppointmentPriority CalculatePriority(Patient patient, IEnumerable<Symptom> symptoms)
    {
        var points = symptoms.Sum(s => s.PriorityPoints);

        var age = DateTime.Today.Year - patient.DateOfBirth.Year;
        if (patient.DateOfBirth.Date > DateTime.Today.AddYears(-age))
        {
            age--;
        }

        if (age > 65)
        {
            points += 2;
        }

        return points switch
        {
            <= 2 => AppointmentPriority.Niski,
            <= 5 => AppointmentPriority.Sredni,
            <= 8 => AppointmentPriority.Wysoki,
            _ => AppointmentPriority.Pilny
        };
    }
}
