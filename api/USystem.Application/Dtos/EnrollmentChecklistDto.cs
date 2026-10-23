// Expediente de inscripción: documentos requeridos y adeudos del periodo.
namespace USystem.Application.Dtos;

public class ChecklistDocumentDto
{
    public int DocumentTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsMandatory { get; set; }

    public long? StudentDocumentId { get; set; }

    // Pending, Delivered, Validated, Rejected
    public string Status { get; set; } = "Pending";

    public string? FilePath { get; set; }

    public string? Notes { get; set; }
}

public class EnrollmentChecklistDto
{
    public long StudentId { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int AcademicTermId { get; set; }

    public List<ChecklistDocumentDto> Documents { get; set; } = new();

    public int PendingCharges { get; set; }

    public decimal PendingAmount { get; set; }

    public bool DocumentsComplete { get; set; }

    public bool PaymentClear { get; set; }

    public bool CanConfirm { get; set; }

    // Avisos que no bloquean (documentación pendiente con 3 meses de gracia)
    public List<string> Warnings { get; set; } = new();

    // Códigos i18n de lo que impide confirmar (hoy solo adeudos)
    public List<string> BlockingReasons { get; set; } = new();
}

public class RegisterStudentDocumentRequest
{
    public int DocumentTypeId { get; set; }

    public string? FilePath { get; set; }

    public string? Notes { get; set; }
}
