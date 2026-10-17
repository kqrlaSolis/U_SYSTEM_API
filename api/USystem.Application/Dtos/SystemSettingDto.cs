// Parámetro del sistema expuesto por la API.
namespace USystem.Application.Dtos;

public class SystemSettingDto
{
    public int Id { get; set; }

    public string SettingKey { get; set; } = string.Empty;

    public string SettingValue { get; set; } = string.Empty;

    // String, Int, Decimal o Bool
    public string ValueType { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }
}
