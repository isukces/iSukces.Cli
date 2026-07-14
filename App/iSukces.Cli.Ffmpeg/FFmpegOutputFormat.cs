namespace iSukces.Cli.Ffmpeg;

/// <summary>
/// Output container format requested from FFmpeg.
/// </summary>
public enum FFmpegOutputFormat
{
    /// <summary>
    /// MP3 audio output format.
    /// </summary>
    Mp3,

    /// <summary>
    /// WAV audio output format.
    /// </summary>
    Wav,

    /// <summary>
    /// AAC audio output format.
    /// </summary>
    Aac,

    /// <summary>
    /// Ogg audio output format.
    /// </summary>
    Ogg
}
