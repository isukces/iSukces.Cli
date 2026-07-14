namespace iSukces.Cli.Python;

/// <summary>
/// Diagnostic description of a Python environment problem.
/// </summary>
public sealed class PythonEnvironmentProblem
{
    /// <summary>
    /// Creates an empty environment problem instance.
    /// </summary>
    public PythonEnvironmentProblem()
    {
    }

    /// <summary>
    /// Short diagnostic message.
    /// </summary>
    public required string? Message { get; init; }

    /// <summary>
    /// Detailed diagnostic description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Environment areas affected by the problem.
    /// </summary>
    public ProblemAffected Affected { get; init; }

    /// <summary>
    /// Suggested corrective action.
    /// </summary>
    public string? Remedy { get; init; }

    /// <summary>
    /// Writes the diagnostic message, description, and remedy to the console.
    /// </summary>
    public void WriteToConsole()
    {
        Console.WriteLine(Message + ", Affected: " + Affected);
        var lines = (Description??"")
            .Replace("\r\n", "\n").Split('\n')
            .Where(a=>!string.IsNullOrWhiteSpace(a))
            .Select(a=>$" {a}")
            .ToArray();
        Print("Description:", Description);
        Print("Remedy:", Remedy);
        return;

        void Print(string title, string? value)
        {
            lines = (value ?? "")
                .Replace("\r\n", "\n").Split('\n')
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => $" {a}")
                .ToArray();
            if (lines.Length == 0) return;
            Console.WriteLine(title + ":");
            foreach (var line in lines)
                Console.WriteLine(line);
        }
    }
}
