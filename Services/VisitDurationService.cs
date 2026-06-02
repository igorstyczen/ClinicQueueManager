using ClinicQueueManager.Models;

namespace ClinicQueueManager.Services;

/// <summary>Sugeruje czas trwania wizyty na podstawie typu wizyty (nie priorytetu).</summary>
public interface IVisitDurationService
{
    int GetSuggestedDuration(string? visitType);
    string GetDurationExplanation(string? visitType);
    string? GetPriorityWarning(AppointmentPriority priority, string? visitType);
}

public class VisitDurationService : IVisitDurationService
{
    public static readonly string[] VisitTypeOptions =
    {
        "Konsultacja ogólna",
        "Kontrola",
        "Wypisanie recepty",
        "Badanie",
        "Zabieg",
        "Pierwsza wizyta",
        "Pilna konsultacja",
        "Szczepienie"
    };

    public static readonly int[] AllowedDurations = { 15, 30, 45, 60 };

    private static readonly Dictionary<string, int> DurationByVisitType = new()
    {
        ["Konsultacja ogólna"] = 30,
        ["Kontrola"] = 15,
        ["Wypisanie recepty"] = 15,
        ["Badanie"] = 30,
        ["Zabieg"] = 60,
        ["Pierwsza wizyta"] = 45,
        ["Pilna konsultacja"] = 45,
        ["Szczepienie"] = 15
    };

    private static readonly HashSet<string> RoutineVisitTypesForWarning = new(StringComparer.Ordinal)
    {
        "Kontrola",
        "Wypisanie recepty",
        "Szczepienie"
    };

    public int GetSuggestedDuration(string? visitType)
    {
        if (string.IsNullOrWhiteSpace(visitType))
            return 30;

        return DurationByVisitType.TryGetValue(visitType, out var minutes)
            ? minutes
            : 30;
    }

    public string GetDurationExplanation(string? visitType)
    {
        if (string.IsNullOrWhiteSpace(visitType))
            return "Wybierz typ wizyty, aby zobaczyć sugerowany czas trwania.";

        var minutes = GetSuggestedDuration(visitType);
        return $"Typ wizyty „{visitType}” wymaga odpowiedniego czasu w grafiku, dlatego sugerowany czas to {minutes} minut.";
    }

    public string? GetPriorityWarning(AppointmentPriority priority, string? visitType)
    {
        if (priority is not (AppointmentPriority.Wysoki or AppointmentPriority.Pilny))
            return null;

        if (string.IsNullOrWhiteSpace(visitType) || !RoutineVisitTypesForWarning.Contains(visitType))
            return null;

        return "Uwaga: objawy wskazują na podwyższony priorytet. Rozważ zmianę typu wizyty na „Pilna konsultacja” lub wydłużenie czasu wizyty.";
    }
}
