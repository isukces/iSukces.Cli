using iSukces.CliTools;

namespace iSukces.Cli.Python;

/// <summary>
/// ncvv
/// </summary>
public static class CudaToolkit
{
    /// <summary>
    /// Z katalogu c:\Program Files\NVIDIA GPU Computing Toolkit\CUDA
    /// </summary>
    /// <returns></returns>
    public static Result<IReadOnlyList<CudaToolkitVersion>?> GetInstalledVersions()
    {
        return Result.Make(MakeInternal);

        IReadOnlyList<CudaToolkitVersion> MakeInternal()
        {
            var dir = new DirectoryInfo(BaseDir);
            if (!dir.Exists)
                return [];
            return dir.GetDirectories()
                .Select(CudaToolkitVersion.TryParse)
                .Where(x => x is not null)
                .ToList()!;
        }
    }

    public static string BaseDir = @"c:\Program Files\NVIDIA GPU Computing Toolkit\CUDA";
}
