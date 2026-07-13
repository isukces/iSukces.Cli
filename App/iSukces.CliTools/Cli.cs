using System.Diagnostics;
using System.Text;

namespace iSukces.CliTools;

public class Cli
{
    public async Task<CliResult> RunAsync()
    {
        var date      = DateTime.Now;
        var arguments = GetArguments();
        var pi = new ProcessStartInfo(FileName)
        {
            WorkingDirectory       = WorkingDirectory,
            Arguments              = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            RedirectStandardInput  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
        };
        foreach (var (key, value) in EnvironmentVariables.GetEnvironmentVariables())
        {
            if (string.Equals(key, "Path", StringComparison.OrdinalIgnoreCase))
                pi.Environment["PATH"] = value;
            else
                pi.Environment[key] = value;
        }

        {
            Console.Write("Running ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(FileName);
            Console.ResetColor();
            Console.WriteLine($" {arguments}");
        }

        using var proc = new Process();
        proc.StartInfo           = pi;
        proc.EnableRaisingEvents = true;

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        using var tail = OutputMode == CliOutputMode.Tail ? new TailConsoleOutput() : null;

        proc.OutputDataReceived += (_, e) =>
        {
            if (e.Data == null) return;
            stdout.AppendLine(e.Data);
            if ((Redirect & RedirctToConsole.Output) != 0)
                WriteToConsole(e.Data);
        };

        proc.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                stderr.AppendLine(e.Data);
            if ((Redirect & RedirctToConsole.Error) != 0)
                WriteToConsole(e.Data);
        };

        if (!proc.Start())
            throw new Exception("Failed to start process");

        proc.StandardInput.Close();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        await proc.WaitForExitAsync();
        // ReSharper disable once MethodHasAsyncOverload
        proc.WaitForExit();

        var result = new CliResult
        {
            ExitCode         = proc.ExitCode,
            Output           = stdout.ToString(),
            Error            = stderr.ToString(),
            Started          = date,
            Finished         = DateTime.Now,
            Arguments        = arguments,
            FileName         = FileName,
            WorkingDirectory = WorkingDirectory ?? ""
        };
        if (result.ExitCode != 0 && ThrowIfFailed)
            throw result.CreateException();
        return result;

        void WriteToConsole(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            switch (OutputMode)
            {
                case CliOutputMode.All:
                    Console.WriteLine(text); break;
                case CliOutputMode.Tail:
                    var lines = text.TrimEnd()
                        .Replace("\r\n", "\n")
                        .Split('\n');
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                            tail!.Write(line);
                    }

                    break;
            }
        }

        string GetArguments()
        {
            if (Arguments is null || Arguments.Length == 0)
                return "";
            var args = Arguments
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(WindowsConsoleUtils.EscapeCommandLineParameterIfNecessary);
            var joined = string.Join(" ", args);
            return joined;
        }
    }

    #region Properties

    public required string    FileName  { get; set; }
    public          string[]? Arguments { get; set; }

    public string? WorkingDirectory { get; set; } = "";
    public bool    ThrowIfFailed    { get; set; }

    public EnvironmentUpdater EnvironmentVariables { get; }      = new();
    public CliOutputMode      OutputMode           { get; set; } = CliOutputMode.All;
    public RedirctToConsole   Redirect             { get; set; } = RedirctToConsole.Both;

    #endregion


    public Cli ExpandFileName(string? exeShortFilename = null)
    {
        if (string.IsNullOrEmpty(exeShortFilename))
            exeShortFilename = FileName;
        if (EnvironmentVariables.SearchFile(exeShortFilename, true, out var fullPath))
        {
            FileName = fullPath;
        }

        return this;
    }
}

public enum CliOutputMode
{
    None,
    All,
    Tail
}

[Flags]
public enum RedirctToConsole
{
    None,
    Output = 1,
    Error = 2,
    Both = Output | Error
}
