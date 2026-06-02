using ClinicQueueManager.Data;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _db;

    public PatientRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Patient>> GetAllAsync(string? search = null)
    {
        var query = _db.Patients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search) || p.Pesel.Contains(search));
        }
        return query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync();
    }

    public Task<Patient?> GetByIdAsync(int id) => _db.Patients.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Patient patient)
    {
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        _db.Patients.Update(patient);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return;
        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
