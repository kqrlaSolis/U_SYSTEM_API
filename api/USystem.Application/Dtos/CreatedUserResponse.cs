// Resultado del alta: el usuario creado y, si la API la generó, la contraseña temporal.
namespace USystem.Application.Dtos;

public class CreatedUserResponse
{
    public UserDto User { get; set; } = new();

    public string? TemporaryPassword { get; set; }
}
