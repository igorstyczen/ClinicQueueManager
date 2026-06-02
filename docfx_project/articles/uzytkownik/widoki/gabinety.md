# Gabinety — słownik

Moduł **słownikowy** gabinetów przychodni: numer, piętro, opis.


## Funkcje

| Operacja | Opis |
|----------|------|
| Dodaj | Numer (unikalny), piętro, opis |
| Zapisz | Edycja zaznaczonego gabinetu |
| Usuń | Zablokowane, jeśli gabinet przypisany do lekarza |

---

## Pola

| Pole | Walidacja |
|------|-----------|
| Numer gabinetu | Wymagany, unikalny |
| Piętro | Liczba całkowita (wymagana) |
| Opis | Opcjonalny |

W ComboBox lekarzy wyświetlane jest np. *„Gabinet 101 (piętro 1)”*.

---

## Dane startowe

Seeder dodaje m.in. gabinety: 101, 102 (piętro 1), 201, 202 (piętro 2), 301, 302 (piętro 3).

---

## Walidacja

- *„Numer gabinetu jest wymagany.”*
- *„Gabinet o tym numerze już istnieje.”*
- *„Piętro musi być liczbą całkowitą.”*
