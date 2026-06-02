using ClinicQueueManager.Data;
using System.Windows;
using System.Windows.Threading;

namespace ClinicQueueManager;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += OnDispatcherUnhandledException;

        try
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            DatabaseSchemaHelper.ApplyCompatibilityUpdates(db);
            DatabaseSeeder.Seed(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Nie udało się zainicjalizować bazy danych:\n{ex.Message}\n\nAplikacja uruchomi się bez danych startowych.",
                "Ostrzeżenie",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Wystąpił błąd:\n{e.Exception.Message}",
            "Błąd aplikacji",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}
