using System.Text.Json.Serialization;

namespace NZWalksAPI.Models.Dto.Response;

using System.Net;

public class ApiResponse<T>(HttpStatusCode status, string message, T data)
{
    public HttpStatusCode Status { get; set; } = status;
    public string Message { get; set; } = message;
    public T Data { get; set; } = data;

    public static ApiResponse<T?> Success(HttpStatusCode statusCode, string message, T data)
    {
        return new ApiResponse<T?>(statusCode, message, data);
    }

    public static ApiResponse<T?> Error(string message, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T?>(status, message, default);
    }
}

