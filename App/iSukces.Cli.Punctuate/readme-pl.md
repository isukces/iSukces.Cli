# iSukces.Cli.Punctuate

[English version](readme.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Punctuate.svg)](https://www.nuget.org/packages/iSukces.Cli.Punctuate/)

Biblioteka .NET do przywracania interpunkcji i granic zdań w tekście z poziomu C#. Pakiet uruchamia skrypt przetwarzający Python jako osobny proces systemowy, buforuje wyniki i stosuje końcowe przetwarzanie zdań po stronie .NET. Opcjonalne API pomocnicze może przygotować środowisko Python przez orkiestrację zewnętrznych narzędzi w środowisku systemu operacyjnego użytkownika.

## Wymagania

- .NET 10
- Python 3.10.x obsługiwany przez [`PythonVenv`](../iSukces.Cli.Python/PythonVenv.cs:3)
- `uv.exe` dostępny, jeśli aplikacja używająca biblioteki korzysta z opcjonalnego helpera przygotowania środowiska
- Dostęp do pakietów Python `torch`, `transformers` i `sentencepiece`
- Dostęp do modelu Hugging Face `kredor/punctuate-all`, jeśli nie jest już obecny w lokalnym cache

## Zewnętrzne komponenty i modele AI

- Python, `uv`, `pip`, tworzone środowisko `venv`, PyTorch (`torch`), Hugging Face Transformers (`transformers`), SentencePiece (`sentencepiece`) oraz model `kredor/punctuate-all` są komponentami zewnętrznymi używanymi poza kodem pakietu.
- Instalacja pakietu NuGet nie instaluje pakietów Python, nie tworzy środowiska wirtualnego, nie pobiera modeli i nie instaluje narzędzi zewnętrznych.
- Kod [`PunctuateVenvFactory`](PunctuateVenvFactory.cs:7) jest opcjonalnym helperem, który aplikacja używająca biblioteki może wywołać w celu orkiestracji narzędzi zewnętrznych w środowisku użytkownika. Po jawnym wywołaniu może uruchomić `uv`, utworzyć środowisko wirtualne i wykonać `uv pip install torch transformers sentencepiece` jako osobne procesy systemu operacyjnego.
- Skrypt [`punctuate_process.py`](Resources/punctuate_process.py:1) używa `transformers.pipeline` i modelu `kredor/punctuate-all`. W zależności od konfiguracji Hugging Face/Transformers w środowisku użytkownika ten zewnętrzny runtime może pobrać model w czasie działania, jeśli nie znajduje się on w lokalnym cache.
- Pakiet nie zawiera, nie dystrybuuje i nie licencjonuje Pythona, PyTorch, Hugging Face Transformers, SentencePiece ani modelu `kredor/punctuate-all`.
- Użytkownik odpowiada za instalację zależności zewnętrznych oraz za zgodność użycia wybranych wersji pakietów Python, modelu AI i wygenerowanych wyników z ich licencjami oraz warunkami użycia.

## Model integracji

Pakiet działa jako wrapper procesu. [`PunctExec`](PunctExec.cs:9) zapisuje tymczasowy skrypt Python oraz plik wejściowy, a następnie uruchamia `python.exe` przez warstwę [`Cli`](../iSukces.CliTools/Cli.cs:1), przekazując argumenty wiersza poleceń: nazwę skryptu, plik wejściowy i plik wyjściowy JSON. Wynik jest odczytywany z pliku JSON i przetwarzany po stronie .NET.

Taki model oznacza komunikację między oddzielnymi programami przez mechanizmy systemu operacyjnego: `exec`/uruchomienie procesu, argumenty wiersza poleceń oraz pliki wejścia/wyjścia. Analogicznie, gdy programy komunikują się przez potoki lub standardowe strumienie procesu, jest to zwykle komunikacja pomiędzy osobnymi programami, a nie jeden połączony utwór. Ten opis techniczny nie jest poradą prawną.

## Instalacja

```powershell
dotnet add package iSukces.Cli.Punctuate
```

## Użycie

```csharp
var venv = await PunctuateVenvFactory.CreateVenv(workingDirectory);
var sentences = await new PunctExec(venv).Process("to jest tekst bez interpunkcji czy da się go poprawić");
```

## Główne typy

### [`PunctuateVenvFactory`](PunctuateVenvFactory.cs:7)

Opcjonalny helper tworzący i przygotowujący środowisko Python przez uruchamianie zewnętrznych narzędzi w środowisku systemu operacyjnego użytkownika. Może wykonać polecenia instalujące zewnętrzne pakiety Python, ale te pakiety pozostają poza pakietem NuGet.

### [`PunctExec`](PunctExec.cs:9)

Uruchamia skrypt Python jako osobny proces, dzieli tekst na fragmenty, zapisuje stan i cache oraz przekazuje wynik do postprocessingu.

### [`PunctuationPostProcessor`](PunctuationPostProcessor.cs:5)

Przetwarza wynik token classification zwrócony przez skrypt Python i rekonstruuje zdania z interpunkcją.

### [`PunctuateSentencesExtractor`](PunctuateSentencesExtractor.cs:13)

Adapter usługowy do pobierania środowiska `punctuate` i zwracania zdań.

## Konfiguracja projektu

- Target framework: `net10.0`
- Licencja pakietu: MIT tylko dla kodu pakietu `iSukces.Cli.Punctuate`

## Zależności

- [`iSukces.CliTools`](../iSukces.CliTools/readme-pl.md) — bazowa biblioteka do uruchamiania procesów CLI i zbierania argumentów.
- [`iSukces.Cli.Python`](../iSukces.Cli.Python/readme-pl.md) — obsługa Pythona, `venv` i zmiennych środowiskowych procesu.
- Zewnętrzne komponenty instalowane lub używane w środowisku użytkownika: Python, `uv`, `pip`, `torch`, `transformers`, `sentencepiece`, model Hugging Face `kredor/punctuate-all`.
- Zależności NuGet przechodnie według `dotnet list package --include-transitive`: `Microsoft.SourceLink.GitHub`, `Microsoft.Build.Tasks.Git`, `Microsoft.SourceLink.Common`, `System.IO.Hashing`, `Newtonsoft.Json`, `YamlDotNet`, `iSukces.Translation`.

## Licencje i odpowiedzialność

Licencja MIT deklarowana przez ten pakiet NuGet dotyczy kodu pakietu `iSukces.Cli.Punctuate`, w tym kodu C# i dołączonego pomocniczego skryptu Python. Nie udziela praw do Pythona, PyTorch, Hugging Face Transformers, SentencePiece, modelu `kredor/punctuate-all` ani innych komponentów instalowanych lub pobieranych poza pakietem.

Nazwy Hugging Face, Transformers, PyTorch, Python i `kredor/punctuate-all` są używane opisowo w celu wskazania kompatybilności i zależności technicznych. Pakiet nie jest tworzony, sponsorowany ani zatwierdzony przez autorów tych projektów lub modeli.

Pakiet nie odpowiada za licencje, wyniki, błędy, wymagania środowiskowe, zużycie zasobów, pobrania modeli, działanie komponentów zewnętrznych ani szkody wynikające z użycia zewnętrznych bibliotek, narzędzi lub modeli AI.

Jeśli aplikacja używająca biblioteki wywoła metody pomocnicze uruchamiające `uv`, `pip`, `py`, Python albo kod Hugging Face/Transformers, te procesy działają w środowisku systemu operacyjnego użytkownika i pozostają zewnętrzne wobec pakietu NuGet. Aplikacja używająca biblioteki oraz jej użytkownik odpowiadają za wybór dokładnych wersji, akceptację ewentualnych warunków oraz weryfikację licencji zewnętrznych pakietów, narzędzi, modeli i pobieranych artefaktów.
