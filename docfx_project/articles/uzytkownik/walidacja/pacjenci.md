# Walidacja — moduł Pacjenci

Metoda: `ValidationHelper.ValidatePatient`.

## Komunikaty

| Warunek | Komunikat |
|---------|-----------|
| Puste imię | *Imię pacjenta jest wymagane.* |
| Puste nazwisko | *Nazwisko pacjenta jest wymagane.* |
| Pusty PESEL | *PESEL jest wymagany.* |
| PESEL ≠ 11 cyfr | *PESEL musi składać się z 11 cyfr.* |
| Duplikat PESEL | *Pacjent z takim numerem PESEL już istnieje.* |
| Data urodzenia w przyszłości | *Data urodzenia nie może być z przyszłości.* |
| Pusty telefon | *Telefon jest wymagany.* |
| Zły format telefonu | *Telefon może zawierać cyfry, spacje, myślniki i znak + (7–20 znaków).* |
| Pusty e-mail | *Email jest wymagany.* |
| Zły format e-mail | *Email musi mieć poprawny format (znak @ i kropka w domenie).* |

## Usuwanie

Usunięcie pacjenta z wizytami jest zablokowane w UI.
