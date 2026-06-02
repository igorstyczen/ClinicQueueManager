using ClinicQueueManager.Data;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Services;

/// <summary>Sprawdzanie nakładających się wizyt lekarza w danym dniu.</summary>
public static class AppointmentSchedulingHelper
{
    /// <summary>Domyślny czas trwania wizyty, gdy w rekordzie brak wartości.</summary>
    public const int DefaultDurationMinutes = 30;

    public static bool IsDoctorSlotConflict(
        AppDbContext db,
        int doctorId,
        DateTime appointmentDateTime,
        AppointmentStatus status,
        int? excludeAppointmentId = null,
        int durationMinutes = DefaultDurationMinutes) =>
        HasScheduleOverlap(db, doctorId, appointmentDateTime, durationMinutes, status, excludeAppointmentId);

    /// <summary>Czy nowy slot nakłada się na zaplanowaną wizytę lekarza.</summary>
    public static bool HasScheduleOverlap(
        AppDbContext db,
        int doctorId,
        DateTime appointmentStart,
        int durationMinutes,
        AppointmentStatus status,
        int? excludeAppointmentId = null)
    {
        if (status != AppointmentStatus.Zaplanowana)
            return false;

        if (durationMinutes <= 0)
            durationMinutes = DefaultDurationMinutes;

        var excludeId = excludeAppointmentId ?? 0;
        var newStart = NormalizeSlot(appointmentStart);
        var newEnd = newStart.AddMinutes(durationMinutes);

        var sameDayPlanned = db.Appointments
            .Where(a =>
                a.Id != excludeId &&
                a.DoctorId == doctorId &&
                a.Status == AppointmentStatus.Zaplanowana &&
                a.AppointmentDate.Date == appointmentStart.Date)
            .AsEnumerable();

        foreach (var existing in sameDayPlanned)
        {
            var existingStart = NormalizeSlot(existing.AppointmentDate);
            var existingDuration = existing.DurationMinutes > 0
                ? existing.DurationMinutes
                : DefaultDurationMinutes;
            var existingEnd = existingStart.AddMinutes(existingDuration);

            if (newStart < existingEnd && existingStart < newEnd)
                return true;
        }

        return false;
    }

    private static DateTime NormalizeSlot(DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
}
