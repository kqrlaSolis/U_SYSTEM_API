// Error de validación o de negocio asociado a un campo del DTO.
namespace USystem.Application.Common;

public class ApiError
{
    // Código estable del catálogo i18n, por ejemplo "Users.DuplicateEmail"
    public string Code { get; set; } = string.Empty;

    // Propiedad del DTO que origina el error; vacío cuando es general
    public string Field { get; set; } = string.Empty;

    // Mensaje ya traducido según Accept-Language
    public string Message { get; set; } = string.Empty;
}
