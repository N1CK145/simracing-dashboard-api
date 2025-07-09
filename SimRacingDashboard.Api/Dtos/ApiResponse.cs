namespace SimRacingDashboard.Api.Dtos;

public class ApiResponse<T>
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(T? data, bool success = true, string? message = null)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    // Helper factory methods for convenience
    public static ApiResponse<T> Ok(T data, string? message = null)
        => new(data, true, message);

    public static ApiResponse<T> Fail(string message)
        => new(default, false, message);
}


public class ApiResponse
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }

    public ApiResponse() { }

    public ApiResponse(bool success = true, string? message = null)
    {
        Success = success;
        Message = message;
    }

    public static ApiResponse Ok(string? message = null)
        => new(true, message);

    public static ApiResponse Fail(string message)
        => new(false, message);
}
