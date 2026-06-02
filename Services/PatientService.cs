using System.Text.RegularExpressions;
using ClinicQueueManager.Models;
using ClinicQueueManager.Repositories;

namespace ClinicQueueManager.Services;

public class PatientService
{
    private readonly IPatientRepository _repo;

    public PatientService(IPatientRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Patient>> GetAllAsync(string? search = null) => _repo.GetAllAsync(search);

    public async Task AddAsync(Patient patient)
    {
        Validate(patient);
        try
        {
            await _repo.AddAsync(patient);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Nie udalo sie dodac pacjenta.", ex);
        }
    }

    public async Task UpdateAsync(Patient patient)
    {
        Validate(patient);
        try
        {
            await _repo.UpdateAsync(patient);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Nie udalo sie zaktualizowac pacjenta.", ex);
        }
    }

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static void Validate(Patient patient)
    {
        if (!Regex.IsMatch(patient.Pesel, @"^\d{11}$"))
            throw new ArgumentException("PESEL musi zawierac 11 cyfr.");
        if (!Regex.IsMatch(patient.PhoneNumber, @"^[\d\+\-\s]{7,20}$"))
            throw new ArgumentException("Niepoprawny numer telefonu.");
        if (!Regex.IsMatch(patient.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Niepoprawny adres email.");
    }
}
