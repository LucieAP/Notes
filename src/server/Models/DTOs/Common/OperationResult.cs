public record OperationResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public int? StatusCode { get; }

    private OperationResult()
    {
        IsSuccess = true;
    }

    private OperationResult(string error, int statusCode)
    {
        IsSuccess = false;
        ErrorMessage = error;
        StatusCode = statusCode;
    }

    public static OperationResult Success() => new();
    public static OperationResult Failure(string error, int statusCode = 400) => new(error, statusCode);
}