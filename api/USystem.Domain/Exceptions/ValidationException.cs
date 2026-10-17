// Datos de entrada inválidos; el campo se informa para que la UI lo resalte.
namespace USystem.Domain.Exceptions;

public class ValidationException : USystemException
{
    public ValidationException(
        string ErrorCode,
        string Field = "",
        IReadOnlyDictionary<string, string>? Values = null)
        : base(ErrorCode, Values)
    {
        this.Field = Field;
    }

    // Nombre de la propiedad del DTO que falló
    public string Field { get; }
}
