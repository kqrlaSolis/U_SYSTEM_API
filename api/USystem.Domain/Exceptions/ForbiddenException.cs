// El usuario está autenticado pero no tiene el permiso que exige la operación.
namespace USystem.Domain.Exceptions;

public class ForbiddenException : USystemException
{
    public ForbiddenException(
        string ErrorCode = "Api.Forbidden",
        IReadOnlyDictionary<string, string>? Values = null)
        : base(ErrorCode, Values)
    {
    }
}
