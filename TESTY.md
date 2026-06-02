# Raport testów — MediQueue (ClinicQueueManager)

**Autor:** Igor Styczeń  
**Środowisko:** Windows, .NET 8, WPF, EF Core 9, SQLite

## Uruchomienie

```powershell
dotnet restore
dotnet build
dotnet run
```

**Wynik:** aplikacja uruchamia się, tworzy `clinic.db`, ładuje dane startowe (seeder).

## Testy ręczne

| Obszar | Test | Wynik |
|--------|------|--------|
| Start | `dotnet build`, `dotnet run`, menu boczne | OK |
| Dashboard | Kafelki i tabele na dziś | OK |
| Pacjenci | Dodanie, edycja, wyszukiwanie | OK |
| Pacjenci | Walidacja — pusty PESEL / duplikat PESEL | OK (MessageBox) |
| Lekarze | Dodanie ze specjalizacją i gabinetem | OK |
| Lekarze | Usunięcie lekarza z wizytami | Zablokowane — OK |
| Specjalizacje / Gabinety | CRUD słowników | OK |
| Wizyty | Dodanie wizyty z triage i czasem trwania | OK |
| Wizyty | Kolizja terminów u lekarza | Zablokowane — OK |
| Wizyty | Filtrowanie listy | OK |
| Dashboard | Odświeżenie po zmianie wizyty | OK |

## Uwagi

Przed `dotnet build` zamknij uruchomioną aplikację (inaczej plik `.exe` może być zablokowany).
