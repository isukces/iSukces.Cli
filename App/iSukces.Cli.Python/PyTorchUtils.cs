using iSukces.CliTools;

namespace iSukces.Cli.Python;

public static class PyTorchUtils
{
    /// <summary>
    /// Na podstawie torch.cuda.is_available()
    /// </summary>
    /// <param name="venv"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
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
    /// Na podstawie torch.version.cuda
    /// </summary>
    /// <param name="venv"></param>
    /// <returns></returns>
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
    /// Na podstawie torch.__version__
    /// </summary>
    /// <param name="venv"></param>
    /// <returns></returns>
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

    public static IEnumerable<InstallerOptions> GetInstallingElements(bool useGpu)
    {
        if (!useGpu)
            yield break;
        var pkg = Packages.Split(' ');
        yield return new InstallerOptions(false, pkg);
        yield return new InstallerOptions(true, pkg, IndexUrl);
    }
}
