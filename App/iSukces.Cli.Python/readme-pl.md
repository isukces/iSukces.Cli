# iSukces.Cli.Python

[English version](readme.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Python.svg)](https://www.nuget.org/packages/iSukces.Cli.Python/)

Biblioteka .NET (C#) ułatwiająca zarządzanie środowiskami Python z poziomu kodu. Umożliwia tworzenie wirtualnych środowisk (venv), instalowanie pakietów pip, wykrywanie i konfigurację PyTorch oraz CUDA Toolkit, a także obsługę tokenów HuggingFace i zarządzanie zmiennymi środowiskowymi procesów.

## Wymagania

- .NET 10
- Windows z launcherem Python `py`

## Zewnętrzne narzędzia i modele AI

- Python, launcher `py`, `pip`, opcjonalnie `uv` oraz tworzone środowiska `venv` są zależnościami zewnętrznymi dostarczanymi poza pakietem.
- PyTorch (`torch`, `torchvision`, `torchaudio`) i CUDA Toolkit są zewnętrznymi bibliotekami/narzędziami instalowanymi w środowisku Python lub w systemie operacyjnym.
- Tokeny i zasoby HuggingFace, prywatne indeksy pakietów oraz modele AI pobierane albo używane przez kod użytkownika nie są częścią pakietu.
- Pakiet nie zawiera modeli AI i nie gwarantuje działania modeli, bibliotek Python, sterowników GPU ani narzędzi instalowanych poza pakietem.

## Instalacja

Pakiet NuGet:

```powershell
dotnet add package iSukces.Cli.Python
```

## Użycie

Podstawowa konfiguracja procesu Python:

```csharp
var result = await new Cli
{
    FileName = "py",
    Arguments = ["--version"]
}.WithPythonCli().RunAsync();

Console.WriteLine(result.Output);
```

## Główne możliwości

### Zarządzanie wirtualnymi środowiskami (venv)
- [`PythonVenv`](PythonVenv.cs:5) — reprezentacja wirtualnego środowiska Python; tworzenie środowiska przez `py -m venv` lub `uv venv`, konfiguracja zmiennych środowiskowych (`VIRTUAL_ENV`, `PYTHONHOME`, `PATH`).
- [`PythonVenv.WithVenv()`](PythonVenv.cs:69) — metoda aplikująca ustawienia venv do uruchamianego procesu [`Cli`](../iSukces.CliTools/Cli.cs:6).
- [`PythonCliExtension.WithPythonCli()`](PythonCliExtension.cs:5) — ustawia `PYTHONIOENCODING=utf-8` dla uruchamianych procesów.

### Instalacja pakietów
- [`Installer`](Installer.cs:7) — zarządza instalacją i odinstalowywaniem pakietów pip w venv, z opcjonalnym wsparciem dla prywatnych indeksów (`--index-url`) oraz tokenów HuggingFace (`HF_TOKEN`).
- Mechanizm „touched files” (flag w katalogu `.prv-flags`) zapobiega powtarzaniu się operacji instalacji, jeśli zostały już wykonane.

### Wykrywanie i konfiguracja PyTorch / CUDA
- [`PyTorchUtils`](PyTorchUtils.cs:3) — uruchamia kod Python z biblioteką `torch` i odczytuje:
  - wersję PyTorch (`torch.__version__`),
  - dostępność CUDA (`torch.cuda.is_available()`),
  - wersję CUDA (`torch.version.cuda`).
- [`CudaToolkit`](CudaToolkit.cs:6) — wykrywa zainstalowane wersje CUDA Toolkit w systemie (na podstawie katalogu `C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA`).
- [`PythonEnvironmentInfo`](PythonEnvironmentInfo.cs:3) — agreguje informacje o środowisku PyTorch/CUDA i potrafi wygenerować [`EnvironmentUpdater`](../iSukces.CliTools/EnvironmentUpdater.cs:5) ustawiający `CUDA_PATH` oraz odpowiednie ścieżki w `PATH`.

### Diagnostyka problemów środowiska
- [`PythonEnvironmentProblemFinder`](PythonEnvironmentProblemFinder.cs:14) — analizuje środowisko i wykrywa problemy, m.in.:
  - brak lub uszkodzoną instalację PyTorch,
  - brak akceleracji GPU (CUDA),
  - niezgodność wersji CUDA Toolkit z wersją używaną przez PyTorch.
- [`PythonEnvironmentProblem`](PythonEnvironmentProblem.cs:3) — opis pojedynczego problemu wraz z sugerią rozwiązania (`Remedy`) i wskazaniem obszarów, których dotyczy ([`ProblemAffected`](PythonEnvironmentProblemFinder.cs:4)).

### Wykrywanie zainstalowanych wersji Python
- [`PyUtils.GetInstalledVersions()`](PyUtils.cs:3) — wywołuje `py -0` i parsuje listę zainstalowanych wersji Python.
- [`PyLauncherParser`](PythonVersionInfo.cs:25) — parser wyjścia Windowsowego launchera `py`, rozpoznaje wersję, architekturę (32/64-bit) oraz wersję domyślną.

### Zarządzanie zmiennymi środowiskowymi
- [`EnvironmentUpdater`](../iSukces.CliTools/EnvironmentUpdater.cs:5) — modyfikuje zmienne środowiskowe (`PATH`, `CUDA_PATH` i inne), z możliwością tymczasowej aktywacji i przywrócenia poprzedniego stanu (`Activate()` / `IDisposable`).

## Budowanie

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Konfiguracja projektu

- Target framework: `net10.0`
- Licencja pakietu: MIT

## Zależności

- [`iSukces.CliTools`](../iSukces.CliTools/readme-pl.md) — bazowa biblioteka do uruchamiania procesów CLI i zbierania argumentów.

## Platforma

- .NET 10 (`net10.0`), C# 14.
- Przeznaczony dla systemu Windows (wykorzystuje Windowsowy launcher `py` oraz ścieżki typowe dla Windows).

## Ograniczenie odpowiedzialności

Pakiet udostępnia wyłącznie warstwę .NET uruchamiającą zewnętrzne procesy i narzędzia Python w systemie operacyjnym. Działa jako pomost/fasada do narzędzi dostarczonych i instalowanych poza tym pakietem.

Pakiet nie zawiera uruchamianych narzędzi, modeli AI, bibliotek Python, sterowników GPU ani CUDA Toolkit, nie modyfikuje ich działania i nie odpowiada za ich wyniki, błędy, wymagania środowiskowe, zużycie zasobów ani szkody wynikające z użycia tych zewnętrznych narzędzi.
