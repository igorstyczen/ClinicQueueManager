using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClinicQueueManager.Models;

namespace ClinicQueueManager.Helpers;

public class PatientComboItem
{
    public int Id { get; set; }
    public string Display { get; set; } = string.Empty;
    public Patient Patient { get; set; } = null!;
}

public class DoctorComboItem
{
    public int Id { get; set; }
    public string Display { get; set; } = string.Empty;
    public Doctor Doctor { get; set; } = null!;
}

public class DoctorFilterItem
{
    public int Id { get; set; }
    public string Display { get; set; } = string.Empty;
}

public class OfficeComboItem
{
    public int Id { get; set; }
    public string Display { get; set; } = string.Empty;
    public Office Office { get; set; } = null!;
}

public class DoctorGridRow
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SpecializationName { get; set; } = string.Empty;
    public string OfficeNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class SymptomCheckItem : INotifyPropertyChanged
{
    private bool _isChecked;

    public Symptom Symptom { get; set; } = null!;
    public string Display => $"{Symptom.Name} ({Symptom.PriorityPoints} pkt)";

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value) return;
            _isChecked = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
