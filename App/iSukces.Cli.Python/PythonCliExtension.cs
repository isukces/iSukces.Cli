namespace iSukces.Cli.Python;

/// <summary>
/// Extension methods for configuring Python-related CLI process settings.
/// </summary>
public static class PythonCliExtension
{
    extension(CliTools.Cli cli)
    {
        /// <summary>
        /// Configures UTF-8 output encoding for Python child processes.
        /// </summary>
        /// <returns>Configured CLI instance.</returns>
        public CliTools.Cli WithPythonEncodingUtf8()
        {
            cli.EnvironmentVariables.SetVariable("PYTHONIOENCODING", "utf-8");
            return cli;
        }
        /// <summary>
        /// Configures virtual environment variables and the working directory for a CLI process.
        /// </summary>
        /// <param name="venv">Virtual environment used to configure the CLI process.</param>
        /// <returns>Configured CLI instance.</returns>
        public CliTools.Cli WithVenvSettingsAndWorkingDirectory(PythonVenv venv)
        {
            cli.WithVenvSettings(venv);
            cli.WorkingDirectory = venv.WorkingDirectory;
            return cli;
        }
        /// <summary>
        /// Configures virtual environment variables for a CLI process.
        /// </summary>
        /// <param name="venv">Virtual environment used to configure the CLI process.</param>
        /// <returns>Configured CLI instance.</returns>
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
