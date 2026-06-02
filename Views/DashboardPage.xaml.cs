using System.Windows;
using System.Windows.Controls;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using ClinicQueueManager.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Views;

public partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadDashboard();
        AppDataEvents.AppointmentsChanged += OnAppointmentsChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) =>
        AppDataEvents.AppointmentsChanged -= OnAppointmentsChanged;

    private void OnAppointmentsChanged(object? sender, EventArgs e) =>
        Dispatcher.Invoke(LoadDashboard);

    private void LoadDashboard()
    {
        try
        {
            using var db = new AppDbContext();
            var today = DateTime.Today;

            PatientsCountText.Text = db.Patients.Count().ToString();
            DoctorsCountText.Text = db.Doctors.Count().ToString();
            TodayVisitsCountText.Text = db.Appointments
                .Count(a => a.AppointmentDate.Date == today)
                .ToString();
            UrgentTodayCountText.Text = db.Appointments
                .Count(a => a.AppointmentDate.Date == today && a.Priority == AppointmentPriority.Pilny)
                .ToString();

            var todaySchedule = db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentDate)
                .ToList()
                .Select(ToScheduleRow)
                .ToList();
            BindSection(TodayScheduleGrid, TodayScheduleEmptyText, todaySchedule);

            var urgentCases = db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Priority == AppointmentPriority.Pilny ||
                            a.Priority == AppointmentPriority.Wysoki)
                .ToList()
                .OrderBy(a => a.AppointmentDate.Date == today ? 0 : 1)
                .ThenBy(a => a.AppointmentDate)
                .Take(5)
                .Select(ToUrgentRow)
                .ToList();
            BindSection(UrgentCasesGrid, UrgentCasesEmptyText, urgentCases);

            var futureVisits = db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate.Date > today)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList()
                .Select(ToFutureRow)
                .ToList();
            BindSection(FutureVisitsGrid, FutureVisitsEmptyText, futureVisits);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd dashboardu: {ex.Message}", "Błąd",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void BindSection<T>(DataGrid grid, TextBlock emptyText, IList<T> items)
    {
        var hasItems = items.Count > 0;
        grid.ItemsSource = hasItems ? items : null;
        grid.Visibility = hasItems ? Visibility.Visible : Visibility.Collapsed;
        emptyText.Visibility = hasItems ? Visibility.Collapsed : Visibility.Visible;
    }

    private static AppointmentGridRow ToScheduleRow(Appointment a) => new()
    {
        Id = a.Id,
        Time = a.AppointmentDate.ToString("HH:mm"),
        Patient = FormatName(a.Patient),
        Doctor = FormatName(a.Doctor),
        Status = EnumHelper.StatusDisplay(a.Status),
        Priority = EnumHelper.PriorityDisplay(a.Priority),
        PriorityEnum = a.Priority,
        VisitType = string.IsNullOrWhiteSpace(a.VisitType)
            ? VisitDurationService.VisitTypeOptions[0]
            : a.VisitType,
        Reason = a.Reason
    };

    private static AppointmentGridRow ToUrgentRow(Appointment a) => new()
    {
        Id = a.Id,
        Date = a.AppointmentDate.ToString("yyyy-MM-dd"),
        Time = a.AppointmentDate.ToString("HH:mm"),
        Patient = FormatName(a.Patient),
        PatientPhone = a.Patient?.PhoneNumber ?? "—",
        Doctor = FormatName(a.Doctor),
        Priority = EnumHelper.PriorityDisplay(a.Priority),
        PriorityEnum = a.Priority,
        Reason = a.Reason
    };

    private static AppointmentGridRow ToFutureRow(Appointment a) => new()
    {
        Id = a.Id,
        Date = a.AppointmentDate.ToString("yyyy-MM-dd"),
        Time = a.AppointmentDate.ToString("HH:mm"),
        Patient = FormatName(a.Patient),
        Doctor = FormatName(a.Doctor),
        Priority = EnumHelper.PriorityDisplay(a.Priority),
        PriorityEnum = a.Priority
    };

    private static string FormatName(Patient? patient) =>
        patient == null ? "—" : $"{patient.FirstName} {patient.LastName}";

    private static string FormatName(Doctor? doctor) =>
        doctor == null ? "—" : $"{doctor.FirstName} {doctor.LastName}";
}
