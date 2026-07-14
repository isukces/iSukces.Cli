namespace iSukces.CliTools;

/// <summary>
/// Exception containing details from a failed CLI execution.
/// </summary>
public class CliException : Exception
{
    /// <summary>
    /// Initializes a new CLI exception.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Exception that caused this exception.</param>
    public CliException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Captured standard output text.
    /// </summary>
    public required string Output { get; init; }

    /// <summary>
    /// Captured standard error text.
    /// </summary>
    public required string Error  { get; init; }
}
