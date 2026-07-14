using System.Globalization;

namespace iSukces.Cli.Ffmpeg;

/// <summary>
/// FFmpeg bitrate value with its unit suffix.
/// </summary>
/// <param name="Value">Numeric bitrate value.</param>
/// <param name="Unit">FFmpeg bitrate unit suffix.</param>
public record FFmpegBitrate(int Value, string Unit)
{
    /// <summary>
    /// Creates a bitrate value expressed in kilobits.
    /// </summary>
    /// <param name="value">Bitrate value in kilobits.</param>
    /// <returns>A bitrate value with the FFmpeg kilobit suffix.</returns>
    public static FFmpegBitrate FromKilobites(int value) => new(value, "k");
    
    /// <summary>
    /// FFmpeg argument text for this bitrate value.
    /// </summary>
    /// <returns>The invariant-culture bitrate value followed by its unit suffix.</returns>
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
