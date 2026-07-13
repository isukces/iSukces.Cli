# iSukces.CliTools

[Polska wersja](readme-pl.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.CliTools.svg)](https://www.nuget.org/packages/iSukces.CliTools/)

Helper library for running external CLI tools from .NET applications. It provides classes for asynchronous process execution, argument building, environment variable handling, stdout/stderr capture, and returning process results in a consistent model.

## Requirements

- .NET 10

## Installation

NuGet package:

```powershell
dotnet add package iSukces.CliTools
```

## Usage

Minimal process execution:

```csharp
var result = await new Cli
{
    FileName = "dotnet",
    Arguments = ["--info"]
}.RunAsync();

Console.WriteLine(result.Output);
```

Building command-line arguments:

```csharp
var arguments = new ArgumentCollector();
arguments.Add("input.mp4");
arguments.Add("output.mp3", "output");
arguments.Add(true, "overwrite", "no-overwrite");
```

## Main types

### [`Cli`](Cli.cs:6)

Runs external processes asynchronously. It supports executable file selection, arguments, working directory, environment variables, stdout/stderr redirection, and output display modes: `All`, `Tail`, and `None`.

### [`ArgumentCollector`](ArgumentCollector.cs:5)

Builds CLI argument arrays. It supports plain arguments, prefixed options, boolean switches, numeric and text values, and snake_case conversion through [`AddSnake`](ArgumentCollector.cs:52).

### [`CliResult`](CliResult.cs:3) and [`CliException`](CliException.cs:3)

[`CliResult`](CliResult.cs:3) stores the exit code, stdout, stderr, start time, finish time, duration, and full invocation data. [`CreateException`](CliResult.cs:5) creates a [`CliException`](CliException.cs:3) with process error details.

### [`Result<T>`](Result.cs:46) and [`Result`](Result.cs:3)

Operation result model containing a value, success status, exception, and optional [`CliResult`](CliResult.cs:3). [`Make`](Result.cs:5), [`MakeAsync`](Result.cs:10), and [`FromCliCallEx`](Result.cs:15) wrap synchronous, asynchronous, and CLI calls.

### [`EnvironmentUpdater`](EnvironmentUpdater.cs:5)

Prepares environment variables for a process, prepends directories to `PATH`, activates changes temporarily, and searches for executable files in configured paths.

### [`TailConsoleOutput`](TailConsoleOutput.cs:8)

Displays the last process output lines in the console. It is used by [`Cli`](Cli.cs:6) when the `Tail` output mode is selected.

### [`WindowsConsoleUtils`](WindowsConsoleUtils.cs:5)

Contains [`EscapeCommandLineParameterIfNecessary`](WindowsConsoleUtils.cs:7), which escapes command-line arguments according to Windows parsing rules.

### [`TextExtensions`](TextExtensions.cs:3)

Contains text extensions, including [`ToSnake`](TextExtensions.cs:5) and [`Coalesce`](TextExtensions.cs:35).

## Build

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Project configuration

- Target framework: `net10.0`
- Package license: MIT

## Disclaimer

This package only provides a .NET layer that starts the selected external process in the operating system. It acts as a bridge/facade for a tool supplied and installed outside this package.

This package does not include the executed tool, does not modify how it works, and is not responsible for its results, failures, environment requirements, resource usage, or any damages resulting from the use of that external tool.
