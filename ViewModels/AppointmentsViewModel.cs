using System.Collections.ObjectModel;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using ClinicQueueManager.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.ViewModels;

public class AppointmentsViewModel : ObservableObject
{
    private readonly AppDbContext _db = new();
    private readonly IPriorityCalculatorService _priorityService = new PriorityCalculatorService();
    private string _message = string.Empty;

    public ObservableCollection<Appointment> Appointments { get; } = new();
    public List<Patient> Patients { get; private set; } = new();
    public List<Doctor> Doctors { get; private set; } = new();
    public List<Symptom> Symptoms { get; private set; } = new();

    public Appointment EditingAppointment { get; set; } = new()
    {
        AppointmentDate = DateTime.Now.AddHours(1),
        Status = AppointmentStatus.Zaplanowana
    };

    public List<int> SelectedSymptomIds { get; } = new();
    public string Message { get => _message; set => SetProperty(ref _message, value); }

    public async Task LoadAsync()
    {
        Patients = await _db.Patients.OrderBy(x => x.LastName).ToListAsync();
        Doctors = await _db.Doctors.Include(d => d.Specialization).OrderBy(x => x.LastName).ToListAsync();
        Symptoms = await _db.Symptoms.OrderByDescending(x => x.PriorityPoints).ToListAsync();

        var data = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
        Appointments.Clear();
        foreach (var item in data) Appointments.Add(item);
    }

    public void ToggleSymptom(int symptomId, bool isChecked)
    {
        if (isChecked && !SelectedSymptomIds.Contains(symptomId))
            SelectedSymptomIds.Add(symptomId);
        else if (!isChecked)
            SelectedSymptomIds.Remove(symptomId);
    }

    public async Task AddAsync()
    {
        try
        {
            var patient = await _db.Patients.FirstAsync(p => p.Id == EditingAppointment.PatientId);
            var symptoms = await _db.Symptoms.Where(s => SelectedSymptomIds.Contains(s.Id)).ToListAsync();
            var calculated = _priorityService.CalculatePriority(patient, symptoms);

            EditingAppointment.Priority = calculated;
            _db.Appointments.Add(EditingAppointment);
            await _db.SaveChangesAsync();

            foreach (var symptomId in SelectedSymptomIds)
            {
                _db.AppointmentSymptoms.Add(new AppointmentSymptom
                {
                    AppointmentId = EditingAppointment.Id,
                    SymptomId = symptomId
                });
            }
            await _db.SaveChangesAsync();

            Message = $"Dodano wizyte. Priorytet: {calculated}.";
            EditingAppointment = new Appointment { AppointmentDate = DateTime.Now.AddHours(1), Status = AppointmentStatus.Zaplanowana };
            SelectedSymptomIds.Clear();
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Message = $"Blad: {ex.Message}";
        }
    }
}
