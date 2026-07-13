# iSukces.Cli.Whisper

[Polska wersja](readme-pl.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Whisper.svg)](https://www.nuget.org/packages/iSukces.Cli.Whisper/)

.NET library for running OpenAI Whisper CLI from C# code. It provides a configuration model for `whisper.exe` calls, builds process arguments, supports transcription and translation tasks, integrates execution with a Python virtual environment, and includes predefined Whisper model names and metadata.

## Requirements

- .NET 10
- OpenAI Whisper CLI installed and available in the Python environment
- Python virtual environment represented by `PythonVenv`

## External tools and AI models

- OpenAI Whisper CLI, the `whisper.exe` process, Python, and the Python virtual environment are external dependencies supplied outside this package.
- The OpenAI Whisper project is available at [https://github.com/openai/whisper](https://github.com/openai/whisper), and its repository currently declares the MIT license at [https://github.com/openai/whisper/blob/main/LICENSE](https://github.com/openai/whisper/blob/main/LICENSE).
- Whisper models, including `tiny`, `base`, `small`, `medium`, `large`, and `turbo`, are external AI models used by OpenAI Whisper CLI; this package stores only their names and configuration metadata.
- PyTorch, CUDA, GPU drivers, and other dependencies required by Whisper CLI are not part of this package.
- This package does not include, distribute, install, download, link against, or license OpenAI Whisper, Whisper models, Python, PyTorch, CUDA, or GPU drivers.
- This package invokes an independently installed `whisper.exe` executable as a separate operating-system process, passing command-line arguments through the process execution layer.
- Users are responsible for installing OpenAI Whisper and ensuring that their use of the selected Whisper version, models, Python packages, GPU stack, and generated outputs complies with the licenses and terms applicable to those external components.

## Installation

NuGet package:

```powershell
dotnet add package iSukces.Cli.Whisper
```

## Usage

Minimal transcription call:

```csharp
var result = await new WhisperCli
{
    Venv = pythonVenv,
    InputFile = "audio.mp3",
    Model = WhisperModel.Turbo,
    OutputFormat = WhisperOutputFormat.Srt
}.RunAsync(workingDirectory: null);
```

Audio translation to English:

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

Do not type language names directly as string literals, for example `Language = "Japanese"`. Use constants from the [`WhisperLanguages`](WhisperLanguages.cs:4) class, for example [`WhisperLanguages.Japanese`](WhisperLanguages.cs:294) or [`WhisperLanguages.Ja`](WhisperLanguages.cs:84), to keep values consistent with languages accepted by Whisper CLI.

## Main types

### [`WhisperCli`](WhisperCli.cs:9)

Builds arguments for `whisper.exe` and runs the process through the base CLI mechanism. It supports input file, model, language, output directory, output format, task, temperature, initial prompt, time range, and options related to previous-text context.

### [`WhisperModel`](WhisperModel.cs:6)

Describes Whisper models with parameter count, multilingual model name, English-only model name, VRAM requirements, and relative speed. The class exposes predefined models: [`WhisperModel.Tiny`](WhisperModel.cs:60), [`WhisperModel.Base`](WhisperModel.cs:70), [`WhisperModel.Small`](WhisperModel.cs:80), [`WhisperModel.Medium`](WhisperModel.cs:90), [`WhisperModel.Large`](WhisperModel.cs:100), and [`WhisperModel.Turbo`](WhisperModel.cs:110). [`WhisperModel.All`](WhisperModel.cs:50) contains all defined models.

### [`WhisperLanguages`](WhisperLanguages.cs:4)

Contains string constants for languages accepted by OpenAI Whisper CLI. The class exposes both short language codes, for example [`WhisperLanguages.Ja`](WhisperLanguages.cs:84), [`WhisperLanguages.Pl`](WhisperLanguages.cs:140), and [`WhisperLanguages.En`](WhisperLanguages.cs:42), and English language names, for example [`WhisperLanguages.Japanese`](WhisperLanguages.cs:294), [`WhisperLanguages.Polish`](WhisperLanguages.cs:358), and [`WhisperLanguages.English`](WhisperLanguages.cs:252). Use these constants to set [`WhisperCli.Language`](WhisperCli.cs:89) without hard-coded string literals.

### [`WhisperTask`](WhisperCli.cs:120)

Defines the Whisper task mode: `Transcribe` or `Translate`.

### [`WhisperOutputFormat`](WhisperCli.cs:126)

Defines the output format: `Txt`, `Vtt`, `Srt`, `Tsv`, `Json`, or `All`.

## Build

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Project configuration

- Target framework: `net10.0`
- Package license: MIT for the iSukces.Cli.Whisper package code only

## Dependencies

- [`iSukces.CliTools`](../iSukces.CliTools/readme.md) — base library for running CLI processes and building arguments.
- [`iSukces.Cli.Python`](../iSukces.Cli.Python/readme.md) — Python and virtual environment process integration.
- Externally installed tools and libraries: OpenAI Whisper CLI, Python, PyTorch/CUDA according to the Whisper environment requirements.

## OpenAI Whisper copyright and license

OpenAI Whisper is an external project. Copyrights in OpenAI Whisper, its documentation, and related assets belong to their respective authors and copyright holders. This package does not include OpenAI Whisper source code, binaries, Python packages, or model files; it only stores selected configuration names and metadata needed to call an externally installed `whisper.exe` process.

The MIT license declared by this NuGet package applies to the iSukces.Cli.Whisper package code. It does not grant rights to OpenAI Whisper, Whisper models, Python, PyTorch, CUDA, GPU drivers, or any other component installed outside this package. Users should review the license and usage terms for every external component and model they install.

## Integration model

This package acts as a process wrapper. It does not copy OpenAI Whisper code, bundle Whisper binaries, bundle model files, install Python packages, or link against OpenAI Whisper internals. The integration consists of invoking a separate program through the operating system with command-line arguments.

The package includes model names, language constants, and argument-building helpers so that C# applications can configure an external Whisper CLI invocation without hard-coded command-line strings. This technical summary is not legal advice.

## Trademarks

OpenAI and Whisper are used descriptively only to indicate compatibility with the OpenAI Whisper CLI tool. This package is not created, sponsored, or endorsed by OpenAI.

## Disclaimer

This package only provides a .NET layer that starts the external `whisper.exe` process in the operating system. It acts as a bridge/facade for a tool supplied and installed outside this package.

This package does not include the OpenAI Whisper CLI or Whisper models, does not modify how they work, and is not responsible for their licenses, results, failures, environment requirements, resource usage, generated content, or any damages resulting from the use of that external tool, its dependencies, or AI models.
