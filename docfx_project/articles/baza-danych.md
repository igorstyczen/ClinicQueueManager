# Baza danych

Aplikacja używa **SQLite** — plik `clinic.db` tworzy się przy pierwszym uruchomieniu obok aplikacji.

| Właściwość | Wartość |
|------------|---------|
| Connection string | `Data Source=clinic.db` |
| Tworzenie schematu | `EnsureCreated()` |
| Dane demo | `DatabaseSeeder` — tylko gdy tabele puste |
| Aktualizacja schematu | `DatabaseSchemaHelper` (np. kolumny `VisitType`, `DurationMinutes`) |

Dane **przetrwają restart** aplikacji. Seeder **nie duplikuje** rekordów przy kolejnych uruchomieniach.

---

## Encje

### Patient (Pacjent)

| Pole | Typ / uwagi |
|------|-------------|
| Id | PK |
| FirstName, LastName | Wymagane w walidacji |
| Pesel | 11 cyfr, **unikalny** |
| DateOfBirth | Nie z przyszłości |
| PhoneNumber, Email | Wymagane, format |
| Address | Opcjonalny |
| CreatedAt | UTC |

### Doctor (Lekarz)

| Pole | Typ / uwagi |
|------|-------------|
| Id | PK |
| FirstName, LastName | |
| PhoneNumber, Email | Email **unikalny** |
| SpecializationId | FK → Specialization |
| OfficeId | FK → Office |

### Specialization / Office

Słowniki: nazwa lub numer **unikalny**, opis opcjonalny; gabinet ma **Floor** (int).

### Appointment (Wizyta)

| Pole | Typ / uwagi |
|------|-------------|
| PatientId, DoctorId | FK |
| AppointmentDate | Data + godzina |
| Status | Zaplanowana / Zakończona / Anulowana |
| Reason | Powód — wymagany |
| VisitType | Np. Kontrola |
| Priority | Niski … Pilny |
| DurationMinutes | 15, 30, 45 lub 60 |

### Symptom + AppointmentSymptom

- **Symptom** — nazwa, `PriorityPoints`
- **AppointmentSymptom** — klucz złożony (AppointmentId, SymptomId) dla relacji **N:M**

---

## Relacje

| Relacja | Kardynalność |
|---------|--------------|
| Patient → Appointment | 1:N |
| Doctor → Appointment | 1:N |
| Specialization → Doctor | 1:N |
| Office → Doctor | 1:N |
| Appointment ↔ Symptom | N:M przez AppointmentSymptom |

### Ograniczenia usuwania

- Pacjent / lekarz z wizytami — **Restrict** + komunikat w UI
- Specjalizacja / gabinet używany przez lekarza — blokada przed usunięciem

### Diagram ERD (tekst)

```
Specialization (1) ──< (N) Doctor (1) ──< (N) Appointment (N) >── (1) Patient
Office (1) ──────────< (N) Doctor
Appointment (N) ──< AppointmentSymptom >── (N) Symptom
```

![Diagram ERD](~/screenshots/erd.png)

---

## Indeksy i unikalność

- **PESEL** pacjenta — indeks unikalny w EF
- E-mail lekarza — walidacja unikalności w `ValidationHelper`
- Nazwa specjalizacji, numer gabinetu — walidacja unikalności
