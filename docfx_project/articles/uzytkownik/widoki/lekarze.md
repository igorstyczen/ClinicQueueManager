# Lekarze

Moduł zarządzania **kadra lekarską**: dane kontaktowe, przypisanie **specjalizacji** i **gabinetu**, filtrowanie i wyszukiwanie.

![Lekarze](~/screenshots/lekarze.png)

---

## Lista lekarzy (DataGrid)

Kolumny pokazują **czytelne nazwy**, nie identyfikatory z bazy:

| Kolumna | Źródło |
|---------|--------|
| Imię, nazwisko | Doctor |
| Specjalizacja | `Specialization.Name` |
| Gabinet | `Office.Number` |
| Telefon, e-mail | Doctor |

---

## Formularz

| Pole | Uwagi |
|------|--------|
| Imię, nazwisko | Wymagane |
| Telefon, e-mail | Wymagane; e-mail unikalny |
| Specjalizacja | **ComboBox** — wymagany wybór |
| Gabinet | **ComboBox** — wymagany wybór |

Listy ComboBox ładują się ze słowników **Specjalizacje** i **Gabinety**.

---

## Filtrowanie i wyszukiwanie

| Narzędzie | Działanie |
|-----------|-----------|
| ComboBox specjalizacji | „Wszystkie specjalizacje” lub jedna wybrana |
| Pole tekstowe | Imię, nazwisko, e-mail, telefon |
| Wyczyść filtr | Pełna lista |

---

## Usuwanie

Nie można usunąć lekarza z **przypisanymi wizytami** — komunikat z instrukcją usunięcia lub przeniesienia wizyt.

---

## Walidacja

[Szczegóły walidacji lekarzy](../walidacja/lekarze.md).

---

## Powiązane

- [Specjalizacje](specjalizacje.md)
- [Gabinety](gabinety.md)
- [Wizyty](wizyty.md)
