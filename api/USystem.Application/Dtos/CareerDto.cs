// Carrera del catálogo académico.
namespace USystem.Application.Dtos;

public class CareerDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    // Licenciatura, Ingeniería, Maestría…
    public string Level { get; set; } = string.Empty;

    // Duración en cuatrimestres
    public byte TermCount { get; set; }

    public short TotalCredits { get; set; }

    // Escolarizada, Mixta, En línea…
    public string Modality { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
