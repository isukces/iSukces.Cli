using iSukces.CliTools;

namespace iSukces.Cli.Python;

public sealed class PythonEnvironmentInfo
{
    /// <summary>
    /// Na podstawie torch.__version__
    /// </summary>
    public required Result<PyTorchVersion> TorchVersion { get; init; }

    
    /// <summary>
    /// Na podstawie torch.cuda.is_available()
    /// </summary>
    public required Result<bool> IsTorchCudaAvailable { get; init; }

    
    /// <summary>
    /// Na podstawie torch.version.cuda
    /// </summary>
    public required Result<TorchCudaVersion?> TorchCudaVersion { get; init; }
    
    /// <summary>
    /// Z katalogu c:\Program Files\NVIDIA GPU Computing Toolkit\CUDA
    /// </summary>
    public required Result<IReadOnlyList<CudaToolkitVersion>> InstalledVersions { get; init; }


    public static async Task<PythonEnvironmentInfo> Make(PythonVenv venv)
    {
        if (venv == null) throw new ArgumentNullException(nameof(venv));
        var torchVersionTask         = PyTorchUtils.GetTorchVersion(venv);
        var isTorchCudaAvailableTask = PyTorchUtils.GetIsCudaAvailable(venv);
        var torchCudaVersionTask     = PyTorchUtils.GetTorchCudaVersion(venv);

        await Task.WhenAll(torchVersionTask, isTorchCudaAvailableTask, torchCudaVersionTask);
        
        var torchVersion         = await torchVersionTask;
        var isTorchCudaAvailable = await isTorchCudaAvailableTask;
        var torchCudaVersion     = await torchCudaVersionTask;
        var versions             = CudaToolkit.GetInstalledVersions();

        var v = new PythonEnvironmentInfo
        {
            TorchVersion         = torchVersion,
            IsTorchCudaAvailable = isTorchCudaAvailable,
            TorchCudaVersion     = torchCudaVersion,
            InstalledVersions    = versions
        };
        return v;
    }

    public IReadOnlyList<PythonEnvironmentProblem> GetProblems()
    {
        var a = new PythonEnvironmentProblemFinder(this);
        return a.GetProblems();

    }
    
    public EnvironmentUpdater? GetEnvironmentUpdater()
    {
        if (!IsTorchCudaAvailable.TryGetValue(out var isTorchCudaAvailable)) return null;
        if (!isTorchCudaAvailable) return null;
        if (!TorchCudaVersion.TryGetValue(out var torchCudaVersion)) return null;
        if (torchCudaVersion is null) return null;
        if (!InstalledVersions.TryGetValue(out var installedVersions)) return null;
        installedVersions ??= [];
        var toolkit   = installedVersions.FirstOrDefault(a => a.Value == torchCudaVersion.Value);
        var dir = toolkit?.ToolkitDirectory.FullName;
        if (string.IsNullOrEmpty(dir)) 
            return null;

        var updater = new EnvironmentUpdater
        {
            SetVariables =
            {
                ["CUDA_PATH"] = dir
            },
            AddToPathAtBeginning =
            {
                Path.Combine(dir, "bin", "x64"),
                dir
            }
        };
        return updater;

    }
}