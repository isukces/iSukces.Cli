using iSukces.CliTools;

namespace iSukces.Cli.Python;

/// <summary>
/// Aggregated Python environment information related to PyTorch and CUDA.
/// </summary>
public sealed class PythonEnvironmentInfo
{
    /// <summary>
    /// PyTorch version reported by torch.__version__.
    /// </summary>
    public required Result<PyTorchVersion> TorchVersion { get; init; }

    
    /// <summary>
    /// CUDA availability reported by torch.cuda.is_available().
    /// </summary>
    public required Result<bool> IsTorchCudaAvailable { get; init; }

    
    /// <summary>
    /// CUDA version reported by torch.version.cuda.
    /// </summary>
    public required Result<TorchCudaVersion?> TorchCudaVersion { get; init; }
    
    /// <summary>
    /// CUDA Toolkit versions installed in the default NVIDIA Toolkit directory.
    /// </summary>
    public required Result<IReadOnlyList<CudaToolkitVersion>> InstalledVersions { get; init; }


    /// <summary>
    /// Complete environment information collected from the specified virtual environment.
    /// </summary>
    /// <param name="venv">Virtual environment used for PyTorch and CUDA inspection.</param>
    /// <returns>Task containing collected Python environment information.</returns>
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
    /// <summary>
    /// Detected environment problems for the collected PyTorch and CUDA information.
    /// </summary>
    /// <returns>Detected environment problems.</returns>
    public IReadOnlyList<PythonEnvironmentProblem> GetProblems()
    {
        var a = new PythonEnvironmentProblemFinder(this);
        return a.GetProblems();

    }
    
    /// <summary>
    /// Environment updater that activates the matching CUDA Toolkit installation.
    /// </summary>
    /// <returns>Environment updater for CUDA variables, or null when no matching Toolkit is available.</returns>
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
