// Violación de una regla de negocio: la petición es válida pero el estado del sistema la impide.
namespace USystem.Domain.Exceptions;

public class BusinessRuleException : USystemException
{
    // Código HTTP asociado: 400 por defecto; 409 cuando choca con el estado actual del registro
    public const int DefaultStatusCode = 400;
    public const int ConflictStatusCode = 409;

    public BusinessRuleException(
        string ErrorCode,
        IReadOnlyDictionary<string, string>? Values = null,
        int StatusCode = DefaultStatusCode,
        Exception? InnerException = null)
        : base(ErrorCode, Values, InnerException)
    {
        this.StatusCode = StatusCode;
    }

    public int StatusCode { get; }

    // Atajo para duplicados y estados inválidos
    public static BusinessRuleException Conflict(
        string ErrorCode,
        IReadOnlyDictionary<string, string>? Values = null)
        => new(ErrorCode, Values, ConflictStatusCode);
}
