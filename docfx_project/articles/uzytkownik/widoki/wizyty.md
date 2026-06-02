# Wizyty — rejestracja terminów

**Centralny moduł aplikacji** — rejestracja wizyt z triage, typem wizyty, czasem trwania i kontrolą konfliktów u lekarza.

![Formularz wizyty](~/screenshots/wizyty.png)

---

## Formularz wizyty

| Element | Opis |
|---------|------|
| Pacjent | ComboBox — wymagany |
| Lekarz | ComboBox — wymagany |
| Data | DatePicker — nie z przeszłości |
| Godzina | ComboBox (sloty co 30 min, np. 08:00–15:00) |
| Status | Zaplanowana / Zakończona / Anulowana |
| Objawy | CheckBoxy — wpływ na triage |
| Powód wizyty | Pole tekstowe — wymagane |
| Typ wizyty | ComboBox — wpływ na sugerowany czas |
| Czas trwania | 15 / 30 / 45 / 60 min |

---

## Karta triage (innowacja)

Na formularzu widoczne są:

- **Suma punktów** objawów (+2 pkt jeśli wiek > 65 lat),
- **Wyliczony priorytet** (Niski → Pilny) z kolorami,
- **Rekomendacja medyczna** (tekst pomocniczy),
- **Inteligentna rekomendacja terminu** — czy slot wolny, propozycja wcześniejszej godziny,
- **Sugestia czasu trwania** wg typu wizyty.

Przyciski **Użyj sugerowanego czasu** i **Użyj sugerowanego terminu** ustawiają pola formularza przed zapisem.

![Typ i czas](~/screenshots/innowacja.png)

---

## Tabela wizyt

Kolumny m.in.: data, godzina, czas trwania, typ wizyty, pacjent, lekarz, status, **priorytet** (kolorowany), powód, objawy.

Zaznaczenie wiersza **ładuje formularz** do edycji.

---

## Filtrowanie listy

| Filtr | Opcje |
|-------|--------|
| Status | Wszystkie / Zaplanowana / Zakończona / Anulowana |
| Priorytet | Wszystkie / Niski … Pilny |
| Lekarz | ComboBox |
| Tekst | Imię/nazwisko/PESEL pacjenta lub lekarza |

---

## Konflikty terminów

Przed zapisem `ValidationHelper` sprawdza nakładanie się przedziałów u lekarza (status Zaplanowana).

Przykład blokady: wizyta 10:30 (30 min) + nowa 10:45 (30 min).

[Szczegóły walidacji](../walidacja/wizyty.md) · [Innowacja](../../innowacja.md)

---

## Powiązane

- [Dashboard](dashboard.md)
- [Pacjenci](pacjenci.md)
- [Lekarze](lekarze.md)
