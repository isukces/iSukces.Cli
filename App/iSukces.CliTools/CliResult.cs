namespace iSukces.CliTools;

/// <summary>
/// Result data captured after a CLI process finishes.
/// </summary>
public sealed class CliResult
{
    /// <summary>
    /// Creates an exception populated with this result data.
    /// </summary>
    /// <returns>An exception containing CLI failure details.</returns>
    public CliException CreateException()
    {
        var message = $"ExitCode: {ExitCode}{Environment.NewLine}" +
                      $"STDOUT:{Environment.NewLine}{Output}{Environment.NewLine}" +
                      $"STDERR:{Environment.NewLine}{Error}";
        throw new CliException(message)
        {
            Output = Output,
            Error  = Error
        };
    }

    /// <summary>
    /// Process exit code.
    /// </summary>
    public          int      ExitCode { get; init; }

    /// <summary>
    /// Captured standard output text.
    /// </summary>
    public required string   Output   { get; init; }

    /// <summary>
    /// Captured standard error text.
    /// </summary>
    public required string   Error    { get; init; }

    /// <summary>
    /// Process start time.
    /// </summary>
    public required DateTime Started  { get; init; }

    /// <summary>
    /// Process finish time.
    /// </summary>
    public required DateTime Finished { get; init; }

    /// <summary>
    /// Process execution duration.
    /// </summary>
    public          TimeSpan Duration => Finished - Started;

    /// <summary>
    /// Executable file name or path used for the process.
    /// </summary>
    public  required string FileName         { get; init; }

    /// <summary>
    /// Command-line arguments used for the process.
    /// </summary>
    public  required string Arguments        { get; init; }

    /// <summary>
    /// Working directory used for the process.
    /// </summary>
    public  required string WorkingDirectory { get; init; }
}
