# Architektura aplikacji

Aplikacja jest podzielona na **warstwy** — od interfejsu użytkownika po bazę SQLite. Logika biznesowa nie jest mieszana z kodem XAML tam, gdzie to możliwe.

## Struktura folderów

```
ClinicQueueManager/
├── Models/           # Encje: Patient, Doctor, Appointment, Symptom, …
├── Data/             # AppDbContext, DatabaseSeeder, DatabaseSchemaHelper
├── Views/            # Strony WPF (.xaml + code-behind)
├── Services/         # Triage, harmonogram, czas wizyty, smart scheduler
├── Helpers/          # ValidationHelper, EnumHelper, AppDataEvents
├── Repositories/     # Dostęp do danych (opcjonalnie)
├── ViewModels/       # Przygotowanie pod MVVM
├── App.xaml          # Style globalne, start aplikacji
└── clinic.db         # Baza SQLite (po pierwszym uruchomieniu)
```

## Diagram warstw

```
┌─────────────────────────────────────────┐
│  Prezentacja (Views — WPF)              │
│  DataGrid, formularze, MessageBox       │
├─────────────────────────────────────────┤
│  Logika biznesowa (Services)            │
│  PriorityCalculator, Scheduling, …      │
├─────────────────────────────────────────┤
│  Pomocnicze (Helpers)                   │
│  ValidationHelper, AppDataEvents        │
├─────────────────────────────────────────┤
│  Dostęp do danych (EF Core)             │
│  AppDbContext, LINQ                       │
├─────────────────────────────────────────┤
│  Modele (Models)                        │
│  Encje i enumeracje                     │
└─────────────────────────────────────────┘
                    │
                    ▼
              SQLite (clinic.db)
```

## Kluczowe klasy serwisów

| Serwis | Odpowiedzialność |
|--------|------------------|
| `PriorityCalculatorService` | Triage — punkty → priorytet |
| `VisitDurationService` | Typ wizyty → minuty |
| `AppointmentSchedulingHelper` | Konflikty przedziałów czasowych |
| `SmartSchedulerService` | Rekomendacja terminu |
| `ValidationHelper` | Walidacja wszystkich formularzy |

## Przepływ zapisu wizyty (uproszczony)

1. Użytkownik wypełnia formularz na `AppointmentsPage`.
2. `ValidationHelper.ValidateAppointment` — jeśli błąd → MessageBox, **stop**.
3. `PriorityCalculatorService` — wyliczenie priorytetu z objawów.
4. `SaveChanges()` — zapis do SQLite.
5. `AppDataEvents.NotifyAppointmentsChanged()` — odświeżenie Dashboardu.

## Wzorzec architektoniczny

Projekt używa **code-behind** w widokach WPF z wydzielonymi **serwisami** — uproszczona forma warstwowości, odpowiednia dla projektu zaliczeniowego. Wybrane klasy w `ViewModels/` przygotowują rozszerzenie o pełne **MVVM**.

## Start aplikacji

W `App.xaml.cs`:

1. `Database.EnsureCreated()`
2. `DatabaseSchemaHelper.ApplyCompatibilityUpdates()` — kolumny w starszych bazach
3. `DatabaseSeeder.Seed()` — dane demo tylko gdy tabele puste
