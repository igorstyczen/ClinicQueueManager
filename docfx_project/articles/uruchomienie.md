# Instrukcja uruchomienia

## Wymagania

| Wymaganie | Szczegóły |
|-----------|-----------|
| System | **Windows 10** lub **11** |
| SDK | **.NET SDK 8.0+** |
| IDE | VS 2022 / VS Code |
| Baza | SQLite — **nie wymaga** osobnej instalacji |

## Uruchomienie aplikacji MediQueue

```powershell
cd <katalog-projektu>
dotnet restore
dotnet build
dotnet run
```

### Pierwszy start

1. Powstaje plik **`clinic.db`**.
2. **`DatabaseSeeder`** dodaje: specjalizacje, gabinety, objawy, pacjentów, lekarzy, przykładowe wizyty.
3. Otwiera się okno **MediQueue** z menu bocznym.

### Typowe problemy

| Problem | Rozwiązanie |
|---------|-------------|
| `dotnet build` — plik zablokowany | Zamknij uruchomioną aplikację MediQueue |
| Pusta lub zła baza | Usuń `clinic.db`, uruchom ponownie |
| Brak kolumn w starej bazie | `DatabaseSchemaHelper` przy starcie dodaje brakujące pola |

## Dokument do oddania na uczelnię

Użyj **`DOKUMENTACJA.md`** + zrzuty w **`screenshots/`** → eksport do PDF z podglądu Markdown.
