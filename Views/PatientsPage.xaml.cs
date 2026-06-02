using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueueManager.Views;

public partial class PatientsPage : Page
{
    private int? _editingPatientId;
    private bool _isLoadingForm;

    public PatientsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => InitializePage();
    }

    private void InitializePage()
    {
        BirthDatePicker.SelectedDate = DateTime.Today.AddYears(-30);
        LoadPatients();
        ClearForm();
    }

    private void Search_Click(object sender, RoutedEventArgs e) => LoadPatients();

    private void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Clear();
        LoadPatients();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            LoadPatients();
    }

    private void LoadPatients()
    {
        try
        {
            using var db = new AppDbContext();
            var search = SearchTextBox.Text?.Trim() ?? string.Empty;
            var query = db.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search) ||
                    p.Pesel.Contains(search) ||
                    p.PhoneNumber.Contains(search));
            }

            PatientsGrid.ItemsSource = query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd wczytywania pacjentów: {ex.Message}", "Błąd",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private Patient BuildFromForm()
    {
        return new Patient
        {
            FirstName = FirstNameBox.Text.Trim(),
            LastName = LastNameBox.Text.Trim(),
            Pesel = PeselBox.Text.Trim(),
            DateOfBirth = BirthDatePicker.SelectedDate ?? DateTime.Today.AddYears(-30),
            PhoneNumber = PhoneBox.Text.Trim(),
            Email = EmailBox.Text.Trim(),
            Address = AddressBox.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private bool ValidatePatientForm(Patient patient, out string error)
    {
        try
        {
            using var db = new AppDbContext();
            return ValidationHelper.ValidatePatient(patient, db, _editingPatientId, out error);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var patient = BuildFromForm();
        if (!ValidatePatientForm(patient, out var error))
        {
            MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            db.Patients.Add(patient);
            db.SaveChanges();
            MessageBox.Show("Dodano pacjenta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadPatients();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd dodawania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_editingPatientId == null)
        {
            MessageBox.Show("Wybierz pacjenta w tabeli po lewej, aby zapisać zmiany.", "Informacja",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var patient = BuildFromForm();
        if (!ValidatePatientForm(patient, out var error))
        {
            MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            var entity = db.Patients.FirstOrDefault(p => p.Id == _editingPatientId);
            if (entity == null)
            {
                MessageBox.Show("Nie znaleziono pacjenta w bazie.", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            entity.FirstName = patient.FirstName;
            entity.LastName = patient.LastName;
            entity.Pesel = patient.Pesel;
            entity.DateOfBirth = patient.DateOfBirth;
            entity.PhoneNumber = patient.PhoneNumber;
            entity.Email = patient.Email;
            entity.Address = patient.Address;

            db.SaveChanges();
            MessageBox.Show("Zapisano zmiany pacjenta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadPatients();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_editingPatientId == null)
        {
            MessageBox.Show("Wybierz pacjenta do usunięcia.", "Informacja",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (MessageBox.Show("Usunąć wybranego pacjenta?", "Potwierdzenie",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        try
        {
            using var db = new AppDbContext();
            if (db.Appointments.Any(a => a.PatientId == _editingPatientId))
            {
                MessageBox.Show(
                    "Nie można usunąć pacjenta — ma przypisane wizyty. Usuń wizyty najpierw.",
                    "Usuwanie niemożliwe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var entity = db.Patients.FirstOrDefault(p => p.Id == _editingPatientId);
            if (entity != null)
            {
                db.Patients.Remove(entity);
                db.SaveChanges();
            }

            MessageBox.Show("Usunięto pacjenta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadPatients();
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
        _editingPatientId = null;
        PatientsGrid.SelectedItem = null;
        FirstNameBox.Clear();
        LastNameBox.Clear();
        PeselBox.Clear();
        BirthDatePicker.SelectedDate = DateTime.Today.AddYears(-30);
        PhoneBox.Clear();
        EmailBox.Clear();
        AddressBox.Clear();
        _isLoadingForm = false;
    }

    private void PatientsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoadingForm || PatientsGrid.SelectedItem is not Patient patient)
            return;

        _editingPatientId = patient.Id;
        FirstNameBox.Text = patient.FirstName;
        LastNameBox.Text = patient.LastName;
        PeselBox.Text = patient.Pesel;
        BirthDatePicker.SelectedDate = patient.DateOfBirth;
        PhoneBox.Text = patient.PhoneNumber;
        EmailBox.Text = patient.Email;
        AddressBox.Text = patient.Address;
    }
}
