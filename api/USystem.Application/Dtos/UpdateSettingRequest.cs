// Contrato de PUT /api/v1/settings/{Key}: solo se edita el valor.
namespace USystem.Application.Dtos;

public class UpdateSettingRequest
{
    public string SettingValue { get; set; } = string.Empty;
}
