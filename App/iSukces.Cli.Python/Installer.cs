using System.Globalization;
using iSukces.CliTools;

namespace iSukces.Cli.Python;

/// <summary>
/// Pip package installation or uninstallation options.
/// </summary>
/// <param name="Add">Package operation mode indicating installation when true and uninstallation when false.</param>
/// <param name="Packages">Package names for the pip operation.</param>
/// <param name="IndexUrl">Optional package index URL for installation.</param>
public record InstallerOptions(bool Add, string[] Packages, string? IndexUrl = null);

/// <summary>
/// Python virtual environment installer and package setup coordinator.
/// </summary>
public sealed class Installer
{
    /// <summary>
    /// Environment updater that supplies a HuggingFace access token to child processes.
    /// </summary>
    /// <param name="huggingFaceToken">HuggingFace access token to expose as an environment variable.</param>
    /// <returns>Environment updater with the token variable, or null when the token is empty.</returns>
    public static EnvironmentUpdater? GetHuggingFaceTokenUpdater(string? huggingFaceToken)
    {
        if (string.IsNullOrEmpty(huggingFaceToken))
            return null;
        return EnvironmentUpdater.Make(HuggingFace.EnviromentVariable, huggingFaceToken);
    }

    private static TouchedFile GetTouchFile(PythonVenv venv, string name)
    {
        var touchFile = Path.Combine(venv.VenvFolderFullName, ".prv-flags", name + ".txt");
        return new TouchedFile(touchFile);
    }

    private static async Task UpgradePip(PythonVenv venv)
    {
        var runner = new CliTools.Cli
            {
                FileName      = "py",
                Arguments     = ["-m", "pip", "install", "--upgrade", "pip"],
                ThrowIfFailed = true
            }
            .WithVenvSettingsAndWorkingDirectory(venv);
        await runner.RunAsync();
    }

    /// <summary>
    /// Installs the configured virtual environment and pip packages.
    /// </summary>
    /// <returns>Task representing the asynchronous installation operation.</returns>
    public async Task Install()
    {
        var venv     = ToVenv();
        var touchAll = GetTouchFile(venv, "install-all");
        if (touchAll.Exists) return;

        var vers = await PyUtils.GetInstalledVersions();
        if (!vers.Any(a => a.Compatible(venv.Version)))
            throw new Exception($"Python {venv.Version} nie jest zainstalowany.");

        // using var hfUpdate = GetHuggingFaceTokenUpdater(HuggingFaceToken)?.Activate();
        
        venv.Append(GetHuggingFaceTokenUpdater(HuggingFaceToken));

        var touchVenv = GetTouchFile(venv, "install-venv");
        if (!touchVenv.Exists)
        {
            await venv.Install();
            touchVenv.Touch();
        }

        // using var disposable = venv.GetEnvironmentUpdater()?.Activate();

        await UpgradePip(venv);
        foreach (var package in Packages)
        {
            var a = new List<string>();
            if (package.Add)
            {
                a.Add("install");
                a.AddRange(package.Packages);
                if (!string.IsNullOrEmpty(package.IndexUrl))
                {
                    a.Add("--index-url");
                    a.Add(package.IndexUrl);
                }
            }
            else
            {
                a.Add("uninstall");
                a.AddRange(package.Packages);
                a.Add("-y");
            }

            var runner = new CliTools.Cli
            {
                FileName  = "pip",
                Arguments = a.ToArray()
            }.WithPythonEncodingUtf8();
            var q = await runner.RunAsync();
            if (q.ExitCode != 0)
                throw q.CreateException();
        }

        touchAll.Touch();
    }

    /// <summary>
    /// Virtual environment definition for the installer configuration.
    /// </summary>
    /// <returns>Virtual environment configured from the installer settings.</returns>
    public PythonVenv ToVenv()
    {
        return new PythonVenv
        {
            WorkingDirectory = Folder,
            Version          = Version
        };
    }

    /// <summary>
    /// Working directory for the virtual environment.
    /// </summary>
    public required string Folder { get; init; }

    /// <summary>
    /// Python version used to create the virtual environment.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Pip package operations executed by the installer.
    /// </summary>
    public required InstallerOptions[] Packages { get; init; }

    /// <summary>
    /// Optional HuggingFace access token exposed to installation processes.
    /// </summary>
    public string? HuggingFaceToken { get; init; }

    private record TouchedFile(string Path)
    {
        public void Touch()
        {
            var fi = new FileInfo(Path);
            fi.Directory?.Create();
            File.WriteAllTextAsync(Path, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        public bool Exists => File.Exists(Path);
    }
}
