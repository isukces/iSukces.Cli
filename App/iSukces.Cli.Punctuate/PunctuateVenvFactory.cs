using System.Globalization;
using iSukces.CliTools;
using iSukces.Cli.Python;

namespace iSukces.Cli.Punctuate;

public static class PunctuateVenvFactory
{
    private static async Task ConditionalTask(string flagFile, Func<Task> task)
    {
        if (File.Exists(flagFile)) return;
        await task().ConfigureAwait(false);
        new FileInfo(flagFile).Directory?.Create();
        await File.WriteAllTextAsync(flagFile, DateTime.Now.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
    }

    public static async Task<PythonVenv> CreateVenv(string workingDirectory, Action<PythonVenv>? configureVenv = null)
    {
        var venv = new PythonVenv
        {
            Version          = "3.10.20",
            WorkingDirectory = workingDirectory,
        };
        var flagFile = Path.Combine(venv.VenvFolderFullName, "flags", "venv-installed.txt");
        await ConditionalTask(flagFile, async () =>
        {
            await venv.Install(true);
        });

        flagFile = Path.Combine(venv.VenvFolderFullName, "flags", "transformers-installed.txt");
        await ConditionalTask(flagFile, async () =>
        {
            //var       updater = venv.GetEnvironmentUpdater();
            var cli = new CliTools.Cli
                {
                    FileName         = "uv.exe",
                    Arguments        = ["pip", "install", "torch", "transformers", "sentencepiece"],
                    OutputMode       = CliOutputMode.Tail,
                    Redirect         = RedirctToConsole.Both,
                    ThrowIfFailed    = true,
                    WorkingDirectory = workingDirectory
                }
                .WithVenvSettings(venv);
            var result = await cli.RunAsync();
        });
        configureVenv?.Invoke(venv);
        return venv;
    }
}
