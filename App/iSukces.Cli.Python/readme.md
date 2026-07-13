# iSukces.Cli.Python

[Polska wersja](readme-pl.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Python.svg)](https://www.nuget.org/packages/iSukces.Cli.Python/)

.NET library for managing Python environments from C# code. It supports virtual environment creation, pip package installation, PyTorch and CUDA Toolkit detection, HuggingFace token handling, and process environment variable management.

## Requirements

- .NET 10
- Windows with the Python `py` launcher

## External tools and AI models

- Python, the `py` launcher, `pip`, optional `uv`, and created `venv` environments are external dependencies supplied outside this package.
- PyTorch (`torch`, `torchvision`, `torchaudio`) and CUDA Toolkit are external libraries/tools installed in the Python environment or in the operating system.
- HuggingFace tokens and resources, private package indexes, and AI models downloaded or used by user code are not part of this package.
- This package does not include AI models and does not guarantee the behavior of models, Python libraries, GPU drivers, or tools installed outside this package.

## Installation

NuGet package:

```powershell
dotnet add package iSukces.Cli.Python
```

## Usage

Basic Python process configuration:

```csharp
var result = await new Cli
{
    FileName = "py",
    Arguments = ["--version"]
}.WithPythonCli().RunAsync();

Console.WriteLine(result.Output);
```

## Main features

### Virtual environment management

- [`PythonVenv`](PythonVenv.cs:5) represents a Python virtual environment. It can create environments with `py -m venv` or `uv venv` and configure process environment variables such as `VIRTUAL_ENV`, `PYTHONHOME`, and `PATH`.
- [`PythonVenv.WithVenv()`](PythonVenv.cs:69) applies virtual environment settings to a [`Cli`](../iSukces.CliTools/Cli.cs:6) process call.
- [`PythonCliExtension.WithPythonCli()`](PythonCliExtension.cs:5) sets `PYTHONIOENCODING=utf-8` for started processes.

### Package installation

- [`Installer`](Installer.cs:7) installs and uninstalls pip packages in a virtual environment. It supports private indexes through `--index-url` and HuggingFace tokens through `HF_TOKEN`.
- The touched-files mechanism stores operation flags in `.prv-flags` and prevents repeated installation work when an operation has already been completed.

### PyTorch and CUDA detection

- [`PyTorchUtils`](PyTorchUtils.cs:3) runs Python code with `torch` and reads the PyTorch version, CUDA availability, and CUDA version reported by PyTorch.
- [`CudaToolkit`](CudaToolkit.cs:6) detects installed CUDA Toolkit versions in `C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA`.
- [`PythonEnvironmentInfo`](PythonEnvironmentInfo.cs:3) aggregates PyTorch and CUDA information and can create an [`EnvironmentUpdater`](../iSukces.CliTools/EnvironmentUpdater.cs:5) that sets `CUDA_PATH` and updates `PATH`.

### Environment diagnostics

- [`PythonEnvironmentProblemFinder`](PythonEnvironmentProblemFinder.cs:14) detects environment problems, including missing or broken PyTorch installation, missing GPU acceleration, and CUDA Toolkit version mismatch.
- [`PythonEnvironmentProblem`](PythonEnvironmentProblem.cs:3) describes a single problem, suggested remedy, and affected areas.

### Installed Python versions

- [`PyUtils.GetInstalledVersions()`](PyUtils.cs:3) runs `py -0` and parses installed Python versions.
- [`PyLauncherParser`](PythonVersionInfo.cs:25) parses Windows Python launcher output and detects version, architecture, and default interpreter.

### Environment variables

- [`EnvironmentUpdater`](../iSukces.CliTools/EnvironmentUpdater.cs:5) modifies environment variables, including `PATH` and `CUDA_PATH`, with temporary activation and restoration through `IDisposable`.

## Dependencies

- [`iSukces.CliTools`](../iSukces.CliTools/readme.md) provides base CLI process execution and argument collection utilities.

## Build

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Project configuration

- Target framework: `net10.0`
- Package license: MIT

## Disclaimer

This package only provides a .NET layer that starts external Python processes and tools in the operating system. It acts as a bridge/facade for tools supplied and installed outside this package.

This package does not include the executed tools, AI models, Python libraries, GPU drivers, or CUDA Toolkit, does not modify how they work, and is not responsible for their results, failures, environment requirements, resource usage, or any damages resulting from the use of those external tools.
