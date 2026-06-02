using System.Windows;
using System.Windows.Controls;
using ClinicQueueManager.Data;
using ClinicQueueManager.Helpers;
using ClinicQueueManager.Models;

namespace ClinicQueueManager.Views;

public partial class OfficesPage : Page
{
    private int? _editingId;

    public OfficesPage()
    {
        InitializeComponent();
        Loaded += (_, _) => { LoadData(); ClearForm(); };
    }

    private void LoadData()
    {
        try
        {
            using var db = new AppDbContext();
            OfficesGrid.ItemsSource = db.Offices.OrderBy(o => o.Number).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateForm(out string error, out int floor)
    {
        try
        {
            using var db = new AppDbContext();
            return ValidationHelper.ValidateOffice(NumberBox.Text, FloorBox.Text, db, _editingId, out error, out floor);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            floor = 0;
            return false;
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm(out var error, out var floor))
        {
            MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            db.Offices.Add(new Office
            {
                Number = NumberBox.Text.Trim(),
                Floor = floor,
                Description = DescriptionBox.Text.Trim()
            });
            db.SaveChanges();
            MessageBox.Show("Dodano gabinet.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
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
            MessageBox.Show("Wybierz gabinet z tabeli.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!ValidateForm(out var error, out var floor))
        {
            MessageBox.Show(error, "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            var entity = db.Offices.FirstOrDefault(o => o.Id == _editingId);
            if (entity == null) return;
            entity.Number = NumberBox.Text.Trim();
            entity.Floor = floor;
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
        if (MessageBox.Show("Usunąć gabinet?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        try
        {
            using var db = new AppDbContext();
            if (db.Doctors.Any(d => d.OfficeId == _editingId))
            {
                MessageBox.Show(
                    "Nie można usunąć gabinetu — jest przypisany do co najmniej jednego lekarza. Zmień gabinet u lekarzy lub usuń ich najpierw.",
                    "Usuwanie niemożliwe",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var entity = db.Offices.FirstOrDefault(o => o.Id == _editingId);
            if (entity != null)
            {
                db.Offices.Remove(entity);
                db.SaveChanges();
            }

            MessageBox.Show("Usunięto gabinet.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
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
        NumberBox.Clear();
        FloorBox.Text = "0";
        DescriptionBox.Clear();
        OfficesGrid.SelectedItem = null;
    }

    private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OfficesGrid.SelectedItem is not Office o) return;
        _editingId = o.Id;
        NumberBox.Text = o.Number;
        FloorBox.Text = o.Floor.ToString();
        DescriptionBox.Text = o.Description;
    }
}
