using System.Collections;
using System.Runtime.CompilerServices;
using DataStateDemo.Common;
using CommonError = DataStateDemo.Common.Error;

namespace DataStateDemo.State;

public union DataState<T>(
    DataState<T>.Uninitialized,
    DataState<T>.Initialized,
    DataState<T>.Success,
    DataState<T>.Error
)
{
    public readonly record struct Uninitialized(T? Placeholder = default);
    public readonly record struct Initialized(T? StaleData = default);
    public readonly record struct Success(T Data)
    {
        public bool CheckIsEmpty(Func<T, bool>? customPredicate = null)
        {
            if (customPredicate is not null)
            {
                return customPredicate(Data);
            }

            return Data switch
            {
                null => true,
                string s => string.IsNullOrWhiteSpace(s),
                ICollection c => c.Count == 0,
                IReadOnlyCollection<object> rc => rc.Count == 0,
                IEnumerable enumerable => IsEnumerableEmpty(enumerable),
                _ => false
            };
        }

        private static bool IsEnumerableEmpty(IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }
    }
    public readonly record struct Error(CommonError Details, Exception? Exception = null);

    public bool IsUninitialized => this is Uninitialized;
    public bool IsInitialized => this is Initialized;
    public bool IsSuccess => this is Success;
    public bool IsError => this is Error;

    public T? DataOrDefault => this switch
    {
        Success s => s.Data,
        Initialized i => i.StaleData,
        Uninitialized u => u.Placeholder,
        Error => default
    };

    public T DataOr(T fallback) => DataOrDefault ?? fallback;

    public T Data => this switch
    {
        Success s => s.Data,
        _ => throw new InvalidOperationException("Current state does not contain data.")
    };

    public DataState<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return this switch
        {
            Uninitialized u => DataState<TResult>.None(u.Placeholder is not null ? selector(u.Placeholder) : default),
            Initialized i => DataState<TResult>.InProgress(i.StaleData is not null ? selector(i.StaleData) : default),
            Success s => DataState<TResult>.Loaded(selector(s.Data)),
            Error e => DataState<TResult>.Failed(e.Details, e.Exception)
        };
    }

    public DataState<T> OnSuccess(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (this is Success s)
        {
            action(s.Data);
        }
        return this;
    }

    public DataState<T> OnError(Action<CommonError, Exception?> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (this is Error e)
        {
            action(e.Details, e.Exception);
        }
        return this;
    }

    public DataState<T> ToLoaded(T data) => Loaded(data);
    public DataState<T> ToFailed(CommonError error, Exception? ex = null) => Failed(error, ex);
    public DataState<T> ToFailed(string code, string description, Exception? ex = null) => Failed(code, description, ex);
    public DataState<T> ToFailed(Exception ex, string code = "UnhandledException") => Failed(ex, code);

    public static DataState<T> None(T? placeholder = default) => new Uninitialized(placeholder);
    public static DataState<T> InProgress(T? stale = default) => new Initialized(stale);
    public static DataState<T> Loaded(T data) => new Success(data);
    public static DataState<T> Failed(CommonError error, Exception? ex = null) => new Error(error, ex);
    public static DataState<T> Failed(string code, string description, Exception? ex = null) =>
        new Error(CommonError.Failure(code, description), ex);
    public static DataState<T> Failed(Exception ex, string code = "UnhandledException") =>
        new Error(CommonError.Failure(code, ex.Message), ex);
}

public static class DataStateExtensions
{
    public static DataState<T> Initialize<T>(this ref DataState<T> state, bool preserveStale = true)
    {
        var stale = preserveStale ? state.DataOrDefault : default;
        state = DataState<T>.InProgress(stale);
        return state;
    }

    public static ValueTask<DataState<T>> Initialize<T>(
        this ref DataState<T> state,
        Func<Task<Result<T>>> operation,
        Action<T>? onSuccess = null,
        Action<CommonError, Exception?>? onError = null,
        bool preserveStale = true)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return state.Initialize(
            () => new ValueTask<Result<T>>(operation()),
            onSuccess,
            onError,
            preserveStale);
    }

    public static ValueTask<DataState<T>> Initialize<T>(
        this ref DataState<T> state,
        Func<ValueTask<Result<T>>> operation,
        Action<T>? onSuccess = null,
        Action<CommonError, Exception?>? onError = null,
        bool preserveStale = true)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var stale = preserveStale ? state.DataOrDefault : default;
        state = DataState<T>.InProgress(stale);
        return ExecuteAsync(operation, onSuccess, onError);
    }

    private static async ValueTask<DataState<T>> ExecuteAsync<T>(
        Func<ValueTask<Result<T>>> operation,
        Action<T>? onSuccess,
        Action<CommonError, Exception?>? onError)
    {
        DataState<T> next;
        try
        {
            var result = await operation();
            next = result.Handle(
                onSuccess: DataState<T>.Loaded,
                onFailure: err => DataState<T>.Failed(err));
        }
        catch (Exception ex)
        {
            next = DataState<T>.Failed(ex);
        }

        if (onSuccess is not null)
        {
            next.OnSuccess(onSuccess);
        }

        if (onError is not null)
        {
            next.OnError(onError);
        }

        return next;
    }

    public static int Count<T>(this DataState<List<T>> state) =>
        state.DataOrDefault?.Count ?? 0;

    public static int Count<T>(this DataState<List<T>> state, Func<T, bool> predicate) =>
        state.DataOrDefault?.Count(predicate) ?? 0;
}
