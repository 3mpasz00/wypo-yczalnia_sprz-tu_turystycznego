using System.Globalization;
using WypozyczalniaSportowa.Application;
using WypozyczalniaSportowa.Domain;
using WypozyczalniaSportowa.Domain.Exceptions;
using WypozyczalniaSportowa.Domain.Pricing;

namespace WypozyczalniaSportowa.UI;

public sealed class ConsoleMenu(RentalApplicationService application)
{
    public void Run()
    {
        var exit = false;
        while (!exit)
        {
            Console.WriteLine("=== WYPOŻYCZALNIA OUTDOOR ===");
            Console.WriteLine("1 Katalog | 2 Klienci | 3 Rezerwacje | 4 Serwis | 0 Zapisz i zakończ");
            try
            {
                switch (ReadInt("Wybór: "))
                {
                    case 1: RunSection("KATALOG", new[] { "Pokaż i sortuj sprzęt", "Dodaj sprzęt" }, CatalogAction); break;
                    case 2: RunSection("KLIENCI", new[] { "Pokaż klientów", "Dodaj klienta" }, CustomerAction); break;
                    case 3: RunSection("REZERWACJE", new[] { "Utwórz rezerwację", "Pokaż rezerwacje", "Przyjmij zwrot" }, RentalAction); break;
                    case 4: RunSection("SERWIS", new[] { "Zleć przegląd", "Zakończ pierwszy przegląd", "Pokaż kolejkę" }, ServiceAction); break;
                    case 0: application.Save(); exit = true; break;
                    default: Console.WriteLine("Wybierz pozycję z menu."); break;
                }
            }
            catch (Exception ex) when (IsUserFacing(ex)) { Console.WriteLine(Readable(ex)); }
            finally { if (!exit) Console.WriteLine(); }
        }
    }

    private void RunSection(string title, string[] options, Action<int> action)
    {
        var back = false;
        while (!back)
        {
            Console.WriteLine($"--- {title} ---");
            for (var i = 0; i < options.Length; i++) Console.WriteLine($"{i + 1}. {options[i]}");
            Console.WriteLine("0. Wróć do menu głównego");
            try
            {
                var choice = ReadInt("Wybór: ");
                if (choice == 0) back = true;
                else if (choice > 0 && choice <= options.Length) action(choice);
                else Console.WriteLine("Wybierz pozycję z menu.");
            }
            catch (Exception ex) when (IsUserFacing(ex)) { Console.WriteLine(Readable(ex)); }
            finally { if (!back) Console.WriteLine(); }
        }
    }

    private void CatalogAction(int choice)
    {
        if (choice == 1) { foreach (var item in application.Inventory.OrderBy(x => x)) Console.WriteLine(item); return; }
        var kind = ReadText("Typ (rower/kajak/namiot): ");
        var name = ReadText("Nazwa: ");
        var rate = ReadDecimal("Cena dobowa: ");
        var label = kind.ToLowerInvariant() switch { "rower" => "Liczba przełożeń", "kajak" => "Liczba miejsc", _ => "Pojemność namiotu" };
        Console.WriteLine("Dodano: " + application.AddEquipment(kind, name, rate, ReadInt(label + ": ")));
    }

    private void CustomerAction(int choice)
    {
        if (choice == 1) { foreach (var customer in application.Customers.OrderBy(x => x.FullName)) Console.WriteLine(customer); return; }
        Console.WriteLine("Dodano: " + application.AddCustomer(ReadText("Imię i nazwisko: "), ReadText("E-mail: ")));
    }

    private void RentalAction(int choice)
    {
        if (choice == 1) { CreateRental(); return; }
        if (choice == 2) { foreach (var rental in application.Rentals.OrderByDescending(x => x.Id)) Console.WriteLine(rental); return; }
        Console.WriteLine("Zwrot: " + application.ReturnRental(ReadInt("Id rezerwacji: ")));
    }

    private void CreateRental()
    {
        foreach (var customer in application.Customers) Console.WriteLine(customer);
        foreach (var item in application.Inventory) Console.WriteLine(item);
        var customerId = ReadInt("Id klienta: ");
        var ids = ReadText("Id sprzętu (oddziel przecinkami): ").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        var start = DateOnly.ParseExact(ReadText("Od (rrrr-mm-dd): "), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var end = DateOnly.ParseExact(ReadText("Do (rrrr-mm-dd): "), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var strategy = ReadInt("Cennik: 1 standardowy, 2 weekendowy: ") == 2 ? (IPricingStrategy)new WeekendPricingStrategy() : new StandardPricingStrategy();
        var discount = ReadDecimal("Rabat (0,10 = 10%): ");
        Console.WriteLine("Utworzono: " + application.ToReceipt(application.CreateRental(customerId, ids, new RentalPeriod(start, end), strategy, discount)));
    }

    private void ServiceAction(int choice)
    {
        if (choice == 1) { Console.WriteLine("Zlecono: " + application.ScheduleService(ReadInt("Id sprzętu: "), ReadText("Opis przeglądu: "))); return; }
        if (choice == 2) { Console.WriteLine(application.CompleteNextService()?.ToString() ?? "Kolejka jest pusta."); return; }
        foreach (var order in application.ServiceHistory.Where(x => !x.IsCompleted)) Console.WriteLine(order);
    }

    private static bool IsUserFacing(Exception ex) => ex is EquipmentUnavailableException or InvalidRentalPeriodException or DomainValidationException or CustomerNotFoundException or KeyNotFoundException or StoreFormatException or IOException or UnauthorizedAccessException or FormatException or OverflowException or ArgumentException;
    private static string Readable(Exception ex) => ex switch
    {
        IOException => "Błąd pliku: " + ex.Message,
        FormatException => "Niepoprawny format. Datę wpisz rrrr-mm-dd.",
        OverflowException => "Liczba jest poza dozwolonym zakresem.",
        _ => ex.Message
    };
    private static string ReadText(string prompt) { Console.Write(prompt); return Console.ReadLine() ?? ""; }
    private static int ReadInt(string prompt) => int.Parse(ReadText(prompt), CultureInfo.InvariantCulture);
    private static decimal ReadDecimal(string prompt) => decimal.Parse(ReadText(prompt), CultureInfo.GetCultureInfo("pl-PL"));
    public override string ToString() => "Wielopoziomowe menu konsolowe wypożyczalni";
}
