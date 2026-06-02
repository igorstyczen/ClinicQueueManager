# Dashboard — panel dzienny recepcji

Dashboard to **pierwszy ekran** po uruchomieniu aplikacji. Skupia informacje potrzebne recepcji **na dziś**, bez przekopywania się przez całą listę wizyt.

![Dashboard](~/screenshots/dashboard.png)

---

## Kafelki statystyk (góra ekranu)

| Kafelek | Co liczy |
|---------|----------|
| **Pacjenci** | Wszyscy pacjenci w bazie (`COUNT` z tabeli Patient) |
| **Lekarze** | Wszyscy lekarze |
| **Wizyty dzisiaj** | Wizyty, gdzie `AppointmentDate.Date == DateTime.Today` |
| **Pilne dzisiaj** | Dzisiejsze wizyty z priorytetem **Pilny** |

Kafelki pozwalają w **kilka sekund** ocenić obciążenie dnia.

---

## Tabela: Dzisiejszy harmonogram

- Wszystkie wizyty z **dzisiejszą datą**.
- Sortowanie **rosnąco po godzinie**.
- Kolumny: godzina, pacjent, lekarz, status, priorytet, typ wizyty, powód.
- Gdy brak wizyt: komunikat *„Brak wizyt zaplanowanych na dzisiaj.”*

---

## Tabela: Pilne przypadki

- Wizyty z priorytetem **Pilny** lub **Wysoki**.
- Maksymalnie **5** pozycji.
- Kolejność: **najpierw dzisiejsze**, potem przyszłe.
- Kolumny m.in.: data, godzina, pacjent, **telefon pacjenta**, lekarz, priorytet, powód.

---

## Tabela: Najbliższe przyszłe wizyty

- Do **5** wizyt z datą **późniejszą niż dziś**.
- Sortowanie po dacie i godzinie.
- Szybki podgląd „co nadchodzi” bez filtrowania całej listy wizyt.

---

## Odświeżanie

Dashboard ładuje dane przy **wejściu na widok** oraz po zdarzeniu `AppDataEvents.AppointmentsChanged` (dodanie, edycja, usunięcie wizyty).

---

## Powiązane

- [Wizyty](wizyty.md) — skąd biorą się dane
- [Innowacja](../../innowacja.md) — skąd bierze się priorytet Pilny/Wysoki
