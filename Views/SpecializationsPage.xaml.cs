using System.Windows;
using System.Windows.Controls;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;

namespace ClinicQueueManager.Views;

public partial class SpecializationsPage : Page
{
    private int? _editingId;

    public SpecializationsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => { LoadData(); ClearForm(); };
    }

    private void LoadData()
    {
        try
        {
            using var db = new AppDbContext();
            SpecializationsGrid.ItemsSource = db.Specializations.OrderBy(s => s.Name).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateForm(out string error)
    {
        try
        {
            using var db = new AppDbContext();
            return ValidationHelper.ValidateSpecialization(NameBox.Text, db, _editingId, out error);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
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
            db.Specializations.Add(new Specialization
            {
                Name = NameBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim()
            });
            db.SaveChanges();
            MessageBox.Show("Dodano specjalizację.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_editingId == null)
        {
            MessageBox.Show("Wybierz specjalizację z tabeli.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
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
            var entity = db.Specializations.FirstOrDefault(s => s.Id == _editingId);
            if (entity == null) return;
            entity.Name = NameBox.Text.Trim();
            entity.Description = DescriptionBox.Text.Trim();
            db.SaveChanges();
            MessageBox.Show("Zapisano zmiany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_editingId == null) return;
        if (MessageBox.Show("Usunąć specjalizację?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        try
        {
            using var db = new AppDbContext();
            if (db.Doctors.Any(d => d.SpecializationId == _editingId))
            {
                MessageBox.Show(
                    "Nie można usunąć specjalizacji — jest przypisana do co najmniej jednego lekarza. Zmień specjalizację u lekarzy lub usuń ich najpierw.",
                    "Usuwanie niemożliwe",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var entity = db.Specializations.FirstOrDefault(s => s.Id == _editingId);
            if (entity != null)
            {
                db.Specializations.Remove(entity);
                db.SaveChanges();
            }

            MessageBox.Show("Usunięto specjalizację.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e) => ClearForm();

    private void ClearForm()
    {
        _editingId = null;
        NameBox.Clear();
        DescriptionBox.Clear();
        SpecializationsGrid.SelectedItem = null;
    }

    private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SpecializationsGrid.SelectedItem is not Specialization s) return;
        _editingId = s.Id;
        NameBox.Text = s.Name;
        DescriptionBox.Text = s.Description;
    }
}
