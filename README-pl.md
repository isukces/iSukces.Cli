# iSukces.Cli

[English version](README.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.CliTools.svg)](https://www.nuget.org/packages/iSukces.CliTools/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Python.svg)](https://www.nuget.org/packages/iSukces.Cli.Python/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Ffmpeg.svg)](https://www.nuget.org/packages/iSukces.Cli.Ffmpeg/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Whisper.svg)](https://www.nuget.org/packages/iSukces.Cli.Whisper/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Punctuate.svg)](https://www.nuget.org/packages/iSukces.Cli.Punctuate/)

`iSukces.Cli` to solucja .NET 10 zawierająca biblioteki do uruchamiania zewnętrznych narzędzi wiersza poleceń z kodu C#. Solucja obejmuje bazową warstwę uruchamiania procesów CLI oraz pakiety dla środowisk Python, FFmpeg, OpenAI Whisper CLI i przywracania interpunkcji przez zewnętrzny proces Python.

## Projekty

- `iSukces.CliTools` — bazowa biblioteka do asynchronicznego uruchamiania procesów CLI, budowania argumentów, obsługi środowiska i wyników procesu.
- `iSukces.Cli.Python` — zarządzanie wirtualnymi środowiskami Python, instalacja pakietów pip, wykrywanie PyTorch i CUDA Toolkit, obsługa tokenów HuggingFace i konfiguracja środowiska procesu.
- `iSukces.Cli.Ffmpeg` — wrapper dla zewnętrznie zainstalowanego procesu `ffmpeg`.
- `iSukces.Cli.Whisper` — wrapper dla OpenAI Whisper CLI uruchamianego ze środowiska wirtualnego Python.
- `iSukces.Cli.Punctuate` — przywracanie interpunkcji i granic zdań przez skrypt Python oraz zewnętrzne pakiety Python.

## Wymagania

- .NET 10 SDK
- C# 14
- Windows dla funkcjonalności opartych o Python launcher w `iSukces.Cli.Python`
- Zewnętrzne narzędzia wymagane przez wybrane pakiety:
  - FFmpeg dla `iSukces.Cli.Ffmpeg`
  - Python, `py`, `pip` lub `uv` dla pakietów opartych o Python
  - OpenAI Whisper CLI dla `iSukces.Cli.Whisper`
  - pakiety Python `torch`, `transformers` i `sentencepiece` dla `iSukces.Cli.Punctuate`

## Instalacja

Zainstaluj tylko pakiet wymagany przez aplikację:

```powershell
dotnet add package iSukces.CliTools
dotnet add package iSukces.Cli.Python
dotnet add package iSukces.Cli.Ffmpeg
dotnet add package iSukces.Cli.Whisper
dotnet add package iSukces.Cli.Punctuate
```

## Użycie

Uruchomienie procesu przez bazową warstwę CLI:

```csharp
var result = await new Cli
{
    FileName = "dotnet",
    Arguments = ["--info"]
}.RunAsync();

Console.WriteLine(result.Output);
```

Uruchomienie FFmpeg przez wrapper:

```csharp
var result = await new FFmpegCli
{
    InputFile = "input.wav",
    OutFile = "output.mp3",
    OutputFormat = FFmpegOutputFormat.Mp3,
    AudioBitrate = FFmpegBitrate.FromKilobites(192)
}.RunAsync(workingDirectory: null);
```

Uruchomienie Whisper CLI ze skonfigurowanego wirtualnego środowiska Python:

```csharp
var result = await new WhisperCli
{
    Venv = pythonVenv,
    InputFile = "audio.mp3",
    Model = WhisperModel.Turbo,
    OutputFormat = WhisperOutputFormat.Srt
}.RunAsync(workingDirectory: null);
```

Przywracanie interpunkcji w tekście:

```csharp
var venv = await PunctuateVenvFactory.CreateVenv(workingDirectory);
var sentences = await new PunctExec(venv).Process("this is text without punctuation can it be restored");
```

## Budowanie

Z katalogu głównego repozytorium:

```powershell
dotnet build .\App\iSukces.Cli.sln
```

Projekty generują pakiety NuGet podczas budowania.

## Komponenty zewnętrzne

Część pakietów uruchamia zewnętrzne narzędzia jako osobne procesy systemu operacyjnego. Te narzędzia i modele nie są częścią pakietów NuGet i muszą być zainstalowane albo dostarczone przez środowisko użytkownika.

Użytkownik odpowiada za licencje i wymagania uruchomieniowe zewnętrznych narzędzi, pakietów Python, modeli AI, sterowników GPU oraz wygenerowanych wyników.

## Licencja

Pakiety projektu deklarują licencję MIT.
