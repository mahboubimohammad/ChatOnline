namespace SharedLibrary.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public T? Data { get; set; }
    /// <summary>Provider response body when the caller needs the exact external contract.</summary>
    public string? RawResponse { get; set; }

    public static ServiceResult<T> Ok(T data, string message = "") =>
        new() { Success = true, StatusCode = 200, Data = data, Message = message };

    public static ServiceResult<T> Ok(string message = "") =>
        new() { Success = true, StatusCode = 200, Message = message };

    public static ServiceResult<T> Fail(string message, int statusCode = 400, string? error = null) =>
        new() { Success = false, StatusCode = statusCode, Message = message, Error = error };

    public static ServiceResult<T> Fail(T? data, string message, int statusCode = 400, string? error = null) =>
        new() { Success = false, StatusCode = statusCode, Data = data, Message = message, Error = error };

    public static ServiceResult<object> Fail(object value, int v)
    {
        throw new NotImplementedException();
    }
}
