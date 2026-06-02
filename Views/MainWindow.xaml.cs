using System.Windows;
using System.Windows.Controls;

namespace ClinicQueueManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        try
        {
            SetActiveNav(NavDashboard);
            ContentFrame.Navigate(new DashboardPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Nie udało się załadować widoku startowego:\n{ex.Message}",
                "Błąd",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void SetActiveNav(Button activeButton)
    {
        var navButtonStyle = TryFindResource("NavButton") as Style;
        var navButtonActiveStyle = TryFindResource("NavButtonActive") as Style;

        if (navButtonStyle == null || navButtonActiveStyle == null)
            return;

        foreach (var button in new[] { NavDashboard, NavPatients, NavDoctors, NavAppointments, NavSpecializations, NavOffices })
        {
            button.Style = navButtonStyle;
        }

        activeButton.Style = navButtonActiveStyle;
    }

    private void Dashboard_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavDashboard);
        ContentFrame.Navigate(new DashboardPage());
    }

    private void Patients_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavPatients);
        ContentFrame.Navigate(new PatientsPage());
    }

    private void Doctors_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavDoctors);
        ContentFrame.Navigate(new DoctorsPage());
    }

    private void Appointments_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavAppointments);
        ContentFrame.Navigate(new AppointmentsPage());
    }

    private void Specializations_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavSpecializations);
        ContentFrame.Navigate(new SpecializationsPage());
    }

    private void Offices_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNav(NavOffices);
        ContentFrame.Navigate(new OfficesPage());
    }
}
