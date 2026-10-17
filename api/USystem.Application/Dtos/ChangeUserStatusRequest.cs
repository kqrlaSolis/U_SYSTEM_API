// Contrato de PATCH /api/v1/users/{Id}/status.
namespace USystem.Application.Dtos;

public class ChangeUserStatusRequest
{
    public bool IsActive { get; set; }
}
