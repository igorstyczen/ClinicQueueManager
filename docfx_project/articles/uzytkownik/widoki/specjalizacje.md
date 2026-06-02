# Specjalizacje — słownik

Moduł **słownikowy** — nazwy specjalizacji lekarskich używane przy definiowaniu lekarzy.


## Funkcje

| Operacja | Opis |
|----------|------|
| Dodaj | Nazwa (unikalna) + opcjonalny opis |
| Zapisz | Edycja zaznaczonej pozycji |
| Usuń | Zablokowane, jeśli specjalizacja jest przypisana do lekarza |
| Lista | DataGrid posortowany po nazwie |

---

## Pola

| Pole | Walidacja |
|------|-----------|
| Nazwa | Wymagana, unikalna w bazie |
| Opis | Opcjonalny |

---

## Powiązanie z lekarzami

Po dodaniu specjalizacji pojawia się ona w **ComboBox** na stronie [Lekarze](lekarze.md).

Dane startowe (seeder): Internista, Kardiolog, Pulmonolog, Pediatra, Neurolog, Dermatolog.

---

## Walidacja

Komunikat przy duplikacie: *„Specjalizacja o tej nazwie już istnieje.”*
