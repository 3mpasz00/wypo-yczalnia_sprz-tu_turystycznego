using System.Globalization;
using WypozyczalniaSportowa.Application;
using WypozyczalniaSportowa.Data;
using WypozyczalniaSportowa.Domain.Exceptions;
using WypozyczalniaSportowa.UI;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "wypozyczalnia.json");
var repository = new JsonStoreRepository(dataPath);
using var audit = new AuditLogWriter(Path.Combine(AppContext.BaseDirectory, "Data", "zdarzenia.log"));
var application = new RentalApplicationService(repository);
using var notifications = new NotificationService(application, audit);
try
{
    application.Initialize();
    new ConsoleMenu(application).Run();
}
catch (StoreFormatException ex)
{
    Console.Error.WriteLine($"Nie można odczytać danych wypożyczalni: {ex.Message}");
    Environment.ExitCode = 1;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"Problem z plikiem danych: {ex.Message}");
    Environment.ExitCode = 1;
}
catch (UnauthorizedAccessException ex)
{
    Console.Error.WriteLine($"Brak uprawnień do pliku danych: {ex.Message}");
    Environment.ExitCode = 1;
}
