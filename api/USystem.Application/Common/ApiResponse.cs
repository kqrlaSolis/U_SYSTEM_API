// Envoltura común de todas las respuestas de la API (JSON en PascalCase).
namespace USystem.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public List<ApiError> Errors { get; set; } = new();

    public string? TraceId { get; set; }

    public static ApiResponse<T> Ok(T Data, string Message = "OK") => new()
    {
        Success = true,
        Message = Message,
        Data = Data,
    };

    public static ApiResponse<T> Fail(string Message, params ApiError[] Errors) => new()
    {
        Success = false,
        Message = Message,
        Errors = Errors.ToList(),
    };
}
