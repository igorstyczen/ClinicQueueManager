using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Views;

public partial class DoctorsPage : Page
{
    private int? _editingDoctorId;
    private bool _isLoadingForm;

    public DoctorsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => InitializePage();
    }

    private void InitializePage()
    {
        LoadFilterSpecs();
        LoadFormCombos();
        LoadDoctors();
        ClearForm();
    }

    private void LoadFilterSpecs()
    {
        try
        {
            using var db = new AppDbContext();
            var list = db.Specializations.OrderBy(s => s.Name).ToList();
            list.Insert(0, new Specialization { Id = 0, Name = "Wszystkie specjalizacje", Description = "" });
            FilterSpecCombo.ItemsSource = list;
            FilterSpecCombo.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania filtrów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadFormCombos()
    {
        try
        {
            using var db = new AppDbContext();
            SpecCombo.ItemsSource = db.Specializations.OrderBy(s => s.Name).ToList();

            OfficeCombo.ItemsSource = db.Offices
                .OrderBy(o => o.Number)
                .ToList()
                .Select(o => new OfficeComboItem
                {
                    Id = o.Id,
                    Office = o,
                    Display = $"Gabinet {o.Number} (piętro {o.Floor})"
                })
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania list: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadDoctors()
    {
        try
        {
            using var db = new AppDbContext();
            var query = db.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Office)
                .AsQueryable();

            if (FilterSpecCombo.SelectedItem is Specialization spec && spec.Id > 0)
                query = query.Where(d => d.SpecializationId == spec.Id);

            var search = SearchTextBox.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.FirstName.Contains(search) ||
                    d.LastName.Contains(search) ||
                    d.Email.Contains(search) ||
                    d.PhoneNumber.Contains(search));
            }

            DoctorsGrid.ItemsSource = query
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToList()
                .Select(d => new DoctorGridRow
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    SpecializationName = d.Specialization?.Name ?? "—",
                    OfficeNumber = d.Office?.Number ?? "—",
                    PhoneNumber = d.PhoneNumber,
                    Email = d.Email
                })
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania lekarzy: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateForm(out string error)
    {
        try
        {
            var doctor = new Doctor
            {
                FirstName = FirstNameBox.Text.Trim(),
                LastName = LastNameBox.Text.Trim(),
                PhoneNumber = PhoneBox.Text.Trim(),
                Email = EmailBox.Text.Trim()
            };

            int? specId = SpecCombo.SelectedItem is Specialization spec ? spec.Id : null;
            int? officeId = OfficeCombo.SelectedItem is OfficeComboItem office ? office.Id : null;

            using var db = new AppDbContext();
            return ValidationHelper.ValidateDoctor(doctor, specId, officeId, db, _editingDoctorId, out error);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private void ApplyFormToDoctor(Doctor doctor)
    {
        var spec = (Specialization)SpecCombo.SelectedItem!;
        var office = (OfficeComboItem)OfficeCombo.SelectedItem!;
        doctor.FirstName = FirstNameBox.Text.Trim();
        doctor.LastName = LastNameBox.Text.Trim();
        doctor.PhoneNumber = PhoneBox.Text.Trim();
        doctor.Email = EmailBox.Text.Trim();
        doctor.SpecializationId = spec.Id;
        doctor.OfficeId = office.Id;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm(out var error))
        {
            MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            var doctor = new Doctor();
            ApplyFormToDoctor(doctor);
            db.Doctors.Add(doctor);
            db.SaveChanges();

            MessageBox.Show("Dodano lekarza.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDoctors();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd dodawania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_editingDoctorId == null)
        {
            MessageBox.Show("Wybierz lekarza w tabeli po lewej, aby zapisać zmiany.", "Informacja",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!ValidateForm(out var error))
        {
            MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            var doctor = db.Doctors.FirstOrDefault(d => d.Id == _editingDoctorId);
            if (doctor == null)
            {
                MessageBox.Show("Nie znaleziono lekarza w bazie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ApplyFormToDoctor(doctor);
            db.SaveChanges();

            MessageBox.Show("Zapisano zmiany lekarza.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDoctors();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_editingDoctorId == null)
        {
            MessageBox.Show("Wybierz lekarza do usunięcia.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (MessageBox.Show("Usunąć wybranego lekarza?", "Potwierdzenie",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        try
        {
            using var db = new AppDbContext();
            var hasAppointments = db.Appointments.Any(a => a.DoctorId == _editingDoctorId);
            if (hasAppointments)
            {
                MessageBox.Show(
                    "Nie można usunąć lekarza — ma przypisane wizyty. Usuń lub przenieś wizyty najpierw.",
                    "Usuwanie niemożliwe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var doctor = db.Doctors.FirstOrDefault(d => d.Id == _editingDoctorId);
            if (doctor != null)
            {
                db.Doctors.Remove(doctor);
                db.SaveChanges();
            }

            MessageBox.Show("Usunięto lekarza.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDoctors();
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
        _editingDoctorId = null;
        DoctorsGrid.SelectedItem = null;
        FirstNameBox.Clear();
        LastNameBox.Clear();
        PhoneBox.Clear();
        EmailBox.Clear();
        SpecCombo.SelectedIndex = -1;
        OfficeCombo.SelectedIndex = -1;
        _isLoadingForm = false;
    }

    private void DoctorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoadingForm || DoctorsGrid.SelectedItem is not DoctorGridRow row)
            return;

        try
        {
            _isLoadingForm = true;
            using var db = new AppDbContext();
            var doctor = db.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Office)
                .FirstOrDefault(d => d.Id == row.Id);
            if (doctor == null) return;

            _editingDoctorId = doctor.Id;
            FirstNameBox.Text = doctor.FirstName;
            LastNameBox.Text = doctor.LastName;
            PhoneBox.Text = doctor.PhoneNumber;
            EmailBox.Text = doctor.Email;

            foreach (Specialization item in SpecCombo.Items)
            {
                if (item.Id == doctor.SpecializationId)
                {
                    SpecCombo.SelectedItem = item;
                    break;
                }
            }

            foreach (OfficeComboItem item in OfficeCombo.Items)
            {
                if (item.Id == doctor.OfficeId)
                {
                    OfficeCombo.SelectedItem = item;
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania lekarza: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isLoadingForm = false;
        }
    }

    private void Search_Click(object sender, RoutedEventArgs e) => LoadDoctors();

    private void ClearFilters_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Clear();
        FilterSpecCombo.SelectedIndex = 0;
        LoadDoctors();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            LoadDoctors();
    }

    private void FilterSpec_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
            LoadDoctors();
    }
}
