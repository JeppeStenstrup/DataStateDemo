namespace DataStateDemo.Common;

public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly Error? _error;
    private readonly bool _isSuccess;

    private Result(T value)
    {
        _value = value;
        _error = null;
        _isSuccess = true;
    }

    private Result(Error error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;
    public T Value => _isSuccess ? _value! : throw new InvalidOperationException("Result does not have a value.");
    public Error Error => !_isSuccess ? _error! : throw new InvalidOperationException("Result does not have an error.");

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
    public static Result<T> Failure(string code, string description) => new(Error.Failure(code, description));

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);

    public TResult Handle<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        _isSuccess ? onSuccess(_value!) : onFailure(_error!);
}
