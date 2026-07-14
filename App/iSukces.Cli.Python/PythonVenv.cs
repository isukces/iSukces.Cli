using iSukces.CliTools;

namespace iSukces.Cli.Python;


/// <summary>
/// Registry for named Python virtual environments.
/// </summary>
public interface IVenvCollectionRegistry
{
    /// <summary>
    /// Adds or replaces a named Python virtual environment.
    /// </summary>
    /// <param name="key">Environment key.</param>
    /// <param name="venv">Python virtual environment registered under the key.</param>
    void Add(string key, PythonVenv venv);
}

/// <summary>
/// Resolver for named Python virtual environments.
/// </summary>
public interface IVenvCollectionResolver
{
    /// <summary>
    /// Named Python virtual environment.
    /// </summary>
    /// <param name="key">Environment key.</param>
    /// <returns>Python virtual environment registered under the key.</returns>
    PythonVenv Get(string key);
}

/// <summary>
/// In-memory collection of named Python virtual environments.
/// </summary>
public sealed class VenvCollection : IVenvCollectionRegistry, IVenvCollectionResolver
{
    private readonly Dictionary<string, PythonVenv> _venvs = new();

    /// <summary>
    /// Adds or replaces a named Python virtual environment.
    /// </summary>
    /// <param name="key">Environment key.</param>
    /// <param name="venv">Python virtual environment registered under the key.</param>
    public void Add(string key, PythonVenv venv)
    {
        _venvs[key] = venv;
    }
    /// <summary>
    /// Named Python virtual environment.
    /// </summary>
    /// <param name="key">Environment key.</param>
    /// <returns>Python virtual environment registered under the key.</returns>
    public PythonVenv Get(string key)
    {
        return _venvs[key];
    }
}


/// <summary>
/// Python virtual environment definition and activation helper.
/// </summary>
public sealed class PythonVenv
{
    private void Delete()
    {
        var dir = new DirectoryInfo(VenvFolderFullName);
        if (dir.Exists)
            dir.Delete(true);
    }
    /// <summary>
    /// Additional environment settings applied during virtual environment activation.
    /// </summary>
    public EnvironmentUpdater ExtraSettings { get; } = new();

    /// <summary>
    /// Environment updater that activates the virtual environment for child processes.
    /// </summary>
    /// <returns>Environment updater configured for virtual environment activation.</returns>
    public EnvironmentUpdater GetEnvironmentUpdater()
    {
        const string virtualEnvKey = "VIRTUAL_ENV";
        const string pythonhomeKey = "PYTHONHOME";


        var updater = new EnvironmentUpdater
        {
            SetVariables =
            {
                { virtualEnvKey, VenvFolderFullName },
                { pythonhomeKey, "" },
            },
            AddToPathAtBeginning =
            {
                Path.Combine(VenvFolderFullName, "Scripts")
            }
        };
        updater.Append(ExtraSettings);
        return updater;
    }
    /// <summary>
    /// Creates the virtual environment using the configured Python version.
    /// </summary>
    /// <param name="useUv">Value indicating whether the uv tool should create the virtual environment.</param>
    /// <returns>Task representing the asynchronous installation operation.</returns>
    public async Task Install(bool useUv = false)
    {
        new DirectoryInfo(WorkingDirectory).Create();
        CliTools.Cli runner;
        if (useUv)
        {
            // uv python install 3.10.20
            runner = new CliTools.Cli
            {
                FileName         = "uv",
                WorkingDirectory = WorkingDirectory,
                Arguments        = ["python", "install", Version],
                OutputMode       = CliOutputMode.All,
                ThrowIfFailed    = true
            }.WithPythonEncodingUtf8();
            await runner.RunAsync();

            // uv venv .venv --python 3.10.20
            runner = new CliTools.Cli
            {
                FileName         = "uv",
                WorkingDirectory = WorkingDirectory,
                Arguments        = ["venv", VenvFolder, "--python", Version]
            }.WithPythonEncodingUtf8();
        }
        else
            runner = new CliTools.Cli
            {
                FileName         = "py",
                WorkingDirectory = WorkingDirectory,
                Arguments        = ["-" + Version, "-m", "venv", VenvFolder]
            }.WithPythonEncodingUtf8();

        Delete();
        var proc = await runner.RunAsync();

        if (proc.ExitCode == 0)
            return;

        try
        {
            Delete();
        }
        catch
        {
        }

        throw proc.CreateException();
    }

    /// <summary>
    /// Working directory containing the virtual environment folder.
    /// </summary>
    public required string WorkingDirectory { get; init; }

    /// <summary>
    /// Python version used for virtual environment creation.
    /// </summary>
    public required string Version { get; init; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    /// <summary>
    /// Virtual environment folder name relative to the working directory.
    /// </summary>
    public string VenvFolder { get; init; } = ".venv";

    /// <summary>
    /// Full path to the virtual environment folder.
    /// </summary>
    public string VenvFolderFullName => Path.Combine(WorkingDirectory, VenvFolder);

    /// <summary>
    /// Appends additional environment settings for virtual environment activation.
    /// </summary>
    /// <param name="u">Environment settings to append.</param>
    public void Append(EnvironmentUpdater? u)
    {
        ExtraSettings.Append(u);
    }

   
}

