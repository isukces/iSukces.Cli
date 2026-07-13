# iSukces.Cli

[Polska wersja](README-pl.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.CliTools.svg)](https://www.nuget.org/packages/iSukces.CliTools/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Python.svg)](https://www.nuget.org/packages/iSukces.Cli.Python/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Ffmpeg.svg)](https://www.nuget.org/packages/iSukces.Cli.Ffmpeg/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Whisper.svg)](https://www.nuget.org/packages/iSukces.Cli.Whisper/)
[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Punctuate.svg)](https://www.nuget.org/packages/iSukces.Cli.Punctuate/)

`iSukces.Cli` is a .NET 10 solution containing libraries for running external command-line tools from C# code. The solution includes a base CLI process execution layer and packages for Python environments, FFmpeg, OpenAI Whisper CLI, and punctuation restoration through an external Python process.

## Projects

- `iSukces.CliTools` — base library for asynchronous CLI process execution, argument building, environment handling, and process results.
- `iSukces.Cli.Python` — Python virtual environment management, pip package installation, PyTorch and CUDA Toolkit detection, HuggingFace token handling, and process environment setup.
- `iSukces.Cli.Ffmpeg` — wrapper for running an externally installed `ffmpeg` process.
- `iSukces.Cli.Whisper` — wrapper for running OpenAI Whisper CLI from a Python virtual environment.
- `iSukces.Cli.Punctuate` — punctuation and sentence boundary restoration through a Python script and external Python packages.

## Requirements

- .NET 10 SDK
- C# 14
- Windows for Python launcher based functionality in `iSukces.Cli.Python`
- External tools required by selected packages:
  - FFmpeg for `iSukces.Cli.Ffmpeg`
  - Python, `py`, `pip` or `uv` for Python-based packages
  - OpenAI Whisper CLI for `iSukces.Cli.Whisper`
  - Python packages `torch`, `transformers`, and `sentencepiece` for `iSukces.Cli.Punctuate`

## Installation

Install only the package required by your application:

```powershell
dotnet add package iSukces.CliTools
dotnet add package iSukces.Cli.Python
dotnet add package iSukces.Cli.Ffmpeg
dotnet add package iSukces.Cli.Whisper
dotnet add package iSukces.Cli.Punctuate
```

## Usage

Run a process with the base CLI layer:

```csharp
var result = await new Cli
{
    FileName = "dotnet",
    Arguments = ["--info"]
}.RunAsync();

Console.WriteLine(result.Output);
```

Run FFmpeg through the wrapper:

```csharp
var result = await new FFmpegCli
{
    InputFile = "input.wav",
    OutFile = "output.mp3",
    OutputFormat = FFmpegOutputFormat.Mp3,
    AudioBitrate = FFmpegBitrate.FromKilobites(192)
}.RunAsync(workingDirectory: null);
```

Run Whisper CLI from a configured Python virtual environment:

```csharp
var result = await new WhisperCli
{
    Venv = pythonVenv,
    InputFile = "audio.mp3",
    Model = WhisperModel.Turbo,
    OutputFormat = WhisperOutputFormat.Srt
}.RunAsync(workingDirectory: null);
```

Restore punctuation in text:

```csharp
var venv = await PunctuateVenvFactory.CreateVenv(workingDirectory);
var sentences = await new PunctExec(venv).Process("this is text without punctuation can it be restored");
```

## Build

From the repository root:

```powershell
dotnet build .\App\iSukces.Cli.sln
```

The projects generate NuGet packages during build.

## External components

Some packages start external tools as separate operating-system processes. These tools and models are not included in the NuGet packages and must be installed or provided by the user environment.

Users are responsible for the licensing and runtime requirements of external tools, Python packages, AI models, GPU drivers, and generated outputs.

## License

Project packages declare the MIT license.
