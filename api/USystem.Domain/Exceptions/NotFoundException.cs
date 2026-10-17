// El recurso solicitado no existe o fue dado de baja lógicamente.
namespace USystem.Domain.Exceptions;

public class NotFoundException : USystemException
{
    public NotFoundException(string ErrorCode, IReadOnlyDictionary<string, string>? Values = null)
        : base(ErrorCode, Values)
    {
    }

    public NotFoundException()
        : base("Api.NotFound")
    {
    }
}
