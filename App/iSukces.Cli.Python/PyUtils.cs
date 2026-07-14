namespace iSukces.Cli.Python;

/// <summary>
/// Python launcher utility methods.
/// </summary>
public static class PyUtils
{
    /// <summary>
    /// Python interpreter versions installed for the Windows Python launcher.
    /// </summary>
    /// <returns>Task containing installed Python interpreter versions.</returns>
    public static async Task<IReadOnlyList<PythonVersionInfo>>
        GetInstalledVersions()
    {
        var runner = new CliTools.Cli
        {
            FileName      = "py",
            Arguments     = ["-0"],
            ThrowIfFailed = true
        }.WithPythonEncodingUtf8();
        var result = await runner.RunAsync();
        var lines  = result.Output.Replace("\r\n", "\n").Split('\n');
        var infos  = PyLauncherParser.ParseMany(lines).ToArray();
        return infos;
    }
}
