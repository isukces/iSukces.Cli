namespace iSukces.CliTools;

/// <summary>
/// Factory methods for operation result wrappers.
/// </summary>
public static class Result
{
    /// <summary>
    /// Wraps a synchronous operation in a result object.
    /// </summary>
    /// <param name="func">Synchronous operation to execute.</param>
    /// <typeparam name="T">Type of the operation value.</typeparam>
    /// <returns>A result object containing the operation value or exception.</returns>
    public static Result<T?> Make<T>(Func<T> func)
    {
        return Result<T>.Make(func);
    }

    /// <summary>
    /// Wraps an asynchronous operation in a result object.
    /// </summary>
    /// <param name="func">Asynchronous operation to execute.</param>
    /// <typeparam name="T">Type of the operation value.</typeparam>
    /// <returns>A task that produces a result object containing the operation value or exception.</returns>
    public static Task<Result<T>> MakeAsync<T>(Func<Task<T>> func)
    {
        return Result<T>.MakeAsync(func);
    }

    /// <summary>
    /// Wraps a CLI call and converts its result to a typed operation result.
    /// </summary>
    /// <param name="callCli">Asynchronous CLI call to execute.</param>
    /// <param name="convert">Converter from CLI result to a typed value.</param>
    /// <param name="getError">Function that extracts an error message from the converted value.</param>
    /// <typeparam name="T">Type of the converted value.</typeparam>
    /// <returns>A task that produces a typed result object with CLI details.</returns>
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

/// <summary>
/// Typed operation result with value, success state, exception, and optional CLI details.
/// </summary>
/// <typeparam name="T">Type of the stored operation value.</typeparam>
public sealed class Result<T>
{
    /// <summary>
    /// Failed operation result created from an exception.
    /// </summary>
    /// <param name="exception">Exception captured from the operation.</param>
    /// <param name="cliResult">Optional CLI result associated with the failure.</param>
    /// <returns>A failed result object containing the exception and optional CLI details.</returns>
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

    /// <summary>
    /// Successful operation result created from a value.
    /// </summary>
    /// <param name="value">Value produced by the operation.</param>
    /// <param name="cliResult">Optional CLI result associated with the operation.</param>
    /// <returns>A successful result object containing the value and optional CLI details.</returns>
    public static Result<T?> FromValue(T? value, CliResult? cliResult = null)
    {
        return new Result<T?>
        {
            Value     = value,
            IsSuccess = true,
            CliResult = cliResult
        };
    }

    /// <summary>
    /// Wraps a synchronous operation in a result object.
    /// </summary>
    /// <param name="func">Synchronous operation to execute.</param>
    /// <returns>A result object containing the operation value or exception.</returns>
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

    /// <summary>
    /// Wraps an asynchronous operation that may produce a nullable value.
    /// </summary>
    /// <param name="func">Asynchronous operation to execute.</param>
    /// <returns>A task that produces a result object containing the operation value or exception.</returns>
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

    /// <summary>
    /// Wraps an asynchronous operation that produces a non-null typed value.
    /// </summary>
    /// <param name="func">Asynchronous operation to execute.</param>
    /// <returns>A task that produces a result object containing the operation value or exception.</returns>
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

    /// <summary>
    /// Exception captured during the operation.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Success status of the operation.
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// Value produced by the operation.
    /// </summary>
    public required T Value { get; init; }

    /// <summary>
    /// CLI execution result associated with the operation.
    /// </summary>
    public CliResult? CliResult { get; init; }

    #endregion

    /// <summary>
    /// Attempts to provide the stored value when the result is fully successful.
    /// </summary>
    /// <param name="value">Stored value assigned by the method.</param>
    /// <returns><see langword="true"/> when the result is fully successful; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(out T value)
    {
        value = Value;
        return IsFullyProperValue;
    }
    
    /// <summary>
    /// Success state with no exception and no CLI error output.
    /// </summary>
    public bool IsFullyProperValue => IsSuccess
                                       && Exception is null
                                       && string.IsNullOrEmpty(CliResult?.Error);
}
