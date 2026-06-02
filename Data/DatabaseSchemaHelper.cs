using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Data;

/// <summary>Uzupełnia brakujące kolumny w istniejącej bazie SQLite.</summary>
public static class DatabaseSchemaHelper
{
    public static void ApplyCompatibilityUpdates(AppDbContext db)
    {
        try
        {
            db.Database.ExecuteSqlRaw(
                "ALTER TABLE Appointments ADD COLUMN DurationMinutes INTEGER NOT NULL DEFAULT 30");
        }
        catch
        {
            // Kolumna już istnieje — ignoruj.
        }

        try
        {
            db.Database.ExecuteSqlRaw(
                "ALTER TABLE Appointments ADD COLUMN VisitType TEXT NOT NULL DEFAULT 'Konsultacja ogólna'");
        }
        catch
        {
            // Kolumna już istnieje — ignoruj.
        }
    }
}
