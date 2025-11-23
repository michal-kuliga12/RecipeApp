namespace RecipeApp.Application.Helpers;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success()
        => new Result(true);

    public static Result Failure(string error)
        => new Result(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }
    protected Result(bool isSuccess, string? error = null, T value = default) : base(isSuccess, error)
    {
        if (isSuccess)
        {
            Value = value;
        }
        else
        {
            Value = default;
        }
    }

    public static Result<T> Success(T value)
        => new Result<T>(true, null, value);

    public static Result<T> Failure(string? error)
    => new Result<T>(false, error, default);
}
