namespace MediaProcessorLibrary;

public readonly struct ResultMedia
{
    public bool IsSuccess { get; }
    public Operation? Operation { get; }
    public ErrorCode? Error { get; }
    public string? Message { get; }

    private ResultMedia(bool success, Operation? op, ErrorCode? error)
    {
        IsSuccess = success;
        Operation = op;
        Error = error;
        Message = error?.GetMessage();
    }
    public static ResultMedia Ok(Operation op) => new(true, op, null);
    public static ResultMedia Fail(ErrorCode error) => new(false, null, error);
}

public readonly struct ResultMedia<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Operation? Operation { get; }
    public ErrorCode? Error { get; }
    public string? Message { get; }

    private ResultMedia(bool success, T? value, Operation? operation, ErrorCode? error)
    {
        IsSuccess = success;
        Value = value;
        Operation = operation;
        Error = error;
        Message = error?.GetMessage();
    }

    public static ResultMedia<T> Ok(T value, Operation operation)
        => new(true, value, operation, null);

    public static ResultMedia<T> Fail(ErrorCode error)
        => new(false, default, null, error);
}