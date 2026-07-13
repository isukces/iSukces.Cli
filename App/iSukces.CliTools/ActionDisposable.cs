namespace iSukces.CliTools;

internal sealed class ActionDisposable : IDisposable
{
    private Action? _disposeAction;

    public ActionDisposable(Action disposeAction)
    {
        _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof (disposeAction));
    }

    public void Dispose()
    {
        var action = Interlocked.Exchange<Action>(ref _disposeAction, null);
        if (action is null)
            return;
        action();
    }

    public static IDisposable Make(Action action) => new ActionDisposable(action);
}
