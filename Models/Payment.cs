namespace ClinicQueueManager.Models;

public class Payment
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Nieoplacona;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Gotowka;

    public Appointment? Appointment { get; set; }
}
