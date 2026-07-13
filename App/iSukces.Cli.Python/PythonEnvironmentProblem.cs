namespace iSukces.Cli.Python;

public sealed class PythonEnvironmentProblem
{
    public PythonEnvironmentProblem()
    {
    }

    public required string?         Message     { get; init; }
    public          string?         Description { get; init; }
    public          ProblemAffected Affected    { get; init; }
    public          string?         Remedy      { get; init; }

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
