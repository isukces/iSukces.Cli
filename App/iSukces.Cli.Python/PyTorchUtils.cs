using iSukces.CliTools;

namespace iSukces.Cli.Python;

/// <summary>
/// PyTorch and CUDA inspection utilities for Python virtual environments.
/// </summary>
public static class PyTorchUtils
{
    /// <summary>
    /// CUDA availability reported by torch.cuda.is_available().
    /// </summary>
    /// <param name="venv">Virtual environment used to run Python code.</param>
    /// <returns>Result containing CUDA availability, or an error description.</returns>
    public static async Task<Result<bool>> GetIsCudaAvailable(PythonVenv venv)
    {
        if (venv == null) throw new ArgumentNullException(nameof(venv));
        return await Result.FromCliCallEx(async () =>
            {
                var cli = await RunPytonCodeWithTorch("print(torch.cuda.is_available())", venv);
                return cli;
            }, cli =>
            {
                var output = cli.Output.Trim();
                return output switch
                {
                    "True" => true,
                    "False" => false,
                    _ => throw new Exception($"Unexpected output: {output}")
                };
            },
            _ => null);
    }

        
    /// <summary>
    /// CUDA version reported by torch.version.cuda.
    /// </summary>
    /// <param name="venv">Optional virtual environment used to run Python code.</param>
    /// <returns>Result containing the CUDA version reported by PyTorch, or an error description.</returns>
    public static async Task<Result<TorchCudaVersion?>> GetTorchCudaVersion(PythonVenv? venv = null)
    {
        var value = await Result.FromCliCallEx(async () =>
            {
                var cli = await RunPytonCodeWithTorch("print(torch.version.cuda)", venv);
                return cli;
            },
            cli =>
            {
                return TorchCudaVersion.TryParse(cli.Output);
            },
            cudaVersion =>
            {
                return cudaVersion is null 
                    ? "Nie można rozpoznać wersji cuda" 
                    : null;
            });
        return value;
    }

    /// <summary>
    /// PyTorch version reported by torch.__version__.
    /// </summary>
    /// <param name="venv">Virtual environment used to run Python code.</param>
    /// <returns>Result containing the PyTorch version, or an error description.</returns>
    public static async Task<Result<PyTorchVersion>> GetTorchVersion(PythonVenv venv)
    {
        if (venv == null) throw new ArgumentNullException(nameof(venv));
        var value = await Result.FromCliCallEx(async () =>
            {
                var cli = await RunPytonCodeWithTorch("print(torch.__version__)", venv);
                return cli;
            }, cli =>
            {
                var output = PyTorchVersion.TryParse(cli.Output);
                return output;
            }
            , a => a is null ? "Nie można rozpoznać wersji torcha" : null);
        return value;
    }
    /// <summary>
    /// Result of running Python code in the specified virtual environment.
    /// </summary>
    /// <param name="code">Python code passed to the interpreter.</param>
    /// <param name="venv">Virtual environment used to run the Python code.</param>
    /// <returns>CLI execution result from the Python process.</returns>
    public static async Task<CliResult> RunPythonCode(string code, PythonVenv venv)
    {
        if (venv == null) throw new ArgumentNullException(nameof(venv));
        var runner = new CliTools.Cli
            {
                FileName      = "python",
                Arguments     = ["-c", code],
                ThrowIfFailed = true
            }
            .WithVenvSettingsAndWorkingDirectory(venv);
        var r = await runner.RunAsync();
        return r;
    }

    private static Task<CliResult> RunPytonCodeWithTorch(string code, PythonVenv venv)
    {
        if (venv == null) throw new ArgumentNullException(nameof(venv));
        code = "import torch; " + code;
        return RunPythonCode(code, venv);
    }
 
    private const string Packages = "torch torchvision torchaudio";
    private const string IndexUrl = "https://download.pytorch.org/whl/cu121";
    /// <summary>
    /// Package operations required to install GPU-enabled PyTorch.
    /// </summary>
    /// <param name="useGpu">Value indicating whether GPU-enabled PyTorch packages are required.</param>
    /// <returns>Package operations required for the selected PyTorch installation mode.</returns>
    public static IEnumerable<InstallerOptions> GetInstallingElements(bool useGpu)
    {
        if (!useGpu)
            yield break;
        var pkg = Packages.Split(' ');
        yield return new InstallerOptions(false, pkg);
        yield return new InstallerOptions(true, pkg, IndexUrl);
    }
}
