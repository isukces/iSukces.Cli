namespace iSukces.Cli.Python;

public static class PyUtils
{
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