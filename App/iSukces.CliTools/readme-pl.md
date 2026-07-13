# iSukces.CliTools

[English version](readme.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.CliTools.svg)](https://www.nuget.org/packages/iSukces.CliTools/)

Biblioteka pomocnicza do uruchamiania zewnętrznych narzędzi CLI z poziomu aplikacji .NET. Dostarcza klasy do asynchronicznego startowania procesów, budowania argumentów, obsługi zmiennych środowiskowych, przechwytywania stdout/stderr oraz zwracania wyniku w spójnym modelu.

## Wymagania

- .NET 10

## Instalacja

Pakiet NuGet:

```powershell
dotnet add package iSukces.CliTools
```

## Użycie

Minimalne uruchomienie procesu:

```csharp
var result = await new Cli
{
    FileName = "dotnet",
    Arguments = ["--info"]
}.RunAsync();

Console.WriteLine(result.Output);
```

Budowanie argumentów:

```csharp
var arguments = new ArgumentCollector();
arguments.Add("input.mp4");
arguments.Add("output.mp3", "output");
arguments.Add(true, "overwrite", "no-overwrite");
```

## Główne typy

### [`Cli`](Cli.cs:6)

Klasa do asynchronicznego uruchamiania zewnętrznych procesów. Obsługuje plik wykonywalny, argumenty, katalog roboczy, zmienne środowiskowe, przekierowanie stdout/stderr oraz tryby prezentacji wyjścia: `All`, `Tail` i `None`.

### [`ArgumentCollector`](ArgumentCollector.cs:5)

Pomaga budować tablicę argumentów CLI. Obsługuje argumenty zwykłe, opcje z prefiksem, przełączniki boolean, wartości liczbowe i tekstowe oraz konwersję wartości do notacji snake_case przez [`AddSnake`](ArgumentCollector.cs:52).

### [`CliResult`](CliResult.cs:3) i [`CliException`](CliException.cs:3)

[`CliResult`](CliResult.cs:3) przechowuje kod wyjścia, stdout, stderr, czas startu, czas zakończenia, czas trwania oraz pełne dane wywołania. [`CreateException`](CliResult.cs:5) tworzy [`CliException`](CliException.cs:3) z opisem błędu procesu.

### [`Result<T>`](Result.cs:46) i [`Result`](Result.cs:3)

Model wyniku operacji zawierający wartość, status powodzenia, wyjątek i opcjonalny [`CliResult`](CliResult.cs:3). Metody [`Make`](Result.cs:5), [`MakeAsync`](Result.cs:10) i [`FromCliCallEx`](Result.cs:15) ułatwiają opakowanie wywołań synchronicznych, asynchronicznych i CLI.

### [`EnvironmentUpdater`](EnvironmentUpdater.cs:5)

Pozwala przygotować zmienne środowiskowe dla procesu, dodać katalogi na początek `PATH`, aktywować zmiany tymczasowo oraz wyszukać plik wykonywalny w skonfigurowanych ścieżkach.

### [`TailConsoleOutput`](TailConsoleOutput.cs:8)

Implementuje tryb wyświetlania ostatnich linii wyjścia procesu w konsoli. Używany przez [`Cli`](Cli.cs:6), gdy wybrano tryb `Tail`.

### [`WindowsConsoleUtils`](WindowsConsoleUtils.cs:5)

Zawiera [`EscapeCommandLineParameterIfNecessary`](WindowsConsoleUtils.cs:7), czyli escapowanie argumentów wiersza poleceń zgodne z regułami systemu Windows.

### [`TextExtensions`](TextExtensions.cs:3)

Zawiera rozszerzenia tekstowe, między innymi [`ToSnake`](TextExtensions.cs:5) i [`Coalesce`](TextExtensions.cs:35).

## Budowanie

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Konfiguracja projektu

- Target framework: `net10.0`
- Licencja pakietu: MIT

## Ograniczenie odpowiedzialności

Pakiet udostępnia wyłącznie warstwę .NET uruchamiającą wskazany zewnętrzny proces w systemie operacyjnym. Działa jako pomost/fasada do narzędzia dostarczonego i instalowanego poza tym pakietem.

Pakiet nie zawiera uruchamianego narzędzia, nie modyfikuje jego działania i nie odpowiada za jego wyniki, błędy, wymagania środowiskowe, zużycie zasobów ani szkody wynikające z użycia tego zewnętrznego narzędzia.
