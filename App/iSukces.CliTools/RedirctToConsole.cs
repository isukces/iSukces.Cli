namespace iSukces.CliTools;

/// <summary>
/// Standard process streams redirected to the console.
/// </summary>
[Flags]
public enum RedirctToConsole
{
    /// <summary>
    /// No process stream is redirected to the console.
    /// </summary>
    None,

    /// <summary>
    /// Standard output stream is redirected to the console.
    /// </summary>
    Output = 1,

    /// <summary>
    /// Standard error stream is redirected to the console.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Standard output and standard error streams are redirected to the console.
    /// </summary>
    Both = Output | Error
}
