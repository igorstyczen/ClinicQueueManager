# Innowacja — inteligentny moduł wizyt

Najważniejszy element wyróżniający MediQueue to **wsparcie decyzji recepcji** przy rejestracji wizyty: **triage (priorytet)** oraz **inteligentny czas trwania** z kontrolą **konfliktów harmonogramu**.

> System **nie zastępuje lekarza** — pomaga uporządkować kolejkę i uniknąć błędów terminowych.

![Innowacja — triage i czas wizyty](~/screenshots/innowacja.png)

---

## 1. Triage — automatyczny priorytet wizyty

### Zasada działania

1. Recepcjonistka zaznacza **objawy** z listy (każdy objaw ma punkty w tabeli `Symptom`).
2. System sumuje punkty objawów.
3. Jeśli pacjent ma **więcej niż 65 lat**, dodaje **+2 punkty**.
4. Suma jest mapowana na priorytet wizyty.
5. Wynik jest **zapisywany w bazie** i widoczny w tabeli oraz na Dashboardzie.

### Tabela progów priorytetu

| Suma punktów | Priorytet | Interpretacja dla recepcji |
|--------------|-----------|----------------------------|
| 0–2 | **Niski** | Wizyta rutynowa |
| 3–5 | **Średni** | Wymaga uwagi, bez pośpiechu |
| 6–8 | **Wysoki** | Rozważyć szybszy termin |
| 9+ | **Pilny** | Najwyższa pilność — sekcja na Dashboardzie |

### Przykładowe objawy (dane startowe)

| Objaw | Punkty |
|-------|--------|
| Ból gardła | 1 |
| Kaszel | 1 |
| Gorączka | 2 |
| Silny ból głowy | 3 |
| Duszność | 5 |
| Ból w klatce piersiowej | 6 |
| Utrata przytomności | 8 |

**Przykład:** pacjent 70 lat z „Bólem w klatce” (6) + „Dusznością” (5) + bonus wieku (2) = **13 pkt → Pilny**.

### Implementacja w kodzie

| Klasa | Rola |
|-------|------|
| `PriorityCalculatorService` | Oblicza priorytet z objawów i wieku |
| `AppointmentSymptom` | Łączy wizytę z wieloma objawami (N:M) |

### Prezentacja w UI

- Karta **triage** na stronie wizyt: suma punktów, wyliczony priorytet, rekomendacja tekstowa.
- Kolory w tabeli: m.in. **czerwony** (Pilny), **pomarańczowy** (Wysoki).

---

## 2. Inteligentny czas wizyty

**Typ wizyty** (nie priorytet) określa **sugerowany czas trwania** slotu w grafiku.

| Typ wizyty | Czas |
|------------|------|
| Kontrola | 15 min |
| Wypisanie recepty | 15 min |
| Szczepienie | 15 min |
| Konsultacja ogólna | 30 min |
| Badanie | 30 min |
| Pierwsza wizyta | 45 min |
| Pilna konsultacja | 45 min |
| Zabieg | 60 min |

### Zachowanie w aplikacji

- Zmiana typu wizyty **aktualizuje sugestię** czasu na formularzu.
- Przycisk **„Użyj sugerowanego czasu”** ustawia ComboBox (15/30/45/60 min) — **nie zapisuje** wizyty automatycznie.
- Przy priorytecie **Wysoki/Pilny** i typie rutynowym (np. Kontrola) — **ostrzeżenie** o rozważeniu „Pilnej konsultacji”.

**Klasa:** `VisitDurationService`.

---

## 3. Asystent terminu i konflikty

### SmartSchedulerService

- Sprawdza, czy wybrany **slot jest wolny** u lekarza.
- Dla wyższych priorytetów może **zaproponować wcześniejszą godzinę** tego samego dnia.
- Przycisk **„Użyj sugerowanego terminu”** kopiuje tylko datę i godzinę do formularza.

### Blokada konfliktów (AppointmentSchedulingHelper)

Sprawdzane są **przedziały** `[godzina startu, start + czas trwania)`, nie tylko identyczna godzina.

| Istniejąca wizyta | Nowa wizyta | Wynik |
|-------------------|-------------|--------|
| 10:30, 30 min | 10:45, 30 min | **BLOKADA** (nakładanie) |
| 10:30, 30 min | 11:00, 30 min | **OK** |

Dotyczy statusu **Zaplanowana**, tego samego lekarza, tego samego dnia.

Walidacja: `ValidationHelper.ValidateAppointment` → komunikat *„Wybrany termin koliduje z inną wizytą lekarza…”*.

---

## Podsumowanie wartości dla recepcji

1. **Szybsza ocena pilności** bez zgadywania.
2. **Spójny czas wizyty** wg typu usługi.
3. **Mniej podwójnych rezerwacji** u jednego lekarza.
4. **Dashboard** pokazuje pilne sprawy od razu po wejściu do aplikacji.
