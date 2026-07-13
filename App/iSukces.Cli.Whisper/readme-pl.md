# iSukces.Cli.Whisper

[English version](readme.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Whisper.svg)](https://www.nuget.org/packages/iSukces.Cli.Whisper/)

Biblioteka .NET do uruchamiania OpenAI Whisper CLI z kodu C#. Dostarcza model konfiguracji wywołania `whisper.exe`, buduje argumenty procesu, obsługuje zadania transkrypcji i tłumaczenia, integruje uruchomienie z wirtualnym środowiskiem Python oraz zawiera zdefiniowane nazwy i metadane modeli Whisper.

## Wymagania

- .NET 10
- Zainstalowany OpenAI Whisper CLI dostępny w środowisku Python
- Wirtualne środowisko Python reprezentowane przez `PythonVenv`

## Zewnętrzne narzędzia i modele AI

- OpenAI Whisper CLI, proces `whisper.exe`, Python oraz wirtualne środowisko Python są zależnościami zewnętrznymi dostarczanymi poza tym pakietem.
- Projekt OpenAI Whisper jest dostępny pod adresem [https://github.com/openai/whisper](https://github.com/openai/whisper), a repozytorium deklaruje obecnie licencję MIT pod adresem [https://github.com/openai/whisper/blob/main/LICENSE](https://github.com/openai/whisper/blob/main/LICENSE).
- Modele Whisper, w tym `tiny`, `base`, `small`, `medium`, `large` i `turbo`, są zewnętrznymi modelami AI używanymi przez OpenAI Whisper CLI; pakiet przechowuje tylko ich nazwy i metadane konfiguracyjne.
- PyTorch, CUDA, sterowniki GPU oraz inne zależności wymagane przez Whisper CLI nie są częścią pakietu.
- Pakiet nie zawiera, nie dystrybuuje, nie instaluje, nie pobiera, nie linkuje i nie licencjonuje OpenAI Whisper, modeli Whisper, Pythona, PyTorch, CUDA ani sterowników GPU.
- Pakiet uruchamia niezależnie zainstalowany plik `whisper.exe` jako osobny proces systemowy, przekazując argumenty wiersza poleceń przez warstwę uruchamiania procesów.
- Użytkownik odpowiada za instalację OpenAI Whisper oraz za zgodność użycia wybranej wersji Whisper, modeli, pakietów Python, stosu GPU i wygenerowanych wyników z licencjami i warunkami dotyczącymi tych zewnętrznych komponentów.

## Instalacja

Pakiet NuGet:

```powershell
dotnet add package iSukces.Cli.Whisper
```

## Użycie

Minimalne uruchomienie transkrypcji:

```csharp
var result = await new WhisperCli
{
    Venv = pythonVenv,
    InputFile = "audio.mp3",
    Model = WhisperModel.Turbo,
    OutputFormat = WhisperOutputFormat.Srt
}.RunAsync(workingDirectory: null);
```

Tłumaczenie audio na język angielski:

```csharp
var result = await new WhisperCli
{
    Venv = pythonVenv,
    InputFile = "japanese.wav",
    Model = WhisperModel.Medium,
    Language = WhisperLanguages.Japanese,
    Task = WhisperTask.Translate
}.RunAsync(workingDirectory: null);
```

Nie wpisuj nazw języków bezpośrednio jako literałów tekstowych, np. `Language = "Japanese"`. Używaj stałych z klasy [`WhisperLanguages`](WhisperLanguages.cs:4), np. [`WhisperLanguages.Japanese`](WhisperLanguages.cs:294) albo [`WhisperLanguages.Ja`](WhisperLanguages.cs:84), aby zachować spójność z wartościami obsługiwanymi przez Whisper CLI.

## Główne typy

### [`WhisperCli`](WhisperCli.cs:9)

Buduje argumenty dla `whisper.exe` i uruchamia proces przez bazowy mechanizm CLI. Obsługuje plik wejściowy, model, język, katalog wyjściowy, format wyjściowy, zadanie, temperaturę, prompt początkowy, zakres czasowy oraz ustawienia powiązane z kontekstem poprzedniego tekstu.

### [`WhisperModel`](WhisperModel.cs:6)

Opisuje modele Whisper wraz z liczbą parametrów, nazwą modelu wielojęzycznego, nazwą modelu tylko dla języka angielskiego, wymaganiami VRAM i względną szybkością. Klasa udostępnia stałe modele [`WhisperModel.Tiny`](WhisperModel.cs:60), [`WhisperModel.Base`](WhisperModel.cs:70), [`WhisperModel.Small`](WhisperModel.cs:80), [`WhisperModel.Medium`](WhisperModel.cs:90), [`WhisperModel.Large`](WhisperModel.cs:100) i [`WhisperModel.Turbo`](WhisperModel.cs:110). Lista [`WhisperModel.All`](WhisperModel.cs:50) zawiera wszystkie zdefiniowane modele.

### [`WhisperLanguages`](WhisperLanguages.cs:4)

Zawiera stałe tekstowe z językami akceptowanymi przez OpenAI Whisper CLI. Klasa udostępnia zarówno krótkie kody języków, np. [`WhisperLanguages.Ja`](WhisperLanguages.cs:84), [`WhisperLanguages.Pl`](WhisperLanguages.cs:140) i [`WhisperLanguages.En`](WhisperLanguages.cs:42), jak i angielskie nazwy języków, np. [`WhisperLanguages.Japanese`](WhisperLanguages.cs:294), [`WhisperLanguages.Polish`](WhisperLanguages.cs:358) i [`WhisperLanguages.English`](WhisperLanguages.cs:252). Stałe służą do ustawiania [`WhisperCli.Language`](WhisperCli.cs:89) bez ręcznego wpisywania literałów tekstowych.

### [`WhisperTask`](WhisperCli.cs:120)

Określa tryb pracy Whisper: `Transcribe` albo `Translate`.

### [`WhisperOutputFormat`](WhisperCli.cs:126)

Określa format wyniku: `Txt`, `Vtt`, `Srt`, `Tsv`, `Json` albo `All`.

## Budowanie

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Konfiguracja projektu

- Target framework: `net10.0`
- Licencja pakietu: MIT wyłącznie dla kodu pakietu iSukces.Cli.Whisper

## Zależności

- [`iSukces.CliTools`](../iSukces.CliTools/readme-pl.md) — bazowa biblioteka do uruchamiania procesów CLI i zbierania argumentów.
- [`iSukces.Cli.Python`](../iSukces.Cli.Python/readme-pl.md) — integracja procesów z Pythonem i wirtualnymi środowiskami.
- Zewnętrznie instalowane narzędzia i biblioteki: OpenAI Whisper CLI, Python, PyTorch/CUDA według wymagań środowiska Whisper.

## Copyright i licencja OpenAI Whisper

OpenAI Whisper jest projektem zewnętrznym. Prawa autorskie do OpenAI Whisper, jego dokumentacji i powiązanych zasobów należą do odpowiednich autorów i właścicieli praw. Ten pakiet nie zawiera kodu źródłowego OpenAI Whisper, binariów, pakietów Python ani plików modeli; przechowuje jedynie wybrane nazwy konfiguracyjne i metadane potrzebne do wywołania zewnętrznie zainstalowanego procesu `whisper.exe`.

Licencja MIT deklarowana przez ten pakiet NuGet dotyczy kodu pakietu iSukces.Cli.Whisper. Nie udziela praw do OpenAI Whisper, modeli Whisper, Pythona, PyTorch, CUDA, sterowników GPU ani innych komponentów instalowanych poza tym pakietem. Użytkownik powinien sprawdzić licencję i warunki użycia każdego instalowanego komponentu zewnętrznego oraz modelu.

## Model integracji

Pakiet działa jako wrapper procesu. Nie kopiuje kodu OpenAI Whisper, nie dołącza binariów Whisper, nie dołącza plików modeli, nie instaluje pakietów Python i nie linkuje z wewnętrznymi bibliotekami OpenAI Whisper. Integracja polega na uruchomieniu osobnego programu przez system operacyjny z argumentami wiersza poleceń.

Pakiet zawiera nazwy modeli, stałe języków i pomocnicze mechanizmy budowania argumentów, aby aplikacje C# mogły konfigurować zewnętrzne wywołanie Whisper CLI bez ręcznego składania literałów wiersza poleceń. Ten opis ma charakter techniczny i nie jest poradą prawną.

## Znaki towarowe

OpenAI i Whisper są nazwami używanymi wyłącznie opisowo w celu wskazania zgodności z narzędziem OpenAI Whisper CLI. Ten pakiet nie jest tworzony, sponsorowany ani zatwierdzony przez OpenAI.

## Ograniczenie odpowiedzialności

Pakiet udostępnia wyłącznie warstwę .NET uruchamiającą zewnętrzny proces `whisper.exe` w systemie operacyjnym. Działa jako pomost/fasada do narzędzia dostarczonego i instalowanego poza tym pakietem.

Pakiet nie zawiera OpenAI Whisper CLI ani modeli Whisper, nie modyfikuje ich działania i nie odpowiada za ich licencje, wyniki, błędy, wymagania środowiskowe, zużycie zasobów, wygenerowane treści ani szkody wynikające z użycia tego zewnętrznego narzędzia, jego zależności lub modeli AI.
