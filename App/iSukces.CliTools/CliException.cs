namespace iSukces.CliTools;

public class CliException : Exception
{
    public CliException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    public required string Output { get; init; }
    public required string Error  { get; init; }
}
