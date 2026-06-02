using ClinicQueueManager.Models;

namespace ClinicQueueManager.Helpers;

public static class EnumHelper
{
    public static string StatusDisplay(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Zaplanowana => "Zaplanowana",
        AppointmentStatus.Zakonczona => "Zakończona",
        AppointmentStatus.Anulowana => "Anulowana",
        _ => status.ToString()
    };

    public static string PriorityDisplay(AppointmentPriority priority) => priority switch
    {
        AppointmentPriority.Niski => "Niski",
        AppointmentPriority.Sredni => "Średni",
        AppointmentPriority.Wysoki => "Wysoki",
        AppointmentPriority.Pilny => "Pilny",
        _ => priority.ToString()
    };

    public static AppointmentStatus? ParseStatus(string? text) => text switch
    {
        "Zaplanowana" => AppointmentStatus.Zaplanowana,
        "Zakończona" => AppointmentStatus.Zakonczona,
        "Anulowana" => AppointmentStatus.Anulowana,
        _ => null
    };

    public static AppointmentPriority? ParsePriority(string? text) => text switch
    {
        "Niski" => AppointmentPriority.Niski,
        "Średni" => AppointmentPriority.Sredni,
        "Wysoki" => AppointmentPriority.Wysoki,
        "Pilny" => AppointmentPriority.Pilny,
        _ => null
    };
}
