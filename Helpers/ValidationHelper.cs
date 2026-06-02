using System.Text.RegularExpressions;
using ClinicQueueManager.Data;
using ClinicQueueManager.Models;
using ClinicQueueManager.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Helpers;

/// <summary>Walidacja formularzy przed zapisem do bazy.</summary>
public static class ValidationHelper
{
    private static readonly Regex PhoneRegex = new(@"^[\d\+\-\s]+$", RegexOptions.Compiled);

    public static bool IsRequired(string? value) => !string.IsNullOrWhiteSpace(value);

    public static bool IsValidEmail(string? email)
    {
        if (!IsRequired(email))
            return false;

        var trimmed = email!.Trim();
        var at = trimmed.IndexOf('@');
        return at > 0 && trimmed.Contains('.') && at < trimmed.LastIndexOf('.');
    }

    public static bool IsValidPhone(string? phone)
    {
        if (!IsRequired(phone))
            return false;

        var trimmed = phone!.Trim();
        if (trimmed.Length < 7 || trimmed.Length > 20)
            return false;

        return PhoneRegex.IsMatch(trimmed) && trimmed.Any(char.IsDigit);
    }

    public static bool IsValidPesel(string? pesel)
    {
        if (!IsRequired(pesel))
            return false;

        var trimmed = pesel!.Trim();
        return trimmed.Length == 11 && trimmed.All(char.IsDigit);
    }

    public static bool IsFutureOrToday(DateTime date) => date.Date >= DateTime.Today;

    public static bool IsNotInFuture(DateTime date) => date.Date <= DateTime.Today;

    public static bool IsValidOfficeNumber(string? number) => IsRequired(number);

    public static bool IsValidDurationMinutes(int minutes) =>
        VisitDurationService.AllowedDurations.Contains(minutes);

    /// <summary>Walidacja danych pacjenta i unikalności PESEL.</summary>
    public static bool ValidatePatient(Patient patient, AppDbContext db, int? excludePatientId, out string error)
    {
        if (!IsRequired(patient.FirstName))
        {
            error = "Imię pacjenta jest wymagane.";
            return false;
        }

        if (!IsRequired(patient.LastName))
        {
            error = "Nazwisko pacjenta jest wymagane.";
            return false;
        }

        if (!IsRequired(patient.Pesel))
        {
            error = "PESEL jest wymagany.";
            return false;
        }

        if (!IsValidPesel(patient.Pesel))
        {
            error = "PESEL musi składać się z 11 cyfr.";
            return false;
        }

        var excludeId = excludePatientId ?? 0;
        if (db.Patients.Any(p => p.Pesel == patient.Pesel.Trim() && p.Id != excludeId))
        {
            error = "Pacjent z takim numerem PESEL już istnieje.";
            return false;
        }

        if (!IsNotInFuture(patient.DateOfBirth))
        {
            error = "Data urodzenia nie może być z przyszłości.";
            return false;
        }

        if (!IsRequired(patient.PhoneNumber))
        {
            error = "Telefon jest wymagany.";
            return false;
        }

        if (!IsValidPhone(patient.PhoneNumber))
        {
            error = "Telefon może zawierać cyfry, spacje, myślniki i znak + (7–20 znaków).";
            return false;
        }

        if (!IsRequired(patient.Email))
        {
            error = "Email jest wymagany.";
            return false;
        }

        if (!IsValidEmail(patient.Email))
        {
            error = "Email musi mieć poprawny format (znak @ i kropka w domenie).";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Walidacja danych lekarza, e-maila i wyboru specjalizacji oraz gabinetu.</summary>
    public static bool ValidateDoctor(
        Doctor doctor,
        int? specializationId,
        int? officeId,
        AppDbContext db,
        int? excludeDoctorId,
        out string error)
    {
        if (!IsRequired(doctor.FirstName))
        {
            error = "Imię lekarza jest wymagane.";
            return false;
        }

        if (!IsRequired(doctor.LastName))
        {
            error = "Nazwisko lekarza jest wymagane.";
            return false;
        }

        if (!IsRequired(doctor.PhoneNumber))
        {
            error = "Telefon jest wymagany.";
            return false;
        }

        if (!IsValidPhone(doctor.PhoneNumber))
        {
            error = "Telefon może zawierać cyfry, spacje, myślniki i znak + (7–20 znaków).";
            return false;
        }

        if (!IsRequired(doctor.Email))
        {
            error = "Email jest wymagany.";
            return false;
        }

        if (!IsValidEmail(doctor.Email))
        {
            error = "Email musi mieć poprawny format (znak @ i kropka w domenie).";
            return false;
        }

        var excludeId = excludeDoctorId ?? 0;
        var email = doctor.Email.Trim();
        if (db.Doctors.Any(d => d.Email == email && d.Id != excludeId))
        {
            error = "Lekarz z tym adresem email już istnieje.";
            return false;
        }

        if (specializationId is null or <= 0)
        {
            error = "Wybierz specjalizację z listy.";
            return false;
        }

        if (officeId is null or <= 0)
        {
            error = "Wybierz gabinet z listy.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Walidacja wizyty, w tym kolizji terminów lekarza i pacjenta.</summary>
    public static bool ValidateAppointment(
        int? patientId,
        int? doctorId,
        DateTime? appointmentDate,
        DateTime? appointmentDateTime,
        bool hasTimeSelected,
        string? statusText,
        string? visitType,
        bool hasDurationSelected,
        int durationMinutes,
        string? reason,
        AppDbContext db,
        int? excludeAppointmentId,
        out string error)
    {
        if (patientId is null or <= 0)
        {
            error = "Wybierz pacjenta.";
            return false;
        }

        if (doctorId is null or <= 0)
        {
            error = "Wybierz lekarza.";
            return false;
        }

        if (appointmentDate == null)
        {
            error = "Wybierz datę wizyty.";
            return false;
        }

        if (!IsFutureOrToday(appointmentDate.Value))
        {
            error = "Data wizyty nie może być z przeszłości.";
            return false;
        }

        if (!hasTimeSelected || appointmentDateTime == null)
        {
            error = "Wybierz godzinę wizyty.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(statusText))
        {
            error = "Wybierz status wizyty.";
            return false;
        }

        if (!IsRequired(visitType))
        {
            error = "Wybierz typ wizyty.";
            return false;
        }

        if (!hasDurationSelected)
        {
            error = "Wybierz czas trwania wizyty.";
            return false;
        }

        if (!IsValidDurationMinutes(durationMinutes))
        {
            error = "Czas trwania musi wynosić 15, 30, 45 lub 60 minut.";
            return false;
        }

        if (!IsRequired(reason))
        {
            error = "Powód wizyty nie może być pusty.";
            return false;
        }

        var status = EnumHelper.ParseStatus(statusText) ?? AppointmentStatus.Zaplanowana;
        var slotStart = appointmentDateTime.Value;

        if (AppointmentSchedulingHelper.HasScheduleOverlap(
                db, doctorId.Value, slotStart, durationMinutes, status, excludeAppointmentId))
        {
            error =
                "Wybrany termin koliduje z inną wizytą lekarza. Zmień godzinę, typ wizyty albo czas trwania.";
            return false;
        }

        if (HasPatientDuplicatePlannedSlot(db, patientId.Value, slotStart, excludeAppointmentId))
        {
            error =
                "Pacjent ma już zaplanowaną wizytę w wybranym terminie. Wybierz inną godzinę lub datę.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool ValidateSpecialization(
        string? name,
        AppDbContext db,
        int? excludeSpecializationId,
        out string error)
    {
        if (!IsRequired(name))
        {
            error = "Nazwa specjalizacji jest wymagana.";
            return false;
        }

        var trimmed = name!.Trim();
        var excludeId = excludeSpecializationId ?? 0;
        if (db.Specializations.Any(s => s.Name == trimmed && s.Id != excludeId))
        {
            error = "Specjalizacja o tej nazwie już istnieje.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool ValidateOffice(
        string? number,
        string? floorText,
        AppDbContext db,
        int? excludeOfficeId,
        out string error,
        out int floor)
    {
        floor = 0;

        if (!IsValidOfficeNumber(number))
        {
            error = "Numer gabinetu jest wymagany.";
            return false;
        }

        var trimmedNumber = number!.Trim();
        var excludeId = excludeOfficeId ?? 0;
        if (db.Offices.Any(o => o.Number == trimmedNumber && o.Id != excludeId))
        {
            error = "Gabinet o tym numerze już istnieje.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(floorText))
        {
            error = "Piętro musi być liczbą całkowitą.";
            return false;
        }

        if (!int.TryParse(floorText.Trim(), out floor))
        {
            error = "Piętro musi być liczbą całkowitą.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool HasPatientDuplicatePlannedSlot(
        AppDbContext db,
        int patientId,
        DateTime appointmentStart,
        int? excludeAppointmentId)
    {
        var excludeId = excludeAppointmentId ?? 0;
        return db.Appointments.Any(a =>
            a.Id != excludeId &&
            a.PatientId == patientId &&
            a.Status == AppointmentStatus.Zaplanowana &&
            a.AppointmentDate.Date == appointmentStart.Date &&
            a.AppointmentDate.Hour == appointmentStart.Hour &&
            a.AppointmentDate.Minute == appointmentStart.Minute);
    }
}
