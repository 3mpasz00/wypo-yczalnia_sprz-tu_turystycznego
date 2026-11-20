# Wypożyczalnia Outdoor

Konsolowy system wypożyczalni sprzętu sportowego w C#/.NET 8. Obsługuje katalog rowerów, kajaków i namiotów, klientów, rezerwacje, wycenę, zwroty oraz zlecenia serwisowe. Reguły biznesowe znajdują się w modelach domenowych i serwisie aplikacyjnym; menu konsolowe obsługuje wejście/wyjście.

## Uruchomienie
Wymagany .NET SDK 8 lub nowszy. W terminalu w katalogu `src` wykonaj:

```powershell
dotnet restore .\WypozyczalniaSportowa.sln
dotnet build .\WypozyczalniaSportowa.sln
dotnet run --project .\WypozyczalniaSportowa\WypozyczalniaSportowa.csproj
```

Dane demonstracyjne tworzą się przy pierwszym uruchomieniu. Plik JSON i dziennik są zapisane w `Data/` obok wykonywanej aplikacji. Przykładowy klient: `Jan Kowalski` (`jan.kowalski@example.com`). Rabat wpisuj jako ułamek, np. `0,10` oznacza 10%.

## Wielopoziomowe menu
Menu główne prowadzi do sekcji Katalog, Klienci, Rezerwacje i Serwis. Każda sekcja ma własne operacje oraz powrót do menu głównego. Rezerwacje obejmują utworzenie, listę i zwrot; serwis obejmuje nowe zlecenie, zamknięcie kolejnego oraz listę kolejki.

## Architektura
`Domain` - encje, reguły, interfejsy, wyjątki, wzorce i kolekcja magazynu. `Application` - przypadki użycia rezerwacji i subskrypcja zdarzeń. `Data` - repozytorium JSON, migawki i dziennik. `UI` - menu konsolowe. Aplikacja nie wymaga zewnętrznych pakietów NuGet.

## Dokumentacja
Kompletna dokumentacja PDF z analizą, UML, mapowaniem wymagań W1-W13, scenariuszami uruchomienia i instrukcją jest w `Documentation/Projekt_Wypozyczalnia_Outdoor.pdf`. Edytowalne źródła diagramów PlantUML znajdują się w `Documentation/UML/`.

Przed oddaniem uzupełnij autora, numer albumu i grupę. W dokumencie opisano użycie AI. Temat własny wymaga akceptacji prowadzącego; należy też prowadzić repozytorium Git z historią co najmniej 10 opisowych commitów.
