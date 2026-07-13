using System.Globalization;

namespace iSukces.Cli.Ffmpeg;

public record FFmpegBitrate(int Value, string Unit)
{
    public static FFmpegBitrate FromKilobites(int value) => new(value, "k");
    
    public override string ToString() => $"{Value.ToString(CultureInfo.InvariantCulture)}{Unit}";
}


/*
public class FFmpegBitrateX
{
    public FFmpegBitrateX(int value, string unit)
    {
        Value = value;
        Unit  = unit;
    }

    public int    Value { get;}
    public string Unit  { get; }
}
*/
