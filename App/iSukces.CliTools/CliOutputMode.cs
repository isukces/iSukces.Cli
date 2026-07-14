namespace iSukces.CliTools;

/// <summary>
/// Console output display mode for CLI execution.
/// </summary>
public enum CliOutputMode
{
    /// <summary>
    /// Process output is not written to the console.
    /// </summary>
    None,

    /// <summary>
    /// Complete process output is written to the console.
    /// </summary>
    All,

    /// <summary>
    /// Only the latest process output lines are shown in tail mode.
    /// </summary>
    Tail
}
