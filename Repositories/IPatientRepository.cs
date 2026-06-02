using ClinicQueueManager.Models;

namespace ClinicQueueManager.Repositories;

public interface IPatientRepository
{
    Task<List<Patient>> GetAllAsync(string? search = null);
    Task<Patient?> GetByIdAsync(int id);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(int id);
}
