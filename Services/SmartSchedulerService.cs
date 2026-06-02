using ClinicQueueManager.Data;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Services;

/// <summary>Rekomendacja terminu wizyty wg priorytetu i wolnych slotów lekarza.</summary>
public interface ISmartSchedulerService
{
    AppointmentRecommendation Evaluate(
        AppDbContext db,
        int doctorId,
        DateTime selectedDateTime,
        AppointmentPriority priority,
        int? excludeAppointmentId = null);
}

public class SmartSchedulerService : ISmartSchedulerService
{
    public static readonly string[] ClinicTimeSlots =
    {
        "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
        "11:00", "11:30", "12:00", "12:30", "13:00", "13:30",
        "14:00", "14:30", "15:00"
    };

    public AppointmentRecommendation Evaluate(
        AppDbContext db,
        int doctorId,
        DateTime selectedDateTime,
        AppointmentPriority priority,
        int? excludeAppointmentId = null)
    {
        var occupied = LoadOccupiedSlots(db, doctorId, excludeAppointmentId);
        var selectedIsFree = !occupied.Contains(NormalizeSlot(selectedDateTime));
        var recommendation = new AppointmentRecommendation();

        switch (priority)
        {
            case AppointmentPriority.Niski:
                recommendation.IsSelectedTermAcceptable = selectedIsFree;
                recommendation.Message =
                    "Termin odpowiedni. Wizyta rutynowa może odbyć się w zaplanowanym czasie.";
                recommendation.Explanation = selectedIsFree
                    ? "Niski priorytet nie wymaga przyspieszenia wizyty."
                    : "Wybrany termin jest zajęty — wybierz inną godzinę lub zapisz po korekcie.";
                break;

            case AppointmentPriority.Sredni:
                recommendation.IsSelectedTermAcceptable = selectedIsFree;
                recommendation.Message =
                    "Termin akceptowalny. Objawy wymagają kontroli, ale nie wskazują na przypadek pilny.";
                recommendation.Explanation = selectedIsFree
                    ? "Średni priorytet pozwala na standardowe zaplanowanie wizyty."
                    : "Wybrany termin jest zajęty — wybierz wolny slot przed zapisaniem wizyty.";
                break;

            case AppointmentPriority.Wysoki:
                ApplyHighPriorityRecommendation(recommendation, selectedDateTime, occupied, selectedIsFree,
                    includeNextDay: true, onlySameDay: false);
                break;

            case AppointmentPriority.Pilny:
                ApplyUrgentPriorityRecommendation(recommendation, selectedDateTime, occupied, selectedIsFree);
                break;

            default:
                recommendation.Message = "Brak oceny terminu dla wybranego priorytetu.";
                break;
        }

        return recommendation;
    }

    private static void ApplyHighPriorityRecommendation(
        AppointmentRecommendation recommendation,
        DateTime selectedDateTime,
        HashSet<DateTime> occupied,
        bool selectedIsFree,
        bool includeNextDay,
        bool onlySameDay)
    {
        recommendation.IsSelectedTermAcceptable = selectedIsFree;

        var earlier = FindEarlierFreeSlot(selectedDateTime, occupied, includeNextDay, onlySameDay);
        if (earlier.HasValue)
        {
            SetSuggestedTerm(recommendation, earlier.Value);
            recommendation.Message =
                "Pacjent ma podwyższony priorytet. System znalazł wcześniejszy wolny termin.";
            recommendation.Explanation =
                $"Sugerowany termin: {earlier.Value:yyyy-MM-dd} o {earlier.Value:HH:mm}. " +
                "Kliknij „Użyj sugerowanego terminu”, aby wypełnić formularz — zapis wykonasz ręcznie.";
        }
        else
        {
            recommendation.Message =
                "Pacjent ma podwyższony priorytet, ale brak wcześniejszego wolnego terminu u wybranego lekarza.";
            recommendation.Explanation = includeNextDay
                ? "Sprawdzono wcześniejsze godziny w wybranym dniu oraz wolne terminy następnego dnia."
                : "Sprawdzono wcześniejsze godziny w wybranym dniu u wybranego lekarza.";
        }
    }

    private static void ApplyUrgentPriorityRecommendation(
        AppointmentRecommendation recommendation,
        DateTime selectedDateTime,
        HashSet<DateTime> occupied,
        bool selectedIsFree)
    {
        recommendation.IsSelectedTermAcceptable = selectedIsFree;

        var earlier = FindEarlierFreeSlot(selectedDateTime, occupied, includeNextDay: false, onlySameDay: true);
        if (earlier.HasValue)
        {
            SetSuggestedTerm(recommendation, earlier.Value);
            recommendation.Message =
                "Pacjent ma priorytet pilny. System zaleca najwcześniejszy dostępny termin.";
            recommendation.Explanation =
                $"Najwcześniejszy wolny termin tego dnia: {earlier.Value:HH:mm}. " +
                "Kliknij „Użyj sugerowanego terminu”, aby wypełnić formularz — zapis wykonasz ręcznie.";
        }
        else
        {
            recommendation.Message =
                "Pacjent ma priorytet pilny, ale brak wcześniejszego wolnego terminu u wybranego lekarza.";
            recommendation.Explanation =
                "W wybranym dniu nie ma wcześniejszej wolnej godziny u tego lekarza. Rozważ inny dzień lub lekarza.";
        }
    }

    private static void SetSuggestedTerm(AppointmentRecommendation recommendation, DateTime slot)
    {
        recommendation.HasSuggestedTerm = true;
        recommendation.SuggestedDate = slot.Date;
        recommendation.SuggestedTime = slot.ToString("HH:mm");
    }

    private static DateTime? FindEarlierFreeSlot(
        DateTime selectedDateTime,
        HashSet<DateTime> occupied,
        bool includeNextDay,
        bool onlySameDay)
    {
        var candidates = new List<DateTime>();

        foreach (var slot in EnumerateSlotsOnDate(selectedDateTime.Date))
        {
            if (slot < selectedDateTime && !occupied.Contains(slot))
                candidates.Add(slot);
        }

        if (!onlySameDay && includeNextDay && candidates.Count == 0)
        {
            foreach (var slot in EnumerateSlotsOnDate(selectedDateTime.Date.AddDays(1)))
            {
                if (!occupied.Contains(slot))
                    candidates.Add(slot);
            }
        }

        return candidates.Count == 0 ? null : candidates.Min();
    }

    private static IEnumerable<DateTime> EnumerateSlotsOnDate(DateTime date)
    {
        foreach (var time in ClinicTimeSlots)
        {
            var parts = time.Split(':');
            yield return new DateTime(date.Year, date.Month, date.Day,
                int.Parse(parts[0]), int.Parse(parts[1]), 0);
        }
    }

    private static HashSet<DateTime> LoadOccupiedSlots(
        AppDbContext db,
        int doctorId,
        int? excludeAppointmentId)
    {
        var excludeId = excludeAppointmentId ?? 0;

        return db.Appointments
            .Where(a =>
                a.Id != excludeId &&
                a.DoctorId == doctorId &&
                a.Status == AppointmentStatus.Zaplanowana)
            .AsEnumerable()
            .Select(a => NormalizeSlot(a.AppointmentDate))
            .ToHashSet();
    }

    private static DateTime NormalizeSlot(DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
}
