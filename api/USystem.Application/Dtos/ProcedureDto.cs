// Tipos de trámite y solicitudes con folio.
namespace USystem.Application.Dtos;

public class ProcedureTypeDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? PaymentConceptId { get; set; }

    public bool IsActive { get; set; }
}

public class CreateProcedureTypeRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? PaymentConceptId { get; set; }
}

public class UpdateProcedureTypeRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? PaymentConceptId { get; set; }

    public bool IsActive { get; set; }
}

public class ProcedureDto
{
    public long Id { get; set; }

    public int ProcedureTypeId { get; set; }

    public string ProcedureTypeCode { get; set; } = string.Empty;

    public long StudentId { get; set; }

    public string FolioNumber { get; set; } = string.Empty;

    // Requested, InProcess, Issued, Delivered, Cancelled
    public string Status { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; }

    public DateTime? IssuedAt { get; set; }

    public string? FilePath { get; set; }

    public long UserId { get; set; }
}

public class CreateProcedureRequest
{
    public int ProcedureTypeId { get; set; }

    public long StudentId { get; set; }

    public long? UserId { get; set; }
}

public class IssueProcedureRequest
{
    // Ruta del PDF emitido (opcional)
    public string? FilePath { get; set; }
}
