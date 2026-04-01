namespace Twon.Application.Common;

public class BaseResult<T>
{
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public T? Data { get; set; }

    public bool IsSuccess => Status == "success";

    public static BaseResult<T> Success(T data, string? message = null) => new()
    {
        Code = "A001", Status = "success", Data = data, Message = message
    };

    public static BaseResult<T> Failure(string message) => new()
    {
        Code = "A002", Status = "failure", Message = message
    };

    public static BaseResult<T> NotFound(string message = "Not found.") => new()
    {
        Code = "A404", Status = "failure", Message = message
    };

    public static BaseResult<T> Conflict(string message) => new()
    {
        Code = "A409", Status = "failure", Message = message
    };

    public static BaseResult<T> Unauthorized(string message = "Unauthorized.") => new()
    {
        Code = "A401", Status = "failure", Message = message
    };

    // Remove refresh token before sending to client
    public BaseResult<T> WithoutRefreshToken() => this;
}
