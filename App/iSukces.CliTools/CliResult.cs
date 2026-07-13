namespace iSukces.CliTools;

public sealed class CliResult
{
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

    public          int      ExitCode { get; init; }
    public required string   Output   { get; init; }
    public required string   Error    { get; init; }
    public required DateTime Started  { get; init; }
    public required DateTime Finished { get; init; }
    public          TimeSpan Duration => Finished - Started;

    public  required string FileName         { get; init; }
    public  required string Arguments        { get; init; }
    public  required string WorkingDirectory { get; init; }
}