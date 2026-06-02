# MediQueue — dokumentacja projektu

**System zarządzania przychodnią lekarską**  
Programowanie Obiektowe 2 · C# · WPF · .NET 8 · EF Core · SQLite

---

## O projekcie

**MediQueue** (*ClinicQueueManager*) to aplikacja **desktopowa** dla recepcji małej przychodni. Zastępuje rozproszone zapisy jednym narzędziem: pacjenci, lekarze, wizyty, dashboard dzienny oraz **inteligentny moduł triage** i **sugerowanie czasu wizyty**.

---

## Szybka nawigacja

| Temat | Opis |
|-------|------|
| [Wprowadzenie](articles/wprowadzenie.md) | Zakres i sposób korzystania z aplikacji |
| [Cel projektu](articles/cel-projektu.md) | Dlaczego powstał system |
| [Dashboard](articles/uzytkownik/widoki/dashboard.md) | Panel dzienny recepcji |
| [Pacjenci](articles/uzytkownik/widoki/pacjenci.md) | Kartoteka pacjentów |
| [Lekarze](articles/uzytkownik/widoki/lekarze.md) | Kadra, ComboBox specjalizacji i gabinetu |
| [Specjalizacje](articles/uzytkownik/widoki/specjalizacje.md) | Słownik specjalizacji |
| [Gabinety](articles/uzytkownik/widoki/gabinety.md) | Słownik gabinetów |
| [Wizyty](articles/uzytkownik/widoki/wizyty.md) | Rejestracja terminów |
| [Cel projektu](articles/cel-projektu.md) | Uzasadnienie i zakres PO2 |
| [Innowacja](articles/innowacja.md) | Triage + czas wizyty + konflikty |
| [Baza danych](articles/baza-danych.md) | Encje i relacje |
| [Walidacja](articles/uzytkownik/walidacja/index.md) | Reguły i komunikaty |
| [Uruchomienie](articles/uruchomienie.md) | `dotnet build` / `dotnet run` |
| [Dokumentacja API](api/ClinicQueueManager.html) | Komentarze XML w kodzie |

---

## Uruchomienie aplikacji

```powershell
cd <katalog-projektu>
dotnet restore
dotnet build
dotnet run
```

## Uruchomienie tej dokumentacji (HTML)

```powershell
docfx docfx_project/docfx.json --serve
```

Adres: **http://localhost:8080**
