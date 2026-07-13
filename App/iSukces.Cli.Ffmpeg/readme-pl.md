# iSukces.Cli.Ffmpeg

[English version](readme.md)

[![NuGet](https://img.shields.io/nuget/v/iSukces.Cli.Ffmpeg.svg)](https://www.nuget.org/packages/iSukces.Cli.Ffmpeg/)

Biblioteka .NET do uruchamiania narzędzia FFmpeg z kodu C#. Dostarcza model konfiguracji wywołania `ffmpeg`, buduje argumenty procesu i korzysta z bazowego mechanizmu uruchamiania procesów CLI z pakietu iSukces.CliTools.

## Wymagania

- .NET 10
- Zainstalowany FFmpeg dostępny jako polecenie `ffmpeg` w systemie operacyjnym albo w środowisku procesu

## Zewnętrzne narzędzia

- FFmpeg i proces `ffmpeg` są zależnościami zewnętrznymi dostarczanymi poza tym pakietem.
- Oryginalny projekt FFmpeg jest dostępny pod adresem [https://ffmpeg.org/](https://ffmpeg.org/), a informacje prawne projektu pod adresem [https://ffmpeg.org/legal.html](https://ffmpeg.org/legal.html).
- Pakiet nie zawiera, nie dystrybuuje, nie instaluje, nie pobiera, nie linkuje i nie licencjonuje narzędzia FFmpeg, bibliotek `libav*` ani kodeków FFmpeg.
- Pakiet uruchamia niezależnie zainstalowany plik wykonywalny FFmpeg jako osobny proces systemu operacyjnego, przekazując mu argumenty i ewentualnie komunikując się przez standardowe strumienie procesu.
- Użytkownik odpowiada za instalację FFmpeg oraz za zgodność użycia i ewentualnej dystrybucji wybranego builda FFmpeg z jego właściwą licencją.
- Dostępne formaty, kodeki i zachowanie przetwarzania zależą od zainstalowanej wersji FFmpeg oraz konfiguracji systemu.

## Instalacja

Pakiet NuGet:

```powershell
dotnet add package iSukces.Cli.Ffmpeg
```

## Użycie

Minimalne uruchomienie konwersji audio:

```csharp
var result = await new FFmpegCli
{
    InputFile = "input.wav",
    OutFile = "output.mp3",
    OutputFormat = FFmpegOutputFormat.Mp3,
    AudioBitrate = FFmpegBitrate.FromKilobites(192)
}.RunAsync(workingDirectory: null);
```

Wyłączenie wideo i zapis audio:

```csharp
var result = await new FFmpegCli
{
    InputFile = "movie.mp4",
    OutFile = "audio.wav",
    DisableVideo = true,
    OutputFormat = FFmpegOutputFormat.Wav
}.RunAsync(workingDirectory: null);
```

## Główne typy

### [`FFmpegCli`](FFmpegCli.cs:8)

Buduje argumenty dla procesu `ffmpeg` i uruchamia go przez bazowy mechanizm CLI. Obsługuje plik wejściowy, plik wyjściowy, wyłączenie audio lub wideo, liczbę kanałów audio, częstotliwość próbkowania, bitrate, głośność, format wyjściowy oraz własne argumenty przekazywane do FFmpeg.

### [`FFmpegBitrate`](FFmpegBitrate.cs:5)

Reprezentuje wartość bitrate wraz z jednostką i formatuje ją jako argument FFmpeg. Metoda [`FFmpegBitrate.FromKilobites()`](FFmpegBitrate.cs:7) tworzy bitrate w kilobitach.

### [`FFmpegOutputFormat`](FFmpegOutputFormat.cs:3)

Określa obsługiwany format wyjściowy: `Mp3`, `Wav`, `Aac` albo `Ogg`.

## Budowanie

```powershell
dotnet build ..\iSukces.Cli.sln
```

## Konfiguracja projektu

- Target framework: `net10.0`
- Licencja pakietu: MIT

## Zależności

- [`iSukces.CliTools`](../iSukces.CliTools/readme-pl.md) — bazowa biblioteka do uruchamiania procesów CLI i zbierania argumentów.
- Zewnętrznie instalowane narzędzie: FFmpeg dostępny jako proces `ffmpeg`.

## Znaki towarowe

FFmpeg jest znakiem towarowym Fabrice'a Bellarda, twórcy projektu FFmpeg, zgodnie z informacją opublikowaną na stronie [FFmpeg Legal](https://ffmpeg.org/legal.html). Nazwa FFmpeg jest używana w tym pakiecie wyłącznie opisowo w celu wskazania zgodności z zewnętrznym narzędziem FFmpeg. Ten pakiet nie jest tworzony, sponsorowany ani zatwierdzony przez projekt FFmpeg.

## Copyright FFmpeg

FFmpeg jest projektem zewnętrznym. Prawa autorskie do FFmpeg należą do odpowiednich autorów i właścicieli praw autorskich projektu FFmpeg. Kod źródłowy narzędzia `ffmpeg` w repozytorium upstream zawiera m.in. informację `Copyright (c) 2000-2003 Fabrice Bellard` w pliku `fftools/ffmpeg.c`. Ten pakiet nie zawiera kodu źródłowego FFmpeg; uruchamia tylko zewnętrznie zainstalowany proces `ffmpeg`.

## Model integracji z FFmpeg

Ten pakiet działa jako wrapper procesu. Nie kopiuje kodu FFmpeg, nie linkuje bibliotek `libav*`, nie dołącza binariów FFmpeg i nie dystrybuuje FFmpeg. Integracja polega na wywołaniu osobnego programu przez system operacyjny z argumentami wiersza poleceń oraz opcjonalną komunikacją przez standardowe wejście i wyjście procesu.

Zgodnie z GNU FAQ komunikacja przez `exec`, argumenty wiersza poleceń i potoki jest zwykle traktowana jako komunikacja między oddzielnymi programami, a nie jako jeden połączony utwór. To podsumowanie ma charakter techniczny i nie stanowi porady prawnej.

## Ograniczenie odpowiedzialności

Pakiet udostępnia wyłącznie warstwę .NET uruchamiającą zewnętrzny proces `ffmpeg` w systemie operacyjnym. Działa jako pomost/fasada do narzędzia dostarczonego i instalowanego poza tym pakietem.

Pakiet nie zawiera FFmpeg, nie modyfikuje działania FFmpeg i nie odpowiada za wyniki, błędy, wymagania środowiskowe, zużycie zasobów, kodeki, licencje zewnętrznych komponentów ani szkody wynikające z użycia tego zewnętrznego narzędzia.
