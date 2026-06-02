# Walidacja — zasady ogólne

Walidacja odbywa się w klasie `ValidationHelper` przed `SaveChanges()`.

| Krok | Zachowanie |
|------|------------|
| 1 | Użytkownik klika **Dodaj** lub **Zapisz** |
| 2 | Pola tekstowe są **Trim()** |
| 3 | Wywołanie `Validate*` z kontekstem `AppDbContext` |
| 4 | Przy błędzie — `MessageBox.Show(error, "Walidacja")`, bez zapisu |
| 5 | Przy sukcesie — zapis i odświeżenie tabeli |

Zasady:

- walidacja w C#, nie w XAML,
- komunikaty po polsku,
- formularz pozostaje wypełniony po błędzie,
- przy edycji `exclude*Id` wyklucza bieżący rekord z testów unikalności.

## Moduły

| Moduł | Metoda |
|-------|--------|
| Pacjenci | `ValidatePatient` |
| Lekarze | `ValidateDoctor` |
| Wizyty | `ValidateAppointment` |
| Specjalizacje | `ValidateSpecialization` |
| Gabinety | `ValidateOffice` |
