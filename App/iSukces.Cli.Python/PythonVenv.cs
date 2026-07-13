using iSukces.CliTools;

namespace iSukces.Cli.Python;


public interface IVenvCollectionRegistry
{
    void Add(string key, PythonVenv venv);
}
public interface IVenvCollectionResolver
{
    PythonVenv Get(string key);
}

public sealed class VenvCollection: IVenvCollectionRegistry,IVenvCollectionResolver
{
    private readonly Dictionary<string, PythonVenv> _venvs = new();

    public void Add(string key, PythonVenv venv)
    {
        _venvs[key] = venv;
    }

    public PythonVenv Get(string key)
    {
        return _venvs[key];
    }
}



public sealed class PythonVenv
{
    private void Delete()
    {
        var dir = new DirectoryInfo(VenvFolderFullName);
        if (dir.Exists)
            dir.Delete(true);
    }

    public EnvironmentUpdater ExtraSettings { get; } = new();

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

    public required string WorkingDirectory { get; init; }
    public required string Version          { get; init; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string VenvFolder { get; init; } = ".venv";

    public string VenvFolderFullName => Path.Combine(WorkingDirectory, VenvFolder);

    public void Append(EnvironmentUpdater? u)
    {
        ExtraSettings.Append(u);
    }

   
}

