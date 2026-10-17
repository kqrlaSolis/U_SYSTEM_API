// Entidad de la tabla Careers.
// Los nombres siguen la regla del proyecto: inglés y PascalCase.

using System;

namespace USystem.Domain.Entities;

public class Career : AuditableEntity
{
    // Llave primaria
    public int Id { get; set; }

    // Clave de la carrera
    public string Code { get; set; } = string.Empty;

    // Nombre
    public string Name { get; set; } = string.Empty;

    // Bachelor, Engineering, Master
    public string Level { get; set; } = string.Empty;

    // Duración en cuatrimestres
    public byte TermCount { get; set; }

    // Créditos totales
    public short TotalCredits { get; set; }

    // OnSite, Online, Hybrid
    public string Modality { get; set; } = string.Empty;

    // Activa
    public bool IsActive { get; set; }
}
