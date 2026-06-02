# Technologie

## Stos technologiczny

| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| **C#** | 12 (z .NET 8) | Logika aplikacji, serwisy, walidacja |
| **.NET** | **8.0** (`net8.0-windows`) | Platforma uruchomieniowa |
| **WPF** | część .NET 8 | Interfejs desktopowy (okna, kontrolki) |
| **XAML** | — | Layout, style zielonego motywu MediQueue |
| **Entity Framework Core** | **9.0.0** | ORM, mapowanie encji, zapytania LINQ |
| **SQLite** | 3.x (provider EF) | Plikowa baza `clinic.db` |
| **LINQ** | — | Filtrowanie, agregacje na Dashboardzie |

## Dlaczego te technologie?

| Wybór | Uzasadnienie |
|-------|--------------|
| WPF | Natywny desktop Windows, bogate DataGrid i formularze — idealne dla recepcji |
| SQLite | Brak instalacji serwera DB, portable plik bazy, wystarczające dla małej przychodni |
| EF Core | Szybkie CRUD, relacje, migracje/schemat w kodzie |
| C# / OOP | Wymagania przedmiotu PO2 — klasy, enkapsulacja, warstwy |

## Narzędzia deweloperskie

- **Visual Studio 2022** lub **VS Code**
- **dotnet CLI** — `restore`, `build`, `run`
## Dokumentacja XML

W pliku `.csproj` włączone jest:

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

Plik `ClinicQueueManager.xml` jest źródłem sekcji **Dokumentacja API** w tym portalu.
