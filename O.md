

**Autor:** Igor Styczeń  




## 1. Jak oceniasz projekt

> Swój projekt oceniam na **około 3.0**, może **3.5** jeśli spytają o ambicję.  
> Program **działa**, ma sensowny temat, bazę SQLite, formularze WPF i podstawową logikę biznesową.  
> Nie jest idealny architektonicznie — część logiki siedzi w code-behind, nie mam pełnych migracji EF.  
> Na zaliczenie uważam, że **wystarcza**.

**Jeśli zapytają „dlaczego nie wyżej”:**

> Brakuje mi pełnego MVVM z bindingiem, Dependency Injection i migracji. To świadome uproszczenie — skupiłem się na działającym produkcie dla recepcji, a nie na wzorcu dla wzorca.

**Jeśli zapytają „dlaczego nie niżej”:**

> Bo jest pełny CRUD, relacje w bazie, walidacja, obsługa błędów try/catch, dashboard i własna logika konfliktów terminów — to więcej niż sama „forma z przyciskiem”.

---

## 2. Pytania „o zdanie” — gotowe odpowiedzi

### „Z którego elementu projektu jesteś najbardziej dumny?”

> Z modułu **wizyt** (`Views/AppointmentsPage`) — łączy formularz, triage, typ wizyty, czas trwania i sprawdzanie kolizji. To serce aplikacji recepcji.

### „Co było najtrudniejsze?”

> **Konflikty terminów** — na początku sprawdzałem tylko tę samą godzinę startu. Musiałem liczyć przedziały `[start, start + czas trwania)`. Rozwiązanie jest w `AppointmentSchedulingHelper.cs`.

### „Czego się nauczyłeś?”

> Praktycznego EF Core z SQLite, relacji 1:N i N:M, WPF (DataGrid, ComboBox, DatePicker) i tego, że walidację lepiej trzymać w jednej klasie (`ValidationHelper`) niż w każdym przycisku osobno.

### „Gdybyś miał tydzień więcej, co byś zrobił?”

> Dodał migracje EF, podpiął ViewModele do widoków i dokończył UI dla recept/płatności (modele już są w `Models/`).

### „Czyj projekt z grupy Ci się podobał najbardziej / najmniej?”

**Praca solo:**

> Projekt robiłem **indywidualnie**. Nie oceniam rankingiem projektów kolegów — każdy miał inny temat i zakres.

**Jeśli naciskają:**

> Najbardziej cenię projekty, które **działają na żywo** i mają czytelną dokumentację — u mnie też na tym mi zależało.

### „Co byś zmienił w UI?”

> Dodałbym więcej podpowiedzi przy triage i może filtr wizyt po **zakresie dat** — w kodzie jest filtr po statusie i lekarzu (`AppointmentsPage`), ale nie po dacie od–do.

### „Czy projekt jest gotowy do użycia w prawdziwej przychodni?”

> Jako **prototyp / ćwiczenie na uczelni — tak**. Produkcyjnie — nie: brak logowania, kopii zapasowych, RODO, integracji z innymi systemami. To świadomie poza zakresem PO2.

### „Dlaczego wybrałeś ten temat?”

> Przychodnia to konkretny problem: recepcja musi mieć pacjentów, lekarzy i terminy w jednym miejscu. Łatwo pokazać CRUD i relacje w bazie.

### „Co byś wyrzucił, gdyby brakowało czasu?”

> DocFX (`docfx_project/`), ViewModele które i tak nie są podpięte, encje `Prescription`/`Payment` bez UI.

---

## 3. Z czego jesteś zadowolony — 5 przykładów

Podaj **dowolne 2** — reszta jako zapas.

---

### 1) `Helpers/ValidationHelper.cs`

| | |
|---|---|
| **Metody** | `ValidatePatient`, `ValidateDoctor`, `ValidateAppointment`, `ValidateSpecialization`, `ValidateOffice` |
| **Wywołania** | `PatientsPage.xaml.cs`, `DoctorsPage.xaml.cs`, `AppointmentsPage.xaml.cs`, itd. |
| **Dlaczego** | Jedna klasa, polskie komunikaty, `out string error` → `MessageBox`, brak zapisu przy błędzie |

**Zdanie:**  
„Jestem zadowolony z `ValidationHelper`, bo cała walidacja jest w jednym miejscu — np. PESEL musi mieć 11 cyfr i być unikalny, a przy wizycie sprawdzam też kolizję u lekarza.”

---

### 2) `Services/AppointmentSchedulingHelper.cs`

| | |
|---|---|
| **Metoda** | `HasScheduleOverlap` |
| **Logika** | Tylko status `Zaplanowana`; porównanie przedziałów czasowych |
| **Przykład** | 10:30 (30 min) + 10:45 (30 min) = **kolizja**; 10:30 + 11:00 = **OK** |

**Zdanie:**  
„Z `HasScheduleOverlap` jestem zadowolony, bo to realna funkcja recepcji — lekarz nie może mieć dwóch wizyt naraz.”

---

### 3) `Services/PriorityCalculatorService.cs`

| | |
|---|---|
| **Metoda** | `CalculatePriority` |
| **Reguła** | Suma punktów objawów + 2 pkt jeśli wiek > 65 |
| **Progi** | 0–2 Niski, 3–5 Średni, 6–8 Wysoki, 9+ Pilny |
| **Widać** | Kolumna priorytetu w wizytach, dashboard „Pilne przypadki” |

**Zdanie:**  
„Triage jest prosty, ale działa — recepcja widzi od razu, które wizyty są pilne.”

---

### 4) `Views/DashboardPage.xaml.cs`

| | |
|---|---|
| **Co robi** | Liczy pacjentów, lekarzy, wizyty dziś, pilne dziś; 3 tabele |
| **Odświeżanie** | `AppDataEvents.AppointmentsChanged` po zmianie wizyt |

**Zdanie:**  
„Dashboard daje od razu obraz dnia — nie trzeba przekopywać całej listy wizyt.”

---

### 5) `Data/DatabaseSeeder.cs`

| | |
|---|---|
| **Co robi** | Wypełnia puste tabele przykładowymi danymi przy starcie |
| **Gdzie wołane** | `App.xaml.cs` → `OnStartup` |

**Zdanie:**  
„Seeder oszczędza czas — po pierwszym uruchomieniu mam od razu pacjentów i wizyty do pokazania na obronie.”

---

## 4. Co mógłbyś rozwinąć — 5 przykładów

Podaj **dowolne 2** — szczerze, bez wstydu.

---

### 1) `ViewModels/` vs `Views/*Page.xaml.cs`

- Są: `PatientsViewModel`, `DoctorsViewModel`, `AppointmentsViewModel`, `DashboardViewModel`, `ObservableObject`, `RelayCommand`.
- **Ale:** widoki tworzą `new AppDbContext()` i obsługują przyciski w code-behind.
- **Rozwój:** MVVM — logika w VM, XAML z `{Binding}`.

---

### 2) Migracje EF — `Migrations/` puste, jest tylko `Migrations/README.md`

- Teraz: `EnsureCreated()` + `DatabaseSchemaHelper.ApplyCompatibilityUpdates` (ręczne `ALTER TABLE`).
- **Rozwój:** `dotnet ef migrations add` / `database update`.

---

### 3) `Repositories/PatientRepository.cs` + `IPatientRepository`

- Repository istnieje, ale `PatientsPage` **nie używa** — bezpośrednio EF w widoku.
- **Rozwój:** warstwa dostępu do danych między UI a `DbContext`.

---

### 4) `Models/Prescription.cs`, `Models/Payment.cs`

- Są w `AppDbContext`, relacja 1:1 z wizytą.
- **Brak** modułu w menu / formularza.
- **Rozwój:** pełny CRUD recept i płatności.

---

### 5) `AppointmentsPage.xaml.cs` (~850 linii)

- Duży plik — formularz, filtry, triage, zapis objawów N:M.
- **Rozwój:** podział na mniejsze klasy / serwis wizyt / ViewModel.

---

## 5. Mapa kodu — moduł po module

### Warstwa startu

| Plik | Co robi |
|------|---------|
| `App.xaml` | Globalne style, zielony motyw |
| `App.xaml.cs` | `EnsureCreated`, seeder, globalny `try/catch`, handler wyjątków UI |
| `Views/MainWindow.xaml` | Menu boczne, przełączanie stron |
| `Views/MainWindow.xaml.cs` | Nawigacja do `DashboardPage`, `PatientsPage`, … |

### Dane

| Plik | Co robi |
|------|---------|
| `Data/AppDbContext.cs` | `DbSet<>`, connection string SQLite, `OnModelCreating` (FK, Restrict, unikalny PESEL) |
| `Data/DatabaseSeeder.cs` | Przykładowi pacjenci, lekarze, wizyty, objawy |
| `Data/DatabaseSchemaHelper.cs` | Dopisanie brakujących kolumn w starej bazie |
| `clinic.db` | Plik bazy — **nie commituj** na Git (jest w `.gitignore`) |

### Modele (`Models/`)

| Plik | Tabela / rola |
|------|----------------|
| `Patient.cs` | Pacjent |
| `Doctor.cs` | Lekarz |
| `Appointment.cs` | Wizyta |
| `Specialization.cs` | Specjalizacja |
| `Office.cs` | Gabinet |
| `Symptom.cs` | Objaw (punkty triage) |
| `AppointmentSymptom.cs` | Łączenie wizyta–objaw (N:M) |
| `Enums.cs` | Status wizyty, priorytet |
| `Prescription.cs`, `Payment.cs` | Pod przyszły rozwój |

### Logika biznesowa (`Services/` + `Helpers/`)

| Plik | Rola |
|------|------|
| `ValidationHelper.cs` | Walidacja wszystkich formularzy |
| `AppointmentSchedulingHelper.cs` | Kolizje harmonogramu |
| `PriorityCalculatorService.cs` | Triage |
| `VisitDurationService.cs` | Typ wizyty → minuty (15/30/45/60) |
| `SmartSchedulerService.cs` | Propozycja wcześniejszego terminu |
| `AppDataEvents.cs` | Event po zmianie wizyt → odśwież dashboard |
| `EnumHelper.cs` | Konwersja statusów tekst ↔ enum |
| `AppointmentGridRow.cs` | Wiersz do DataGrid (czytelne kolumny) |

### Widoki (`Views/`)

| Plik | Moduł |
|------|--------|
| `DashboardPage` | Panel dzienny |
| `PatientsPage` | Pacjenci CRUD + wyszukiwanie |
| `DoctorsPage` | Lekarze + filtry |
| `AppointmentsPage` | Wizyty + triage (największy) |
| `SpecializationsPage` | Słownik specjalizacji |
| `OfficesPage` | Słownik gabinetów |

### Dokumentacja

| Plik | Zawartość |
|------|-----------|
| `dokumentacja.md` | Dokument do oddania |
| `TESTY.md` | Opis testów ręcznych |
| `OBRONA.md` | Ten plik |
| `screenshots/` | Zrzuty do dokumentacji |

---

## 6. Scenariusz obrony krok po kroku

**Czas: ok. 5–10 minut pokazu + pytania**

### Przed wejściem

```powershell
cd <folder-z-ClinicQueueManager.csproj>
dotnet build
dotnet run
```

Zamknij inne okno MediQueue, jeśli build się wywali.

### Krok 1 — Opowiedz (30 s)

> „MediQueue to aplikacja WPF dla recepcji: pacjenci, lekarze, wizyty, dashboard. Baza SQLite, EF Core. Dodatkowo triage i blokada nakładających się terminów u lekarza.”

### Krok 2 — Dashboard (1 min)

- Pokaż kafelki i tabele.
- Powiedz: dane z `DashboardPage.xaml.cs`, odświeżanie przez `AppDataEvents`.

### Krok 3 — Pacjenci + walidacja (1 min)

- **Dodaj** pacjenta z PESEL `123` → MessageBox z `ValidationHelper`.
- Potem poprawny zapis.
- Otwórz `PatientsPage.xaml.cs` → szukaj `ValidatePatient`.

### Krok 4 — Wizyty + triage (2 min)

- Nowa wizyta: zaznacz objawy z wysokimi punktami → pokaż **priorytet**.
- Zmień **typ wizyty** → sugestia czasu (`VisitDurationService`).
- Ten sam lekarz, nachodząca godzina → **kolizja** (`AppointmentSchedulingHelper`).

### Krok 5 — Kod (1–2 min)

Otwórz w IDE (w tej kolejności, jeśli mało czasu — tylko pierwszy):

1. `ValidationHelper.cs` — fragment `ValidateAppointment`
2. `AppointmentSchedulingHelper.cs` — `HasScheduleOverlap`
3. `AppDbContext.cs` — `OnModelCreating` (relacje)

### Krok 6 — Podsumowanie

Użyj zdania z [sekcji 12](#12-zdanie-otwierające-i-zamykające).

---

## 7. Pytania techniczne PO2 — z lokalizacją w kodzie

### OOP i C#

| Pytanie | Odpowiedź | Gdzie pokazać |
|---------|-----------|---------------|
| Co to klasa / obiekt? | `Patient` to klasa; konkretny wiersz w bazie to obiekt | `Models/Patient.cs` |
| Hermetyzacja? | Pola modelu + walidacja przed zapisem | `ValidationHelper` |
| Dziedziczenie? | `Page` — widoki dziedziczą po WPF `Page` | `Views/PatientsPage.xaml.cs` |
| Interfejs? | `IPriorityCalculatorService` — implementacja `PriorityCalculatorService` | `Services/PriorityCalculatorService.cs` |
| Kolekcje? | `List<>`, LINQ `Where`, `Any`, `Include` | `AppointmentsPage`, `AppDbContext` |
| Enum? | `AppointmentStatus`, `AppointmentPriority` | `Models/Enums.cs` |

### WPF / UI

| Pytanie | Odpowiedź | Gdzie |
|---------|-----------|-------|
| XAML vs code-behind? | Wygląd w `.xaml`, zdarzenia w `.xaml.cs` | `PatientsPage.xaml` + `.cs` |
| DataGrid? | Lista pacjentów / wizyt | `PatientsPage.xaml` |
| Binding? | Kolumny `{Binding FirstName}` | XAML widoków |
| INotifyPropertyChanged? | `ObservableObject`, `SymptomCheckItem` | `Helpers/ObservableObject.cs`, `ComboItems.cs` |
| MVVM u Ciebie? | Częściowo — głównie code-behind | Szczerze: `ViewModels/` vs `Views/` |

### Baza i EF

| Pytanie | Odpowiedź | Gdzie |
|---------|-----------|-------|
| Jaka baza? | SQLite, plik `clinic.db` | `AppDbContext.OnConfiguring` |
| Code First? | Tak — klasy w `Models/`, EF tworzy tabele | `EnsureCreated` w `App.xaml.cs` |
| DbContext? | Jedna klasa łącząca się z bazą | `AppDbContext.cs` |
| SaveChanges? | Zapis zmian do pliku bazy | np. `PatientsPage` po `Add` |
| Include? | Ładowanie lekarza ze specjalizacją | `DoctorsPage`, `AppointmentsPage` |
| DeleteBehavior.Restrict? | Nie usuniesz pacjenta/lekarza z wizytami (FK) | `OnModelCreating` |
| Migracje? | **Nie mam** — używam `EnsureCreated` + `DatabaseSchemaHelper` | Szczerze |

### Obsługa błędów

| Pytanie | Odpowiedź | Gdzie |
|---------|-----------|-------|
| try/catch? | Przy zapisie, ładowaniu list | Każdy `*Page.xaml.cs` |
| MessageBox? | Walidacja, sukces, błąd | Po `Validate*` i w `catch` |
| Globalny handler? | Niezłapany wyjątek UI | `App.xaml.cs` → `DispatcherUnhandledException` |

---

## 8. Walidacja — komunikaty i gdzie testować

Plik: **`Helpers/ValidationHelper.cs`**

### Pacjent (`PatientsPage` → Dodaj)

| Test | Komunikat (skrót) |
|------|-------------------|
| Puste imię | „Imię pacjenta jest wymagane.” |
| PESEL `123` | „PESEL musi składać się z 11 cyfr.” |
| Duplikat PESEL | „Pacjent z takim numerem PESEL już istnieje.” |
| E-mail bez kropki | „Email musi mieć poprawny format…” |

### Lekarz (`DoctorsPage`)

| Test | Komunikat (skrót) |
|------|-------------------|
| Bez specjalizacji | „Wybierz specjalizację z listy.” |
| Duplikat e-mail | „Lekarz z tym adresem email już istnieje.” |

### Wizyta (`AppointmentsPage`)

| Test | Komunikat (skrót) |
|------|-------------------|
| Kolizja u lekarza | „Wybrany termin koliduje z inną wizytą lekarza…” |
| Data wczoraj | „Data wizyty nie może być z przeszłości.” |
| Brak pacjenta | „Wybierz pacjenta.” |

### Specjalizacja / gabinet

| Test | Gdzie |
|------|-------|
| Duplikat nazwy | `SpecializationsPage` |
| Duplikat numeru gabinetu | `OfficesPage` |
| Usunięcie używanego słownika | Komunikat w code-behind (nie w ValidationHelper) |

---

## 9. Baza danych — relacje do opowiedzenia

Narysuj palcem lub otwórz `screenshots/erd.png` / `AppDbContext.cs`:

```
Specialization (1) ──< (N) Doctor (1) ──< (N) Appointment (N) >── (1) Patient
Office (1) ──────────< (N) Doctor

Appointment (N) ──< AppointmentSymptom >── (N) Symptom
```

**Zdania:**

- Pacjent ma **wiele wizyt** (1:N).
- Lekarz ma **wiele wizyt** (1:N).
- Wizyta ma **wiele objawów** przez tabelę łączącą (N:M).
- PESEL pacjenta **unikalny** — indeks w `OnModelCreating`.
- Usunięcie lekarza z wizytami — **Restrict** + komunikat w UI.

---

## 10. Gdy poproszą o małą zmianę w kodzie

| Prośba | Gdzie edytować | Bezpieczna zmiana |
|--------|----------------|-------------------|
| Zmień komunikat walidacji PESEL | `ValidationHelper.cs` → `ValidatePatient` | Tekst w `error = "..."` |
| Zmień próg „Pilny” z 9 na 10 pkt | `PriorityCalculatorService.cs` → `switch` | Jedna liczba |
| Dodaj objaw w seederze | `DatabaseSeeder.cs` | Nowy `Symptom` |
| Zmień domyślny czas wizyty | `AppointmentSchedulingHelper.DefaultDurationMinutes` | Stała 30 → inna |
| Zmień kolor w UI | `App.xaml` | Zasób koloru |
| Dodaj godzinę w ComboBox | `SmartSchedulerService.ClinicTimeSlots` | Nowy string `"16:00"` |

**Jeśli nie wiesz:**  
„Pewnie w `ValidationHelper` albo w `AppointmentsPage` — mogę wyszukać w projekcie Ctrl+Shift+F.”

---

## 11. Czego nie mówić

- „Mam pełne MVVM i migracje” — **nie masz**.
- „Projekt na 5.0” — wciągnie trudne pytania.
- Negatyw o kolegach z grupy po imieniu.
- „AI napisało za mnie całość” — niepotrzebne ryzyko.
- Udawaj, że znasz DI / async w całym projekcie — async jest w ViewModelach, ale UI głównie synchroniczne.

**Zamiast tego:**  
„Działa, spełnia PO2, są rzeczy do poprawy — ViewModele i migracje.”

---

## 12. Zdanie otwierające i zamykające

**Otwarcie:**

> „To MediQueue — aplikacja desktopowa WPF dla recepcji przychodni. SQLite i Entity Framework, moduły pacjentów, lekarzy i wizyt, dashboard na dziś oraz prosta innowacja: triage i blokada konfliktów terminów u lekarza.”

**Zamknięcie:**

> „Projekt oceniam na około **3.0** — robi to, co miał robić, mam dokumentację i testy ręczne. Gdybym rozwijał dalej, zacząłbym od migracji EF i pełnego MVVM. Dziękuję za uwagę.”

---

## Szybka ściąga na kartce (4 punkty)

1. **Chwal:** `ValidationHelper` + `AppointmentSchedulingHelper`
2. **Rozwiń:** `ViewModels` (niepodpięte) + brak migracji EF
3. **Ocena:** 3.0 — działa, nie idealny
4. **Pokaż:** Dashboard → walidacja PESEL → kolizja wizyt → kod `ValidationHelper`

Powodzenia na obronie.
