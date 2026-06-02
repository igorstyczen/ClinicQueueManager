# Wprowadzenie

**MediQueue** (*ClinicQueueManager*) to aplikacja **desktopowa WPF** dla recepcji małej przychodni lekarskiej. Łączy kartotekę pacjentów, grafik lekarzy, rejestrację wizyt oraz **dzienny panel** z pilnymi przypadkami.

## Główne możliwości

| Moduł | Funkcja |
|-------|---------|
| **Dashboard** | Statystyki dnia, harmonogram, pilne przypadki |
| **Pacjenci** | CRUD, wyszukiwanie, walidacja PESEL |
| **Lekarze** | CRUD, specjalizacja, gabinet, filtry |
| **Wizyty** | Rejestracja, triage, typ wizyty, konflikty terminów |
| **Specjalizacje** | Słownik dla lekarzy |
| **Gabinety** | Słownik gabinetów |

## Typowy dzień pracy recepcji

1. Otwarcie **Dashboardu** — ile wizyt dziś, co pilne.
2. Wyszukanie pacjenta lub **dodanie nowego**.
3. **Rejestracja wizyty** — lekarz, data, objawy, typ.
4. System liczy **priorytet** i sugeruje **czas trwania**.
5. Przy konflikcie terminu — **komunikat**, bez zapisu błędnych danych.
6. Po zapisie — wizyta w tabeli i na Dashboardzie.

## Dla kogo

Personel recepcji. **Brak logowania** — jeden poziom dostępu dla zaufanego stanowiska.
