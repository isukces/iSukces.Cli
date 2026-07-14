using iSukces.CliTools;

namespace iSukces.Cli.Python;

/// <summary>
/// CUDA Toolkit installation discovery helper.
/// </summary>
public static class CudaToolkit
{
    /// <summary>
    /// Installed CUDA Toolkit versions from the default NVIDIA Toolkit directory.
    /// </summary>
    /// <returns>Result containing installed CUDA Toolkit versions, or an error description.</returns>
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

    /// <summary>
    /// Default NVIDIA CUDA Toolkit installation directory.
    /// </summary>
    public static string BaseDir = @"c:\Program Files\NVIDIA GPU Computing Toolkit\CUDA";
}
