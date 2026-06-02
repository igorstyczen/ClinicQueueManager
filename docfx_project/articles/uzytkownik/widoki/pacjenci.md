# Pacjenci — kartoteka

Moduł do prowadzenia **kartoteki pacjentów** przychodni: przeglądanie, dodawanie, edycja, usuwanie i wyszukiwanie.

![Pacjenci](~/screenshots/pacjenci.png)

---

## Układ ekranu

| Obszar | Element |
|--------|---------|
| Lewa strona | **DataGrid** — lista pacjentów |
| Prawa strona | **Formularz** — dane wybranego lub nowego pacjenta |
| Góra listy | Pole **wyszukiwania** + przycisk „Wyczyść filtr” |

---

## Operacje CRUD

| Przycisk | Działanie |
|----------|-----------|
| **Dodaj** | Walidacja → zapis → komunikat sukcesu → odświeżenie tabeli → wyczyszczenie formularza |
| **Zapisz** | Aktualizacja zaznaczonego pacjenta (wymaga wyboru wiersza) |
| **Usuń** | Potwierdzenie; **zablokowane**, jeśli pacjent ma wizyty |
| **Wyczyść formularz** | Reset pól i zaznaczenia |

Kliknięcie wiersza w tabeli **wypełnia formularz** danymi pacjenta (tryb edycji).

---

## Pola formularza

| Pole | Walidacja |
|------|-----------|
| Imię, nazwisko | Wymagane |
| PESEL | 11 cyfr, cyfry, unikalny |
| Data urodzenia | Nie z przyszłości |
| Telefon | Wymagany, 7–20 znaków |
| E-mail | Wymagany, format z @ i kropką |
| Adres | Opcjonalny |

---

## Wyszukiwanie

Filtr działa na polach (operator `Contains`):

- imię,
- nazwisko,
- PESEL,
- telefon.

Enter w polu wyszukiwania uruchamia filtrowanie.

---

## Walidacja

Szczegóły: [Walidacja — pacjenci](../walidacja/pacjenci.md).

---

## Powiązane

- [Wizyty](wizyty.md) — wybór pacjenta przy rejestracji
- [Baza danych](../../baza-danych.md) — encja Patient
