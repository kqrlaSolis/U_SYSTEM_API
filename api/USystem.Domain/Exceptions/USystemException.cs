// Raíz de las excepciones de negocio de U SYSTEM.
// El código es estable (por ejemplo "Users.DuplicateEmail") y los valores llenan los placeholders del
// catálogo i18n; el middleware de la API es quien traduce el mensaje según Accept-Language.
namespace USystem.Domain.Exceptions;

public abstract class USystemException : Exception
{
    protected USystemException(
        string ErrorCode,
        IReadOnlyDictionary<string, string>? Values = null,
        Exception? InnerException = null)
        : base(ErrorCode, InnerException)
    {
        Code = ErrorCode;
        this.Values = Values ?? new Dictionary<string, string>();
    }

    // Código del catálogo de mensajes; nunca es un texto visible
    public string Code { get; }

    // Valores para los placeholders {{Clave}} del mensaje traducido
    public IReadOnlyDictionary<string, string> Values { get; }
}
