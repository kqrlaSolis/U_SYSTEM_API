// Acceso a datos del expediente de inscripción.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class EnrollmentChecklistRepository : IEnrollmentChecklistRepository
{
    private static readonly string[] _OpenChargeStatuses = { "Pending", "Partial", "Overdue" };

    private readonly USystemDbContext _Context;

    public EnrollmentChecklistRepository(USystemDbContext Context) => _Context = Context;

    public async Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync(
        CancellationToken Cancellation = default)
        => await _Context.DocumentTypes
            .AsNoTracking()
            .OrderBy(Type => Type.Name)
            .ToListAsync(Cancellation);

    public async Task<IReadOnlyList<StudentDocument>> GetStudentDocumentsAsync(
        long StudentId,
        CancellationToken Cancellation = default)
        => await _Context.StudentDocuments
            .AsNoTracking()
            .Where(Document => Document.StudentId == StudentId)
            .ToListAsync(Cancellation);

    public Task<StudentDocument?> GetDocumentAsync(long Id, CancellationToken Cancellation = default)
        => _Context.StudentDocuments.FirstOrDefaultAsync(Document => Document.Id == Id, Cancellation);

    public async Task AddDocumentAsync(StudentDocument Entity, CancellationToken Cancellation = default)
        => await _Context.StudentDocuments.AddAsync(Entity, Cancellation);

    public void UpdateDocument(StudentDocument Entity) => _Context.StudentDocuments.Update(Entity);

    public async Task<IReadOnlyList<Charge>> GetPendingChargesAsync(
        long StudentId,
        int AcademicTermId,
        CancellationToken Cancellation = default)
        => await _Context.Charges
            .AsNoTracking()
            .Where(Charge => Charge.StudentId == StudentId
                && Charge.AcademicTermId == AcademicTermId
                && _OpenChargeStatuses.Contains(Charge.Status))
            .ToListAsync(Cancellation);
}
