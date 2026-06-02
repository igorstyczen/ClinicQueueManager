using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.ViewModels;

public class DashboardViewModel : ObservableObject
{
    private int _patientsCount;
    private int _doctorsCount;
    private int _plannedAppointmentsCount;
    private int _urgentAppointmentsCount;
    private List<Appointment> _recentAppointments = new();

    public int PatientsCount { get => _patientsCount; set => SetProperty(ref _patientsCount, value); }
    public int DoctorsCount { get => _doctorsCount; set => SetProperty(ref _doctorsCount, value); }
    public int PlannedAppointmentsCount { get => _plannedAppointmentsCount; set => SetProperty(ref _plannedAppointmentsCount, value); }
    public int UrgentAppointmentsCount { get => _urgentAppointmentsCount; set => SetProperty(ref _urgentAppointmentsCount, value); }
    public List<Appointment> RecentAppointments { get => _recentAppointments; set => SetProperty(ref _recentAppointments, value); }

    public async Task LoadAsync()
    {
        using var db = new AppDbContext();
        PatientsCount = await db.Patients.CountAsync();
        DoctorsCount = await db.Doctors.CountAsync();
        PlannedAppointmentsCount = await db.Appointments.CountAsync(a => a.Status == AppointmentStatus.Zaplanowana);
        UrgentAppointmentsCount = await db.Appointments.CountAsync(a => a.Priority == AppointmentPriority.Pilny);
        RecentAppointments = await db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .Take(5)
            .ToListAsync();
    }
}
