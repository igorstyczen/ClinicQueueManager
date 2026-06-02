namespace ClinicQueueManager.Models;

/// <summary>Wynik rekomendacji terminu wizyty.</summary>
public class AppointmentRecommendation
{
    public bool IsSelectedTermAcceptable { get; set; }
    public DateTime? SuggestedDate { get; set; }
    public string? SuggestedTime { get; set; }
    public bool HasSuggestedTerm { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}
