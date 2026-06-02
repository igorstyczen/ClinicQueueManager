using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using ClinicQueueManager.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Views;

public partial class AppointmentsPage : Page
{
    private readonly IPriorityCalculatorService _priorityCalculator = new PriorityCalculatorService();
    private readonly ISmartSchedulerService _smartScheduler = new SmartSchedulerService();
    private readonly IVisitDurationService _visitDurationService = new VisitDurationService();
    private List<SymptomCheckItem> _symptomItems = new();
    private AppointmentRecommendation? _lastRecommendation;
    private int? _lastSuggestedDuration;
    private int? _editingAppointmentId;
    private bool _isLoadingForm;

    private static readonly string[] TimeSlots = SmartSchedulerService.ClinicTimeSlots;
    private static readonly string[] DurationOptions = { "15 minut", "30 minut", "45 minut", "60 minut" };

    public AppointmentsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => InitializePage();
    }

    private void InitializePage()
    {
        FilterStatusCombo.ItemsSource = new[] { "Wszystkie", "Zaplanowana", "Zakończona", "Anulowana" };
        FilterStatusCombo.SelectedIndex = 0;
        FilterPriorityCombo.ItemsSource = new[] { "Wszystkie", "Niski", "Średni", "Wysoki", "Pilny" };
        FilterPriorityCombo.SelectedIndex = 0;

        StatusCombo.ItemsSource = new[] { "Zaplanowana", "Zakończona", "Anulowana" };
        StatusCombo.SelectedIndex = 0;
        TimeCombo.ItemsSource = TimeSlots;
        TimeCombo.SelectedIndex = 2;
        AppointmentDatePicker.SelectedDate = DateTime.Today;
        DurationCombo.ItemsSource = DurationOptions;
        DurationCombo.SelectedIndex = 1;
        VisitTypeCombo.ItemsSource = VisitDurationService.VisitTypeOptions;
        VisitTypeCombo.SelectedIndex = 0;

        LoadLookups();
        LoadFilterDoctors();
        LoadAppointments();
        ClearForm();
    }

    private void LoadFilterDoctors()
    {
        try
        {
            using var db = new AppDbContext();
            var items = new List<DoctorFilterItem>
            {
                new() { Id = 0, Display = "Wszyscy lekarze" }
            };
            items.AddRange(db.Doctors
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .Select(d => new DoctorFilterItem
                {
                    Id = d.Id,
                    Display = $"{d.FirstName} {d.LastName}"
                }));
            FilterDoctorCombo.ItemsSource = items;
            FilterDoctorCombo.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania filtra lekarzy: {ex.Message}", "Błąd",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadLookups()
    {
        try
        {
            using var db = new AppDbContext();

            var patients = db.Patients.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();
            PatientCombo.ItemsSource = patients
                .Select(p => new PatientComboItem
                {
                    Id = p.Id,
                    Patient = p,
                    Display = $"{p.FirstName} {p.LastName} - {p.Pesel}"
                })
                .ToList();

            var doctors = db.Doctors
                .Include(d => d.Specialization)
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToList();
            DoctorCombo.ItemsSource = doctors
                .Select(d => new DoctorComboItem
                {
                    Id = d.Id,
                    Doctor = d,
                    Display = $"{d.FirstName} {d.LastName} - {d.Specialization?.Name ?? "brak specjalizacji"}"
                })
                .ToList();

            _symptomItems = db.Symptoms
                .OrderBy(s => s.PriorityPoints)
                .ThenBy(s => s.Name)
                .Select(s => new SymptomCheckItem { Symptom = s })
                .ToList();
            AttachSymptomHandlers();
            BindSymptomsList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AttachSymptomHandlers()
    {
        foreach (var item in _symptomItems)
        {
            item.PropertyChanged -= SymptomItem_PropertyChanged;
            item.PropertyChanged += SymptomItem_PropertyChanged;
        }
    }

    private void SymptomItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SymptomCheckItem.IsChecked) && !_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void BindSymptomsList()
    {
        SymptomsList.ItemsSource = null;
        SymptomsList.ItemsSource = _symptomItems;
    }

    private void ScheduleFormAssistantUpdate()
    {
        Dispatcher.BeginInvoke(UpdateFormAssistant, DispatcherPriority.DataBind);
    }

    private void LoadAppointments()
    {
        try
        {
            using var db = new AppDbContext();
            var query = db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d!.Specialization)
                .Include(a => a.AppointmentSymptoms)
                .ThenInclude(asym => asym.Symptom)
                .AsQueryable();

            if (FilterStatusCombo.SelectedItem is string statusFilter && statusFilter != "Wszystkie")
            {
                var status = EnumHelper.ParseStatus(statusFilter);
                if (status.HasValue)
                    query = query.Where(a => a.Status == status.Value);
            }

            if (FilterPriorityCombo.SelectedItem is string priorityFilter && priorityFilter != "Wszystkie")
            {
                var priority = EnumHelper.ParsePriority(priorityFilter);
                if (priority.HasValue)
                    query = query.Where(a => a.Priority == priority.Value);
            }

            if (FilterDoctorCombo.SelectedItem is DoctorFilterItem doctorFilter && doctorFilter.Id > 0)
                query = query.Where(a => a.DoctorId == doctorFilter.Id);

            var search = FilterSearchBox.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Patient!.FirstName.Contains(search) ||
                    a.Patient.LastName.Contains(search) ||
                    a.Patient.Pesel.Contains(search) ||
                    a.Doctor!.FirstName.Contains(search) ||
                    a.Doctor.LastName.Contains(search));
            }

            AppointmentsGrid.ItemsSource = query
                .OrderByDescending(a => a.AppointmentDate)
                .ToList()
                .Select(a => new AppointmentGridRow
                {
                    Id = a.Id,
                    Date = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    Time = a.AppointmentDate.ToString("HH:mm"),
                    Duration = $"{(a.DurationMinutes > 0 ? a.DurationMinutes : AppointmentSchedulingHelper.DefaultDurationMinutes)} min",
                    VisitType = string.IsNullOrWhiteSpace(a.VisitType) ? "Konsultacja ogólna" : a.VisitType,
                    Patient = $"{a.Patient?.FirstName} {a.Patient?.LastName}",
                    Doctor = $"{a.Doctor?.FirstName} {a.Doctor?.LastName}",
                    Status = EnumHelper.StatusDisplay(a.Status),
                    Priority = EnumHelper.PriorityDisplay(a.Priority),
                    PriorityEnum = a.Priority,
                    Reason = a.Reason,
                    Symptoms = string.Join(", ", a.AppointmentSymptoms
                        .Where(x => x.Symptom != null)
                        .Select(x => x.Symptom!.Name))
                })
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania wizyt: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateFormAssistant()
    {
        if (_isLoadingForm || PointsText == null || PriorityText == null)
            return;

        var selectedSymptoms = _symptomItems.Where(s => s.IsChecked).Select(s => s.Symptom).ToList();
        var symptomPoints = selectedSymptoms.Sum(s => s.PriorityPoints);
        var patient = (PatientCombo.SelectedItem as PatientComboItem)?.Patient;
        var doctorItem = DoctorCombo.SelectedItem as DoctorComboItem;

        UpdateSymptomsExplanation(selectedSymptoms);

        AppointmentPriority? calculatedPriority = null;

        if (patient == null)
        {
            PointsText.Text = symptomPoints > 0
                ? $"Suma punktów: {symptomPoints} (wybierz pacjenta, aby doliczyć ewentualne +2 pkt za wiek > 65 lat)"
                : "Suma punktów: 0";

            if (selectedSymptoms.Count == 0)
            {
                PriorityText.Text = "Wyliczony priorytet: —";
                PriorityText.Foreground = System.Windows.Media.Brushes.Gray;
                PriorityText.FontWeight = FontWeights.Normal;
                MedicalRecommendationText.Text = "Rekomendacja medyczna: wybierz pacjenta i objawy.";
            }
            else
            {
                var previewPatient = new Patient { DateOfBirth = DateTime.Today.AddYears(-30) };
                calculatedPriority = _priorityCalculator.CalculatePriority(previewPatient, selectedSymptoms);
                ApplyPriorityText(calculatedPriority.Value);
                MedicalRecommendationText.Text =
                    $"Rekomendacja medyczna: {BuildMedicalRecommendation(calculatedPriority.Value)} (bez bonusu za wiek)";
            }

            UpdateSchedulerRecommendation(null, null);
            UpdateDurationRecommendation(null);
            return;
        }

        var age = GetAge(patient.DateOfBirth);
        var totalPoints = symptomPoints + (age > 65 ? 2 : 0);
        calculatedPriority = _priorityCalculator.CalculatePriority(patient, selectedSymptoms);

        PointsText.Text = age > 65
            ? $"Suma punktów: {totalPoints} (objawy: {symptomPoints} + 2 za wiek > 65 lat)"
            : $"Suma punktów: {totalPoints}";

        ApplyPriorityText(calculatedPriority.Value);
        MedicalRecommendationText.Text =
            $"Rekomendacja medyczna: {BuildMedicalRecommendation(calculatedPriority.Value)}";

        UpdateDurationRecommendation(calculatedPriority);

        if (doctorItem == null || AppointmentDatePicker.SelectedDate == null || TimeCombo.SelectedItem == null)
        {
            UpdateSchedulerRecommendation(calculatedPriority, doctorItem?.Id);
            return;
        }

        UpdateSchedulerRecommendation(calculatedPriority, doctorItem.Id);
    }

    private void UpdateDurationRecommendation(AppointmentPriority? priority)
    {
        if (SuggestedDurationText == null || DurationExplanationText == null || SelectedVisitTypeText == null)
            return;

        var visitType = GetSelectedVisitType();
        SelectedVisitTypeText.Text = string.IsNullOrWhiteSpace(visitType)
            ? "Wybrany typ wizyty: —"
            : $"Wybrany typ wizyty: {visitType}";

        if (string.IsNullOrWhiteSpace(visitType))
        {
            _lastSuggestedDuration = null;
            SuggestedDurationText.Text = "Sugerowany czas wizyty: —";
            DurationExplanationText.Text = "Uzasadnienie: wybierz typ wizyty.";
            UseSuggestedDurationButton.IsEnabled = false;
            UpdatePriorityWarningDisplay(priority, visitType);
            return;
        }

        var suggested = _visitDurationService.GetSuggestedDuration(visitType);
        _lastSuggestedDuration = suggested;

        SuggestedDurationText.Text = $"Sugerowany czas wizyty: {suggested} minut";
        DurationExplanationText.Text = "Uzasadnienie: " +
            _visitDurationService.GetDurationExplanation(visitType);
        UseSuggestedDurationButton.IsEnabled = true;
        UpdatePriorityWarningDisplay(priority, visitType);
    }

    private void UpdatePriorityWarningDisplay(AppointmentPriority? priority, string? visitType)
    {
        if (PriorityWarningText == null)
            return;

        if (priority == null || string.IsNullOrWhiteSpace(visitType))
        {
            PriorityWarningText.Visibility = Visibility.Collapsed;
            PriorityWarningText.Text = string.Empty;
            return;
        }

        var warning = _visitDurationService.GetPriorityWarning(priority.Value, visitType);
        if (string.IsNullOrWhiteSpace(warning))
        {
            PriorityWarningText.Visibility = Visibility.Collapsed;
            PriorityWarningText.Text = string.Empty;
            return;
        }

        PriorityWarningText.Text = warning;
        PriorityWarningText.Visibility = Visibility.Visible;
    }

    private string? GetSelectedVisitType() =>
        VisitTypeCombo.SelectedItem as string;

    private void SelectVisitType(string? visitType)
    {
        if (string.IsNullOrWhiteSpace(visitType))
        {
            VisitTypeCombo.SelectedIndex = -1;
            return;
        }

        if (VisitDurationService.VisitTypeOptions.Contains(visitType))
            VisitTypeCombo.SelectedItem = visitType;
    }

    private void UpdateSymptomsExplanation(List<Symptom> selectedSymptoms)
    {
        if (SymptomsExplanationText == null)
            return;

        SymptomsExplanationText.Text = selectedSymptoms.Count == 0
            ? "Uzasadnienie objawów: brak zaznaczonych objawów."
            : "Uzasadnienie objawów: " + string.Join(", ",
                selectedSymptoms.Select(s => $"{s.Name} ({s.PriorityPoints} pkt)"));
    }

    private static string BuildMedicalRecommendation(AppointmentPriority priority) => priority switch
    {
        AppointmentPriority.Niski =>
            "wizyta rutynowa — standardowy termin jest wystarczający.",
        AppointmentPriority.Sredni =>
            "kontrola objawów — termin w najbliższym rozsądnym oknie czasowym.",
        AppointmentPriority.Wysoki =>
            "podwyższona pilność — rozważ wcześniejszy termin u wybranego lekarza.",
        AppointmentPriority.Pilny =>
            "pilny przypadek — priorytet najwcześniejszego wolnego terminu tego dnia.",
        _ => "ocena wymaga uzupełnienia danych."
    };

    private void UpdateSchedulerRecommendation(AppointmentPriority? priority, int? doctorId)
    {
        if (SelectedTermText == null || TermEvaluationText == null)
            return;

        var date = AppointmentDatePicker.SelectedDate;
        var timeText = TimeCombo.SelectedItem as string;

        if (doctorId == null || date == null || timeText == null || priority == null)
        {
            _lastRecommendation = null;
            SelectedTermText.Text = "Wybrany termin: uzupełnij lekarza, datę i godzinę.";
            TermEvaluationText.Text = "Ocena terminu: —";
            SuggestedTermText.Text = "Sugerowany termin: brak";
            SchedulerExplanationText.Text = "Uzasadnienie: —";
            UseSuggestedTermButton.IsEnabled = false;
            return;
        }

        var selectedDateTime = BuildAppointmentDateTime();
        SelectedTermText.Text = $"Wybrany termin: {selectedDateTime:yyyy-MM-dd}, godz. {timeText}";

        try
        {
            using var db = new AppDbContext();
            _lastRecommendation = _smartScheduler.Evaluate(
                db,
                doctorId.Value,
                selectedDateTime,
                priority.Value,
                _editingAppointmentId);

            TermEvaluationText.Text = "Ocena terminu: " + _lastRecommendation.Message;
            TermEvaluationText.Foreground = priority.Value is AppointmentPriority.Pilny or AppointmentPriority.Wysoki
                ? System.Windows.Media.Brushes.DarkOrange
                : System.Windows.Media.Brushes.DarkGreen;

            if (_lastRecommendation.HasSuggestedTerm &&
                _lastRecommendation.SuggestedDate.HasValue &&
                !string.IsNullOrEmpty(_lastRecommendation.SuggestedTime))
            {
                SuggestedTermText.Text =
                    $"Sugerowany termin: {_lastRecommendation.SuggestedDate:yyyy-MM-dd}, godz. {_lastRecommendation.SuggestedTime}";
            }
            else
            {
                SuggestedTermText.Text = "Sugerowany termin: brak";
            }

            SchedulerExplanationText.Text = string.IsNullOrWhiteSpace(_lastRecommendation.Explanation)
                ? "Uzasadnienie: —"
                : "Uzasadnienie: " + _lastRecommendation.Explanation;

            UseSuggestedTermButton.IsEnabled = _lastRecommendation.HasSuggestedTerm;
        }
        catch (Exception ex)
        {
            _lastRecommendation = null;
            TermEvaluationText.Text = "Ocena terminu: błąd oceny.";
            SuggestedTermText.Text = "Sugerowany termin: brak";
            SchedulerExplanationText.Text = "Uzasadnienie: " + ex.Message;
            UseSuggestedTermButton.IsEnabled = false;
        }
    }

    private void ApplyPriorityText(AppointmentPriority priority)
    {
        PriorityText.Text = $"Wyliczony priorytet: {EnumHelper.PriorityDisplay(priority)}";
        PriorityText.Foreground = priority == AppointmentPriority.Pilny
            ? System.Windows.Media.Brushes.Red
            : System.Windows.Media.Brushes.Black;
        PriorityText.FontWeight = priority == AppointmentPriority.Pilny
            ? FontWeights.Bold
            : FontWeights.Normal;
    }

    private static int GetAge(DateTime birthDate)
    {
        var age = DateTime.Today.Year - birthDate.Year;
        if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;
        return age;
    }

    private DateTime BuildAppointmentDateTime()
    {
        var date = AppointmentDatePicker.SelectedDate ?? DateTime.Today;
        var timeText = TimeCombo.SelectedItem as string ?? "09:00";
        var parts = timeText.Split(':');
        return new DateTime(date.Year, date.Month, date.Day, int.Parse(parts[0]), int.Parse(parts[1]), 0);
    }

    private bool ValidateForm(AppDbContext db, out string error)
    {
        var patientId = (PatientCombo.SelectedItem as PatientComboItem)?.Id;
        var doctorId = (DoctorCombo.SelectedItem as DoctorComboItem)?.Id;
        var statusText = StatusCombo.SelectedItem as string;
        var hasTime = TimeCombo.SelectedItem != null;
        DateTime? appointmentDateTime = AppointmentDatePicker.SelectedDate != null && hasTime
            ? BuildAppointmentDateTime()
            : null;

        return ValidationHelper.ValidateAppointment(
            patientId,
            doctorId,
            AppointmentDatePicker.SelectedDate,
            appointmentDateTime,
            hasTime,
            statusText,
            GetSelectedVisitType(),
            DurationCombo.SelectedItem != null,
            GetSelectedDurationMinutes(),
            ReasonBox.Text,
            db,
            _editingAppointmentId,
            out error);
    }

    private int GetSelectedDurationMinutes()
    {
        var selected = DurationCombo.SelectedItem as string;
        if (string.IsNullOrEmpty(selected))
            return AppointmentSchedulingHelper.DefaultDurationMinutes;

        var parts = selected.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return int.TryParse(parts[0], out var minutes)
            ? minutes
            : AppointmentSchedulingHelper.DefaultDurationMinutes;
    }

    private void SelectDurationMinutes(int minutes)
    {
        var label = $"{minutes} minut";
        if (DurationOptions.Contains(label))
            DurationCombo.SelectedItem = label;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var db = new AppDbContext();
            if (!ValidateForm(db, out var error))
            {
                MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patientItem = (PatientComboItem)PatientCombo.SelectedItem!;
            var doctorItem = (DoctorComboItem)DoctorCombo.SelectedItem!;
            var symptoms = _symptomItems.Where(s => s.IsChecked).Select(s => s.Symptom).ToList();
            var priority = _priorityCalculator.CalculatePriority(patientItem.Patient, symptoms);
            var status = EnumHelper.ParseStatus(StatusCombo.SelectedItem as string) ?? AppointmentStatus.Zaplanowana;
            var appointmentDateTime = BuildAppointmentDateTime();

            var appointment = new Appointment
            {
                PatientId = patientItem.Id,
                DoctorId = doctorItem.Id,
                AppointmentDate = appointmentDateTime,
                Status = status,
                Reason = ReasonBox.Text.Trim(),
                VisitType = GetSelectedVisitType() ?? VisitDurationService.VisitTypeOptions[0],
                Priority = priority,
                DurationMinutes = GetSelectedDurationMinutes(),
                CreatedAt = DateTime.UtcNow
            };
            db.Appointments.Add(appointment);
            db.SaveChanges();

            foreach (var symptom in symptoms)
            {
                db.AppointmentSymptoms.Add(new AppointmentSymptom
                {
                    AppointmentId = appointment.Id,
                    SymptomId = symptom.Id
                });
            }
            db.SaveChanges();

            MessageBox.Show(
                $"Dodano wizytę.\nPriorytet: {EnumHelper.PriorityDisplay(priority)}.",
                "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            AppDataEvents.NotifyAppointmentsChanged();
            LoadAppointments();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd dodawania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_editingAppointmentId == null)
        {
            MessageBox.Show("Wybierz wizytę w tabeli po lewej, aby zapisać zmiany.", "Informacja",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            if (!ValidateForm(db, out var error))
            {
                MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patientItem = (PatientComboItem)PatientCombo.SelectedItem!;
            var doctorItem = (DoctorComboItem)DoctorCombo.SelectedItem!;
            var symptoms = _symptomItems.Where(s => s.IsChecked).Select(s => s.Symptom).ToList();
            var priority = _priorityCalculator.CalculatePriority(patientItem.Patient, symptoms);
            var status = EnumHelper.ParseStatus(StatusCombo.SelectedItem as string) ?? AppointmentStatus.Zaplanowana;
            var appointmentDateTime = BuildAppointmentDateTime();

            var appointment = db.Appointments
                .Include(a => a.AppointmentSymptoms)
                .FirstOrDefault(a => a.Id == _editingAppointmentId);
            if (appointment == null) return;

            appointment.PatientId = patientItem.Id;
            appointment.DoctorId = doctorItem.Id;
            appointment.AppointmentDate = appointmentDateTime;
            appointment.Status = status;
            appointment.Reason = ReasonBox.Text.Trim();
            appointment.VisitType = GetSelectedVisitType() ?? VisitDurationService.VisitTypeOptions[0];
            appointment.Priority = priority;
            appointment.DurationMinutes = GetSelectedDurationMinutes();

            db.AppointmentSymptoms.RemoveRange(appointment.AppointmentSymptoms);
            foreach (var symptom in symptoms)
            {
                db.AppointmentSymptoms.Add(new AppointmentSymptom
                {
                    AppointmentId = appointment.Id,
                    SymptomId = symptom.Id
                });
            }

            db.SaveChanges();
            MessageBox.Show(
                $"Zapisano zmiany.\nPriorytet: {EnumHelper.PriorityDisplay(priority)}.",
                "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            AppDataEvents.NotifyAppointmentsChanged();
            LoadAppointments();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_editingAppointmentId == null)
        {
            MessageBox.Show("Wybierz wizytę do usunięcia.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (MessageBox.Show("Usunąć wybraną wizytę?", "Potwierdzenie",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        try
        {
            using var db = new AppDbContext();
            var appointment = db.Appointments.FirstOrDefault(a => a.Id == _editingAppointmentId);
            if (appointment != null)
            {
                db.AppointmentSymptoms.RemoveRange(
                    db.AppointmentSymptoms.Where(x => x.AppointmentId == appointment.Id));
                db.Appointments.Remove(appointment);
                db.SaveChanges();
            }

            MessageBox.Show("Usunięto wizytę.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            AppDataEvents.NotifyAppointmentsChanged();
            LoadAppointments();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd usuwania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e) => ClearForm();

    private void ClearForm()
    {
        _isLoadingForm = true;
        _editingAppointmentId = null;
        AppointmentsGrid.SelectedItem = null;
        PatientCombo.SelectedIndex = -1;
        DoctorCombo.SelectedIndex = -1;
        AppointmentDatePicker.SelectedDate = DateTime.Today;
        TimeCombo.SelectedIndex = 2;
        StatusCombo.SelectedIndex = 0;
        DurationCombo.SelectedIndex = 1;
        VisitTypeCombo.SelectedIndex = 0;
        ReasonBox.Clear();
        foreach (var item in _symptomItems)
            item.IsChecked = false;
        _isLoadingForm = false;
        UpdateFormAssistant();
    }

    private void AppointmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AppointmentsGrid.SelectedItem is not AppointmentGridRow row)
            return;

        try
        {
            _isLoadingForm = true;
            using var db = new AppDbContext();
            var appointment = db.Appointments
                .Include(a => a.AppointmentSymptoms)
                .FirstOrDefault(a => a.Id == row.Id);
            if (appointment == null) return;

            _editingAppointmentId = appointment.Id;

            SelectComboItem(PatientCombo, appointment.PatientId);
            SelectComboItem(DoctorCombo, appointment.DoctorId);

            AppointmentDatePicker.SelectedDate = appointment.AppointmentDate.Date;
            var timeStr = appointment.AppointmentDate.ToString("HH:mm");
            TimeCombo.SelectedItem = TimeSlots.Contains(timeStr) ? timeStr : TimeSlots[0];
            StatusCombo.SelectedItem = EnumHelper.StatusDisplay(appointment.Status);
            ReasonBox.Text = appointment.Reason;
            SelectVisitType(string.IsNullOrWhiteSpace(appointment.VisitType)
                ? VisitDurationService.VisitTypeOptions[0]
                : appointment.VisitType);
            SelectDurationMinutes(appointment.DurationMinutes > 0
                ? appointment.DurationMinutes
                : AppointmentSchedulingHelper.DefaultDurationMinutes);

            var symptomIds = appointment.AppointmentSymptoms.Select(x => x.SymptomId).ToHashSet();
            foreach (var item in _symptomItems)
                item.IsChecked = symptomIds.Contains(item.Symptom.Id);

            _isLoadingForm = false;
            UpdateFormAssistant();
        }
        catch (Exception ex)
        {
            _isLoadingForm = false;
            MessageBox.Show($"Błąd wczytywania wizyty: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void SelectComboItem(ComboBox combo, int id)
    {
        foreach (var item in combo.Items)
        {
            var itemId = item switch
            {
                PatientComboItem p => p.Id,
                DoctorComboItem d => d.Id,
                _ => -1
            };
            if (itemId == id)
            {
                combo.SelectedItem = item;
                return;
            }
        }
    }

    private void PatientCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void DoctorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void AppointmentSchedule_Changed(object sender, RoutedEventArgs e)
    {
        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void ReasonBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void VisitTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void DurationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void UseSuggestedDuration_Click(object sender, RoutedEventArgs e)
    {
        if (_lastSuggestedDuration is not int minutes)
            return;

        _isLoadingForm = true;
        SelectDurationMinutes(minutes);
        _isLoadingForm = false;
        ScheduleFormAssistantUpdate();
    }

    private void UseSuggestedTerm_Click(object sender, RoutedEventArgs e)
    {
        if (_lastRecommendation is not { HasSuggestedTerm: true } recommendation ||
            !recommendation.SuggestedDate.HasValue ||
            string.IsNullOrEmpty(recommendation.SuggestedTime))
        {
            return;
        }

        _isLoadingForm = true;
        AppointmentDatePicker.SelectedDate = recommendation.SuggestedDate.Value.Date;
        TimeCombo.SelectedItem = TimeSlots.Contains(recommendation.SuggestedTime)
            ? recommendation.SuggestedTime
            : TimeSlots[0];
        _isLoadingForm = false;
        ScheduleFormAssistantUpdate();
    }

    private void Symptom_CheckedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox { DataContext: SymptomCheckItem item } checkBox)
        {
            var isChecked = checkBox.IsChecked == true;
            if (item.IsChecked != isChecked)
                item.IsChecked = isChecked;
        }

        if (!_isLoadingForm)
            ScheduleFormAssistantUpdate();
    }

    private void Filter_Click(object sender, RoutedEventArgs e) => LoadAppointments();

    private void ClearFilters_Click(object sender, RoutedEventArgs e)
    {
        FilterStatusCombo.SelectedIndex = 0;
        FilterPriorityCombo.SelectedIndex = 0;
        FilterDoctorCombo.SelectedIndex = 0;
        FilterSearchBox.Clear();
        LoadAppointments();
    }

    private void FilterDoctor_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
            LoadAppointments();
    }

    private void FilterSearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            LoadAppointments();
    }
}
