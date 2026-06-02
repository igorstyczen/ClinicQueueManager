# Cel projektu

Celem projektu jest stworzenie **desktopowego systemu administracyjno-recepcyjnego** dla małej przychodni lekarskiej. Aplikacja **MediQueue** konsoliduje pracę recepcji w jednym oknie na Windows — zamiast kartek, Excela lub rozproszonych notatek.

## Problemy, które rozwiązuje system

| Problem w przychodni | Rozwiązanie w MediQueue |
|----------------------|-------------------------|
| Brak jednej kartoteki pacjentów | Moduł **Pacjenci** z wyszukiwaniem i walidacją PESEL |
| Trudność w planowaniu wizyt | Moduł **Wizyty** z kontrolą terminów i konfliktów |
| Brak widoku „co dziś” | **Dashboard** z harmonogramem i pilnymi przypadkami |
| Subiektywna ocena pilności | **Triage** — punkty za objawy i wiek |
| Nakładające się terminy lekarzy | Sprawdzanie **przedziałów czasowych** przed zapisem |

## Cele edukacyjne (PO2)

Projekt pokazuje praktyczne zastosowanie:

| Obszar | Realizacja |
|--------|------------|
| Programowanie obiektowe | Encje, serwisy, separacja warstw |
| Relacje bazodanowe | 1:N, N:M, klucze obce, Restrict przy usuwaniu |
| CRUD | Wszystkie moduły słownikowe i operacyjne |
| GUI (WPF + XAML) | DataGrid, ComboBox, DatePicker, style |
| Walidacja | `ValidationHelper` przed zapisem |
| ORM | Entity Framework Core + SQLite |

## Grupa docelowa

Personel **recepcji** i koordynatorzy wizyt. Aplikacja **nie implementuje logowania** — zakłada zaufane stanowisko w przychodni.

## Zakres poza projektem

- brak portalu pacjenta online,
- brak integracji z NFZ / systemami zewnętrznymi,
- brak modułu płatności i recept w pełnej obsłudze UI (struktura w modelu jest przygotowana na rozwój).
