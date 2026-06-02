namespace ClinicQueueManager.Helpers;

/// <summary>Powiadomienia między widokami po zmianie wizyt.</summary>
public static class AppDataEvents
{
    public static event EventHandler? AppointmentsChanged;

    public static void NotifyAppointmentsChanged() =>
        AppointmentsChanged?.Invoke(null, EventArgs.Empty);
}
