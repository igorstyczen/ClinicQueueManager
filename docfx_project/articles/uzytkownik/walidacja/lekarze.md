# Walidacja — moduł Lekarze

Metoda: `ValidationHelper.ValidateDoctor`.

## Komunikaty

| Warunek | Komunikat |
|---------|-----------|
| Puste imię | *Imię lekarza jest wymagane.* |
| Puste nazwisko | *Nazwisko lekarza jest wymagane.* |
| Pusty telefon | *Telefon jest wymagany.* |
| Zły telefon | *Telefon może zawierać cyfry, spacje, myślniki i znak + (7–20 znaków).* |
| Pusty e-mail | *Email jest wymagany.* |
| Zły e-mail | *Email musi mieć poprawny format (znak @ i kropka w domenie).* |
| Duplikat e-mail | *Lekarz z tym adresem email już istnieje.* |
| Brak specjalizacji | *Wybierz specjalizację z listy.* |
| Brak gabinetu | *Wybierz gabinet z listy.* |

E-mail musi być unikalny w bazie (przy edycji wykluczany jest bieżący rekord).
