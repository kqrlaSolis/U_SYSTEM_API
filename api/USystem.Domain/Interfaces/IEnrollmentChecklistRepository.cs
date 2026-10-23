// Acceso a datos del expediente de inscripción (documentos y cargos del periodo).
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IEnrollmentChecklistRepository
{
    Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync(CancellationToken Cancellation = default);

    Task<IReadOnlyList<StudentDocument>> GetStudentDocumentsAsync(
        long StudentId,
        CancellationToken Cancellation = default);

    Task<StudentDocument?> GetDocumentAsync(long Id, CancellationToken Cancellation = default);

    Task AddDocumentAsync(StudentDocument Entity, CancellationToken Cancellation = default);

    void UpdateDocument(StudentDocument Entity);

    // Cargos abiertos del alumno en el periodo (Pending, Partial, Overdue)
    Task<IReadOnlyList<Charge>> GetPendingChargesAsync(
        long StudentId,
        int AcademicTermId,
        CancellationToken Cancellation = default);
}
