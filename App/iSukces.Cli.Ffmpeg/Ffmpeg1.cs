using iSukces.CliTools;
// Descriptions taken from
// https://gist.githubusercontent.com/tayvano/6e2d456a9897f55025e25035478a3a50/raw/ed7adedee303a17bb05251f2abba4defeb3ac03a/gistfile1.txt

namespace iSukces.Cli.Ffmpeg;

public sealed class FFmpegCli1
{
    private sealed class FfmpegArgumentCollector : ArgumentCollector
    {
    }

    public string[] GetArguments()
    {
        var args = new FfmpegArgumentCollector();
        args.OptionPrefix = "-";
        args.Add(InputFile, "i");
        if (DisableAudio == true)
            args.AddOption("an");
        args.Add(AudioChannelsNumber, "ac");
        args.Add(Volume, "vol");
        args.Add(AudioBitrate?.ToString(), "ab");
        args.AddSnake(OutputFormat, "f");
        if (DisableVideo == true)
            args.AddOption("vn");
        return args.ToArray();
    }
    
    // automatycznie dodane propertisy
    /// <summary>
    /// input_file set input file
    /// </summary>
    public string InputFile { get; set; }

    /// <summary>
    /// disable audio
    /// </summary>
    public bool? DisableAudio { get; set; }

    /// <summary>
    /// channels set number of audio channels
    /// </summary>
    public int? AudioChannelsNumber { get; set; }

    /// <summary>
    /// volume change audio volume (256=normal)
    /// </summary>
    public int? Volume { get; set; }

    /// <summary>
    /// set bitrate (in bits/s) (from 0 to INT_MAX) (default 128000)
    /// </summary>
    public FFmpegBitrate? AudioBitrate { get; set; }

    /// <summary>
    /// ???????????
    /// </summary>
    public FFmpegOutputFormat? OutputFormat { get; set; }

    /// <summary>
    /// disable video
    /// </summary>
    public bool? DisableVideo { get; set; }

}
