namespace iSukces.CliTools;

public static class Result
{
    public static Result<T?> Make<T>(Func<T> func)
    {
        return Result<T>.Make(func);
    }

    public static Task<Result<T>> MakeAsync<T>(Func<Task<T>> func)
    {
        return Result<T>.MakeAsync(func);
    }

    public static async Task<Result<T?>> FromCliCallEx<T>(
        Func<Task<CliResult>> callCli, 
        Func<CliResult, T> convert,
        Func<T, string?> getError)
    {
        try
        {
            var cliResult = await callCli();
            try
            {
                var value  = convert(cliResult);
                var error  = getError(value);
                return new Result<T?>
                {
                    Value     = value,
                    IsSuccess = string.IsNullOrEmpty(error),
                    CliResult = cliResult
                };
            }
            catch (Exception e)
            {
                return Result<T>.FromException(e, cliResult);
            }
        }
        catch (Exception e)
        {
            return Result<T?>.FromException(e);
        }
    }
}

public sealed class Result<T>
{
    public static Result<T?> FromException(Exception exception, CliResult? cliResult = null)
    {
        var result = new Result<T?>
        {
            IsSuccess = false,
            Exception = exception,
            Value     = default,
            CliResult = cliResult
        };
        return result;
    }

    public static Result<T?> FromValue(T? value, CliResult? cliResult = null)
    {
        return new Result<T?>
        {
            Value     = value,
            IsSuccess = true,
            CliResult = cliResult
        };
    }

    public static Result<T?> Make(Func<T> func)
    {
        try
        {
            var value = func();
            return FromValue(value);
        }
        catch (Exception e)
        {
            return FromException(e);
        }
    }

    public static async Task<Result<T?>> MakeAsync(Func<Task<T?>> func)
    {
        try
        {
            var value  = await func();
            var result = FromValue(value);
            return result;
        }
        catch (Exception e)
        {
            var result = FromException(e);
            return result;
        }
    }

    public static async Task<Result<T>> MakeAsync2(Func<Task<T>> func)
    {
        try
        {
            var value = await func();
            return FromValue(value);
        }
        catch (Exception e)
        {
            return FromException(e);
        }
    }

    #region Properties

    public Exception? Exception { get; init; }

    public required bool IsSuccess { get; init; }

    public required T Value { get; init; }

    public CliResult? CliResult { get; init; }

    #endregion

    public bool TryGetValue(out T value)
    {
        value = Value;
        return IsFullyProperValue;
    }
    
    public bool IsFullyProperValue => IsSuccess
                                       && Exception is null
                                       && string.IsNullOrEmpty(CliResult?.Error);
}
