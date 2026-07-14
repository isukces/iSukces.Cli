// Descriptions taken from
// https://gist.githubusercontent.com/tayvano/6e2d456a9897f55025e25035478a3a50/raw/ed7adedee303a17bb05251f2abba4defeb3ac03a/gistfile1.txt

using iSukces.CliTools;

namespace iSukces.Cli.Ffmpeg;

/// <summary>
/// Configuration and execution wrapper for FFmpeg CLI calls.
/// </summary>
public sealed partial class FFmpegCli
{
    private sealed class FfmpegArgumentCollector : ArgumentCollector
    {
    }

    /// <summary>
    /// Command-line arguments created from the current FFmpeg configuration.
    /// </summary>
    /// <returns>The command-line arguments for the FFmpeg process.</returns>
    public string[] GetArguments()
    {
        var args = new FfmpegArgumentCollector();
        args.OptionPrefix = "-";
        args.Add(InputFile, "i");


        // video options
        if (DisableVideo == true)
            args.AddOption("vn");
        args.Add(VideoFrames, "vframes");

        // audio options
        if (DisableAudio == true)
            args.AddOption("an");
        args.Add(AudioChannelsNumber, "ac");
        args.Add(AudioRateHz, "ar");
        args.Add(NumberOfAudioFramesToRecord, "aframes");
        args.Add(Volume, "vol");
        args.Add(AudioBitrate?.ToString(), "ab");
        
        // output options
        args.AddSnake(OutputFormat, "f");
        if (Custom != null)
            foreach (var c in Custom)
                args.Add(c);
        args.Add(OutFile);
        return args.ToArray();
    }

    /// <summary>
    /// Runs the configured FFmpeg process asynchronously.
    /// </summary>
    /// <param name="workingDirectory">Working directory for the FFmpeg process.</param>
    /// <returns>The captured CLI execution result.</returns>
    public async Task<CliResult> RunAsync(string? workingDirectory)
    {
        Console.WriteLine("Running ffmpeg");
        var runner = new CliTools.Cli
        {
            FileName         = "ffmpeg",
            Arguments        = GetArguments().ToArray(),
            WorkingDirectory = workingDirectory,
            ThrowIfFailed    = true
        };
        var r = await runner.RunAsync();
        //Console.WriteLine("Done " + r.Duration.TotalMinutes + " minutes");
        return r;
    }

    /// <summary>
    /// Input media file passed to FFmpeg.
    /// </summary>
    public string InputFile { get; set; }

    /// <summary>
    /// Output media file produced by FFmpeg.
    /// </summary>
    public required string OutFile { get; set; }

    /// <summary>
    /// Custom FFmpeg arguments appended before the output file.
    /// </summary>
    public string[]? Custom { get; set; }
}

//=========== VIDEO
/// <summary>
/// Video-related FFmpeg configuration options.
/// </summary>
public sealed partial class FFmpegCli
{
     
    /// <summary>
    /// Video stream disabling option.
    /// </summary>
    public bool? DisableVideo { get; set; }
    
    /// <summary>
    /// Number of video frames to record.
    /// </summary>
    public int? VideoFrames { get; set; }
}



//=========== AUDIO
/// <summary>
/// Audio-related FFmpeg configuration options.
/// </summary>
public sealed partial class FFmpegCli
{
    
    /// <summary>
    /// Audio stream disabling option.
    /// </summary>
    public bool? DisableAudio { get; set; }
    
    /// <summary>
    /// Number of audio channels.
    /// </summary>
    public int? AudioChannelsNumber { get; set; }


    /// <summary>
    /// Audio sampling rate in hertz.
    /// </summary>
    public int? AudioRateHz { get; set; }

    /// <summary>
    /// Output container format passed to FFmpeg.
    /// </summary>
    public FFmpegOutputFormat? OutputFormat { get; set; }

    /// <summary>
    /// Number of audio frames to record.
    /// </summary>
    public int? NumberOfAudioFramesToRecord { get; set; }

    /// <summary>
    /// Audio bitrate passed to FFmpeg.
    /// </summary>
    public FFmpegBitrate? AudioBitrate { get; set; }
    
    /// <summary>
    /// Audio volume value where 256 means normal volume.
    /// </summary>
    public int? Volume { get; set; }
}
