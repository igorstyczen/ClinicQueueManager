using System.Collections.ObjectModel;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using ClinicQueueManager.Repositories;
using ClinicQueueManager.Services;

namespace ClinicQueueManager.ViewModels;

public class PatientsViewModel : ObservableObject
{
    private readonly PatientService _service;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;

    public ObservableCollection<Patient> Patients { get; } = new();
    public Patient EditingPatient { get; set; } = new() { DateOfBirth = DateTime.Today.AddYears(-20) };

    public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public PatientsViewModel()
    {
        var db = new AppDbContext();
        _service = new PatientService(new PatientRepository(db));
    }

    public async Task LoadAsync()
    {
        Patients.Clear();
        var data = await _service.GetAllAsync(SearchText);
        foreach (var patient in data)
        {
            Patients.Add(patient);
        }
    }

    public async Task AddPatientAsync()
    {
        try
        {
            await _service.AddAsync(EditingPatient);
            EditingPatient = new Patient { DateOfBirth = DateTime.Today.AddYears(-20) };
            ErrorMessage = string.Empty;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
