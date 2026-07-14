namespace iSukces.CliTools;

internal sealed class ActionDisposable : IDisposable
{
    private Action? _disposeAction;

    /// <summary>
    /// Initializes a disposable wrapper for the specified action.
    /// </summary>
    /// <param name="disposeAction">Action executed when the wrapper is disposed.</param>
    public ActionDisposable(Action disposeAction)
    {
        _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof (disposeAction));
    }

    /// <summary>
    /// Executes the wrapped action once.
    /// </summary>
    public void Dispose()
    {
        var action = Interlocked.Exchange<Action>(ref _disposeAction, null);
        if (action is null)
            return;
        action();
    }

    /// <summary>
    /// Disposable wrapper for the specified action.
    /// </summary>
    /// <param name="action">Action executed when the wrapper is disposed.</param>
    /// <returns>A disposable wrapper that executes the action once.</returns>
    public static IDisposable Make(Action action) => new ActionDisposable(action);
}
