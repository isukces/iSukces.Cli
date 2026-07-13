// Descriptions taken from
// https://gist.githubusercontent.com/tayvano/6e2d456a9897f55025e25035478a3a50/raw/ed7adedee303a17bb05251f2abba4defeb3ac03a/gistfile1.txt

using iSukces.CliTools;

namespace iSukces.Cli.Ffmpeg;

public sealed partial class FFmpegCli
{
    private sealed class FfmpegArgumentCollector : ArgumentCollector
    {
    }

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
    /// -i input_file set input file
    /// </summary>
    public string InputFile { get; set; }

    /// <summary>
    /// output file
    /// </summary>
    public required string OutFile { get; set; }

    public string[]? Custom { get; set; }
}

//=========== VIDEO
public sealed partial class FFmpegCli
{
     
    /// <summary>
    /// -vn disable video
    /// </summary>
    public bool? DisableVideo { get; set; }
    
    /// <summary>
    /// -vframes number set the number of video frames to record
    /// </summary>
    public int? VideoFrames { get; set; }
}



//=========== AUDIO
public sealed partial class FFmpegCli
{
    
    /// <summary>
    /// -an disable audio
    /// </summary>
    public bool? DisableAudio { get; set; }
    
    /// <summary>
    ///     -ac channels set number of audio channels
    /// </summary>
    /// <returns></returns>
    public int? AudioChannelsNumber { get; set; }


    /// <summary>
    ///     -ar rate set audio sampling rate (in Hz)
    /// </summary>
    public int? AudioRateHz { get; set; }

    public FFmpegOutputFormat? OutputFormat { get; set; }

    /// <summary>
    /// -aframes number set the number of audio frames to record
    /// </summary>
    public int? NumberOfAudioFramesToRecord { get; set; }

    /// <summary>
    /// -ab set bitrate (in bits/s) (from 0 to INT_MAX) (default 128000)
    /// </summary>
    public FFmpegBitrate? AudioBitrate { get; set; }
    
    /// <summary>
    /// vol volume change audio volume (256=normal)
    /// </summary>
    public int? Volume { get; set; }
}
