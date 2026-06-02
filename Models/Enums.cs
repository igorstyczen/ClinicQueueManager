namespace ClinicQueueManager.Models;

public enum AppointmentStatus
{
    Zaplanowana = 1,
    Zakonczona = 2,
    Anulowana = 3
}

public enum AppointmentPriority
{
    Niski = 1,
    Sredni = 2,
    Wysoki = 3,
    Pilny = 4
}

public enum PaymentStatus
{
    Oplacona = 1,
    Nieoplacona = 2,
    Anulowana = 3
}

public enum PaymentMethod
{
    Gotowka = 1,
    Karta = 2,
    Przelew = 3
}
