public record OperationResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public int? StatusCode { get; }

    private OperationResult(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private OperationResult(string error, int statusCode)
    {
        IsSuccess = false;
        ErrorMessage = error;
        StatusCode = statusCode;
    }

    public static OperationResult<T> Success(T value) => new(value);
    public static OperationResult<T> Failure(string error, int statusCode = 400) => new(error, statusCode);
}
