# iSukces.Cli.Ffmpeg

[Polska wersja](readme-pl.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Ffmpeg.svg)](https://www.nuget.org/packages/iSukces.Cli.Ffmpeg/)

.NET library for running FFmpeg from C# code. It provides a configuration model for `ffmpeg` calls, builds process arguments, and uses the base CLI process execution layer from the iSukces.CliTools package.

## Requirements

- .NET 10
- FFmpeg installed and available as the `ffmpeg` command in the operating system or process environment

## External tools

- FFmpeg and the `ffmpeg` process are external dependencies supplied outside this package.
- The original FFmpeg project is available at [https://ffmpeg.org/](https://ffmpeg.org/), and the project's legal information is available at [https://ffmpeg.org/legal.html](https://ffmpeg.org/legal.html).
- This package does not include, distribute, install, download, link against, or license FFmpeg, the `libav*` libraries, or FFmpeg codecs.
- This package invokes an independently installed FFmpeg executable as a separate operating-system process, passing command-line arguments and optionally communicating through the process standard streams.
- Users are responsible for installing FFmpeg and ensuring that their use and distribution of the chosen FFmpeg build complies with the license applicable to that build.
- Available formats, codecs, and processing behavior depend on the installed FFmpeg version and system configuration.

## Installation

NuGet package:

```powershell
dotnet add package iSukces.Cli.Ffmpeg
```

## Usage

Minimal audio conversion call:

```csharp
var result = await new FFmpegCli
{
    InputFile = "input.wav",
    OutFile = "output.mp3",
    OutputFormat = FFmpegOutputFormat.Mp3,
    AudioBitrate = FFmpegBitrate.FromKilobites(192)
}.RunAsync(workingDirectory: null);
```

Disable video and save audio:

```csharp
var result = await new FFmpegCli
{
    InputFile = "movie.mp4",
    OutFile = "audio.wav",
    DisableVideo = true,
    OutputFormat = FFmpegOutputFormat.Wav
}.RunAsync(workingDirectory: null);
```

## Main types

### [`FFmpegCli`](FFmpegCli.cs:8)

Builds arguments for the `ffmpeg` process and runs it through the base CLI mechanism. It supports input file, output file, disabling audio or video, audio channel count, sampling rate, bitrate, volume, output format, and custom arguments passed to FFmpeg.

### [`FFmpegBitrate`](FFmpegBitrate.cs:5)

Represents a bitrate value with a unit and formats it as an FFmpeg argument. [`FFmpegBitrate.FromKilobites()`](FFmpegBitrate.cs:7) creates a bitrate value in kilobits.

### [`FFmpegOutputFormat`](FFmpegOutputFormat.cs:3)

Defines the supported output format: `Mp3`, `Wav`, `Aac`, or `Ogg`.

## Build

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Project configuration

- Target framework: `net10.0`
- Package license: MIT

## Dependencies

- [`iSukces.CliTools`](../iSukces.CliTools/readme.md) — base library for running CLI processes and building arguments.
- Externally installed tool: FFmpeg available as the `ffmpeg` process.

## Trademarks

FFmpeg is a trademark of Fabrice Bellard, originator of the FFmpeg project, according to the notice published on the [FFmpeg Legal](https://ffmpeg.org/legal.html) page. The FFmpeg name is used in this package descriptively only to indicate compatibility with the external FFmpeg tool. This package is not created, sponsored, or endorsed by the FFmpeg project.

## FFmpeg copyright

FFmpeg is an external project. Copyrights in FFmpeg belong to the respective authors and copyright holders of the FFmpeg project. The upstream source code of the `ffmpeg` tool includes, among others, the notice `Copyright (c) 2000-2003 Fabrice Bellard` in the `fftools/ffmpeg.c` file. This package does not include FFmpeg source code; it only starts an externally installed `ffmpeg` process.

## FFmpeg integration model

This package acts as a process wrapper. It does not copy FFmpeg code, link against `libav*` libraries, bundle FFmpeg binaries, or distribute FFmpeg. The integration consists of invoking a separate program through the operating system with command-line arguments and optional communication through the process standard input and output streams.

According to the GNU FAQ, communication through `exec`, command-line arguments, and pipes is normally communication between separate programs rather than one combined work. This summary is technical information and is not legal advice.

## Disclaimer

This package only provides a .NET layer that starts the external `ffmpeg` process in the operating system. It acts as a bridge/facade for a tool supplied and installed outside this package.

This package does not include FFmpeg, does not modify how FFmpeg works, and is not responsible for its results, failures, environment requirements, resource usage, codecs, external component licenses, or any damages resulting from the use of that external tool.
