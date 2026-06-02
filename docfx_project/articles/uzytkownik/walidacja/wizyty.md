# Walidacja — moduł Wizyty

Metoda: `ValidationHelper.ValidateAppointment`.

## Pola wymagane

| Warunek | Komunikat |
|---------|-----------|
| Brak pacjenta | *Wybierz pacjenta.* |
| Brak lekarza | *Wybierz lekarza.* |
| Brak daty | *Wybierz datę wizyty.* |
| Data w przeszłości | *Data wizyty nie może być z przeszłości.* |
| Brak godziny | *Wybierz godzinę wizyty.* |
| Brak statusu | *Wybierz status wizyty.* |
| Brak typu wizyty | *Wybierz typ wizyty.* |
| Brak czasu trwania | *Wybierz czas trwania wizyty.* |
| Zły czas trwania | *Czas trwania musi wynosić 15, 30, 45 lub 60 minut.* |
| Pusty powód | *Powód wizyty nie może być pusty.* |

Dozwolone minuty: **15, 30, 45, 60**.

## Kolizja u lekarza

Sprawdzana przez `AppointmentSchedulingHelper.HasScheduleOverlap` dla statusu **Zaplanowana** (nakładanie przedziałów czasowych).

> *Wybrany termin koliduje z inną wizytą lekarza. Zmień godzinę, typ wizyty albo czas trwania.*

## Duplikat u pacjenta

> *Pacjent ma już zaplanowaną wizytę w wybranym terminie. Wybierz inną godzinę lub datę.*
