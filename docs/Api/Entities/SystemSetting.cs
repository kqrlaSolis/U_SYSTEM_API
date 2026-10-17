// Entidad de la tabla SystemSettings.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class SystemSetting : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave del parámetro
    public string SettingKey { get; set; } = string.Empty;

    // Valor
    public string SettingValue { get; set; } = string.Empty;

    // String, Int, Decimal, Bool
    public string ValueType { get; set; } = string.Empty;

    // Categoría
    public string Category { get; set; } = string.Empty;

    // Descripción
    public string? Description { get; set; }
}
