namespace iSukces.Cli.Python;

public static  class PythonCliExtension
{
    extension(CliTools.Cli cli)
    {
        public CliTools.Cli WithPythonEncodingUtf8()
        {
            cli.EnvironmentVariables.SetVariable("PYTHONIOENCODING", "utf-8");
            return cli;
        }

        public CliTools.Cli WithVenvSettingsAndWorkingDirectory(PythonVenv venv)
        {
            cli.WithVenvSettings(venv);
            cli.WorkingDirectory = venv.WorkingDirectory;
            return cli;
        }

        public CliTools.Cli WithVenvSettings(PythonVenv venv)
        {
            ArgumentNullException.ThrowIfNull(venv);
            cli.WithPythonEncodingUtf8();
            var updater = venv.GetEnvironmentUpdater();
            cli.EnvironmentVariables.Append(updater);
            return cli;
        }
    }
}
