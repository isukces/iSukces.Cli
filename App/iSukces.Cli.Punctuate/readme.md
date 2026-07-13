# iSukces.Cli.Punctuate

[Polska wersja](readme-pl.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Punctuate.svg)](https://www.nuget.org/packages/iSukces.Cli.Punctuate/)

.NET library for restoring punctuation and sentence boundaries in text from C# code. The package starts a Python processing script as a separate operating-system process, caches results, and applies final sentence post-processing on the .NET side. Optional helper APIs can prepare a Python environment by orchestrating external tools in the user's operating-system environment.

## Requirements

- .NET 10
- Python 3.10.x represented by [`PythonVenv`](../iSukces.Cli.Python/PythonVenv.cs:3)
- `uv.exe` available if the consuming application uses the optional environment-preparation helper
- Access to the Python packages `torch`, `transformers`, and `sentencepiece`
- Access to the Hugging Face model `kredor/punctuate-all` if it is not already available in the local cache

## External components and AI models

- Python, `uv`, `pip`, the created `venv`, PyTorch (`torch`), Hugging Face Transformers (`transformers`), SentencePiece (`sentencepiece`), and the `kredor/punctuate-all` model are external components used outside this package code.
- Installing this NuGet package does not install Python packages, create a virtual environment, download models, or install external tools.
- [`PunctuateVenvFactory`](PunctuateVenvFactory.cs:7) is an optional helper that may be called by the consuming application to orchestrate external tools in the user's environment. When explicitly invoked, it may start `uv`, create a virtual environment, and run `uv pip install torch transformers sentencepiece` as separate operating-system processes.
- The [`punctuate_process.py`](Resources/punctuate_process.py:1) script uses `transformers.pipeline` and the `kredor/punctuate-all` model. Depending on the user's Hugging Face/Transformers configuration, that external runtime may download the model at run time if it is not present in the local cache.
- This package does not include, distribute, or license Python, PyTorch, Hugging Face Transformers, SentencePiece, or the `kredor/punctuate-all` model.
- Users are responsible for installing external dependencies and ensuring that their use of selected Python packages, AI models, and generated outputs complies with the applicable licenses and terms.

## Integration model

This package acts as a process wrapper. [`PunctExec`](PunctExec.cs:9) writes a temporary Python script and an input file, then starts `python.exe` through the [`Cli`](../iSukces.CliTools/Cli.cs:1) process execution layer, passing command-line arguments: the script file, the input file, and the output JSON file. The JSON result is read back from disk and post-processed on the .NET side.

This means communication between separate programs through operating-system mechanisms: `exec`/process launch, command-line arguments, and input/output files. Similarly, when programs communicate through pipes or standard process streams, this is usually communication between separate programs rather than one combined work. This technical summary is not legal advice.

## Installation

```powershell
dotnet add package iSukces.Cli.Punctuate
```

## Usage

```csharp
var venv = await PunctuateVenvFactory.CreateVenv(workingDirectory);
var sentences = await new PunctExec(venv).Process("this is text without punctuation can it be restored");
```

## Main types

### [`PunctuateVenvFactory`](PunctuateVenvFactory.cs:7)

Optional helper that creates and prepares a Python environment by starting external tools in the user's operating-system environment. It may run installation commands for external Python packages, but those packages remain outside this NuGet package.

### [`PunctExec`](PunctExec.cs:9)

Runs the Python script as a separate process, splits input text into chunks, persists state and cache, and sends the result to post-processing.

### [`PunctuationPostProcessor`](PunctuationPostProcessor.cs:5)

Processes the token-classification result returned by the Python script and reconstructs punctuated sentences.

### [`PunctuateSentencesExtractor`](PunctuateSentencesExtractor.cs:13)

Service adapter that resolves the `punctuate` environment and returns sentences.

## Project configuration

- Target framework: `net10.0`
- Package license: MIT for the `iSukces.Cli.Punctuate` package code only

## Dependencies

- [`iSukces.CliTools`](../iSukces.CliTools/readme.md) — base library for running CLI processes and building arguments.
- [`iSukces.Cli.Python`](../iSukces.Cli.Python/readme.md) — Python, `venv`, and process environment integration.
- External components installed or used in the user's environment: Python, `uv`, `pip`, `torch`, `transformers`, `sentencepiece`, and the Hugging Face model `kredor/punctuate-all`.
- Transitive NuGet dependencies reported by `dotnet list package --include-transitive`: `Microsoft.SourceLink.GitHub`, `Microsoft.Build.Tasks.Git`, `Microsoft.SourceLink.Common`, `System.IO.Hashing`, `Newtonsoft.Json`, `YamlDotNet`, `iSukces.Translation`.

## Licenses and responsibility

The MIT license declared by this NuGet package applies to the `iSukces.Cli.Punctuate` package code, including the C# code and the bundled helper Python script. It does not grant rights to Python, PyTorch, Hugging Face Transformers, SentencePiece, the `kredor/punctuate-all` model, or any other component installed or downloaded outside this package.

The names Hugging Face, Transformers, PyTorch, Python, and `kredor/punctuate-all` are used descriptively only to identify compatibility and technical dependencies. This package is not created, sponsored, or endorsed by the authors of those projects or models.

This package is not responsible for licenses, results, failures, environment requirements, resource usage, model downloads, behavior of external components, or damages resulting from the use of external libraries, tools, or AI models.

If a consuming application chooses to call helper methods that start `uv`, `pip`, `py`, Python, or Hugging Face/Transformers code, those processes run in the user's operating-system environment and remain external to this NuGet package. The consuming application and its user are responsible for selecting exact versions, accepting any applicable terms, and verifying licenses of the external packages, tools, models, and downloaded artifacts.
