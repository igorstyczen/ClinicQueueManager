using System.Collections.ObjectModel;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.ViewModels;

public class DoctorsViewModel : ObservableObject
{
    private readonly AppDbContext _db = new();
    private string _errorMessage = string.Empty;

    public ObservableCollection<Doctor> Doctors { get; } = new();
    public List<Specialization> Specializations { get; private set; } = new();
    public List<Office> Offices { get; private set; } = new();
    public Doctor EditingDoctor { get; set; } = new();
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public async Task LoadAsync()
    {
        Specializations = await _db.Specializations.OrderBy(x => x.Name).ToListAsync();
        Offices = await _db.Offices.OrderBy(x => x.Number).ToListAsync();
        var data = await _db.Doctors.Include(d => d.Specialization).Include(d => d.Office).ToListAsync();
        Doctors.Clear();
        foreach (var doctor in data) Doctors.Add(doctor);
    }

    public async Task AddAsync()
    {
        try
        {
            _db.Doctors.Add(EditingDoctor);
            await _db.SaveChangesAsync();
            EditingDoctor = new Doctor();
            ErrorMessage = string.Empty;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
