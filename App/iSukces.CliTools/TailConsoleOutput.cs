namespace iSukces.CliTools;

/// <summary>
/// Tail-mode console output window with framed scrolling text.
/// </summary>
public sealed class TailConsoleOutput : IDisposable
{
    /// <summary>
    /// Initializes tail-mode console output with the requested number of visible lines.
    /// </summary>
    /// <param name="visibleLines">Number of output lines visible in the tail window.</param>
    public TailConsoleOutput(int visibleLines = 5)
    {
        _visibleLines = Math.Max(1, visibleLines);
        _lines        = new string[_visibleLines];

        // Make room for the box: top border + content lines + bottom border
        var boxHeight = _visibleLines + 2;
        for (var i = 0; i < boxHeight; i++)
            Console.WriteLine();

        if (TryGetCursorPosition(out var top))
        {
            _topLine           = top - boxHeight;
            _useCursorPosition = _topLine >= 0;
        }
        else
        {
            _useCursorPosition = false;
        }

        // Listen for console resize events
        try
        {
            Console.CancelKeyPress += OnCancelKeyPress;
        }
        catch
        {
            // ignore
        }

        Redraw();
    }

    /// <summary>
    /// Writes one line to the tail output window.
    /// </summary>
    /// <param name="text">Line of text to display.</param>
    public void Write(string text)
    {
        text = text.Replace("\r", "").Replace("\n", "");

        // Shift lines up, add new line at bottom
        for (var i = 0; i < _visibleLines - 1; i++)
            _lines[i] = _lines[i + 1];
        _lines[_visibleLines - 1] = text;

        Redraw();
    }

    /// <summary>
    /// Releases the tail output window and restores the console cursor position.
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
        try
        {
            Console.CancelKeyPress -= OnCancelKeyPress;
        }
        catch
        {
            // ignore
        }

        if (_useCursorPosition)
        {
            var bottomLine = _topLine + _visibleLines + 2;
            try
            {
                if (bottomLine < Console.BufferHeight)
                    Console.SetCursorPosition(0, bottomLine);
            }
            catch
            {
                // ignore — terminal may not support cursor positioning
            }
        }

        Console.WriteLine();
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        // On Ctrl+C, move cursor below the box before the process exits
        if (_useCursorPosition && !_disposed)
        {
            try
            {
                var bottomLine = _topLine + _visibleLines + 2;
                if (bottomLine < Console.BufferHeight)
                    Console.SetCursorPosition(0, bottomLine);
            }
            catch
            {
                // ignore
            }
        }
    }

    private void Redraw()
    {
        if (!_useCursorPosition)
        {
            // Fallback: single-line \r mode (no frame)
            var text   = _lines[_visibleLines - 1] ?? "";
            var maxLen = Math.Max(1, Console.BufferWidth - 1);
            if (text.Length > maxLen)
                text = text.Substring(0, maxLen);
            if (text.Length < _lastLength)
                text = text.PadRight(_lastLength);
            Console.Write("\r" + text);
            _lastLength = text.Length;
            return;
        }

        // Clamp box position to current buffer dimensions.
        // If the buffer shrank below our box, we re-anchor at the bottom.
        var boxHeight = _visibleLines + 2;
        var bufferH   = Console.BufferHeight;
        var bufferW   = Console.BufferWidth;

        if (_topLine + boxHeight > bufferH)
        {
            // Re-anchor: place box so its bottom border is the last buffer line
            _topLine = Math.Max(0, bufferH - boxHeight);
        }

        var boxWidth   = Math.Max(3, bufferW - 1);
        var innerWidth = Math.Max(1, boxWidth - 2);

        try
        {
            // Top border
            Console.SetCursorPosition(0, _topLine);
            Console.Write("┌" + new string('─', innerWidth) + "┐");

            // Content lines
            for (var i = 0; i < _visibleLines; i++)
            {
                var line = _lines[i] ?? "";
                if (line.Length > innerWidth)
                    line = line.Substring(0, innerWidth);
                else
                    line = line.PadRight(innerWidth);

                Console.SetCursorPosition(0, _topLine + 1 + i);
                Console.Write("│" + line + "│");
            }

            // Bottom border
            Console.SetCursorPosition(0, _topLine + 1 + _visibleLines);
            Console.Write("└" + new string('─', innerWidth) + "┘");
        }
        catch
        {
            // If cursor positioning fails (e.g. terminal doesn't support it),
            // fall back to single-line mode from now on.
            _useCursorPosition = false;
            Redraw();
        }
    }

    private static bool TryGetCursorPosition(out int top)
    {
        try
        {
            top = Console.GetCursorPosition().Top;
            return true;
        }
        catch
        {
            top = -1;
            return false;
        }
    }

    private          int      _topLine;
    private          bool     _useCursorPosition;
    private          int      _lastLength;
    private          bool     _disposed;
    private readonly int      _visibleLines;
    private readonly string[] _lines;
}
