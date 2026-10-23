// Reglas del expediente de inscripción: documentos obligatorios y adeudos antes de confirmar.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IEnrollmentChecklistService
{
    Task<EnrollmentChecklistDto> GetAsync(
        long StudentId,
        int AcademicTermId,
        CancellationToken Cancellation = default);

    Task<ChecklistDocumentDto> RegisterDocumentAsync(
        long StudentId,
        RegisterStudentDocumentRequest Request,
        CancellationToken Cancellation = default);

    Task<ChecklistDocumentDto> ValidateDocumentAsync(
        long DocumentId,
        CancellationToken Cancellation = default);

    Task ConfirmAsync(long EnrollmentId, CancellationToken Cancellation = default);
}

public class EnrollmentChecklistService : IEnrollmentChecklistService
{
    private readonly IEnrollmentChecklistRepository _ChecklistRepository;
    private readonly IStudentRepository _StudentRepository;
    private readonly IAcademicTermRepository _TermRepository;
    private readonly IEnrollmentRepository _EnrollmentRepository;
    private readonly ICurrentUser _CurrentUser;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public EnrollmentChecklistService(
        IEnrollmentChecklistRepository ChecklistRepository,
        IStudentRepository StudentRepository,
        IAcademicTermRepository TermRepository,
        IEnrollmentRepository EnrollmentRepository,
        ICurrentUser CurrentUser,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _ChecklistRepository = ChecklistRepository;
        _StudentRepository = StudentRepository;
        _TermRepository = TermRepository;
        _EnrollmentRepository = EnrollmentRepository;
        _CurrentUser = CurrentUser;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<EnrollmentChecklistDto> GetAsync(
        long StudentId,
        int AcademicTermId,
        CancellationToken Cancellation = default)
    {
        var Student = await _StudentRepository.GetByIdAsync(StudentId, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        _ = await _TermRepository.GetByIdAsync(AcademicTermId, Cancellation)
            ?? throw new NotFoundException("AcademicTerms.NotFound");

        var Types = await _ChecklistRepository.GetDocumentTypesAsync(Cancellation);
        var Documents = await _ChecklistRepository.GetStudentDocumentsAsync(StudentId, Cancellation);
        var Charges = await _ChecklistRepository.GetPendingChargesAsync(StudentId, AcademicTermId, Cancellation);

        var Items = Types.Select(Type =>
        {
            var Document = Documents.FirstOrDefault(Item => Item.DocumentTypeId == Type.Id);
            return new ChecklistDocumentDto
            {
                DocumentTypeId = Type.Id,
                Name = Type.Name,
                IsMandatory = Type.IsMandatory,
                StudentDocumentId = Document?.Id,
                Status = Document?.Status ?? "Pending",
                FilePath = Document?.FilePath,
                Notes = Document?.Notes,
            };
        }).ToList();

        var MissingMandatory = Items.Any(Item =>
            Item.IsMandatory && !string.Equals(Item.Status, "Validated", StringComparison.OrdinalIgnoreCase));

        var PendingAmount = Charges.Sum(Charge => Charge.Amount - Charge.Discount + Charge.Surcharge);
        var Blocking = new List<string>();
        var Warnings = new List<string>();

        // Regla de negocio: la documentación NO bloquea la inscripción; el alumno tiene
        // tres meses para completarla y se muestra un aviso.
        if (MissingMandatory)
        {
            Warnings.Add("Enrollments.DocumentsPending");
        }

        if (Charges.Count > 0)
        {
            Blocking.Add("Enrollments.PaymentPending");
        }

        var FullName = string.Join(
            ' ',
            new[] { Student.FirstName, Student.PaternalLastName, Student.MaternalLastName }
                .Where(Part => !string.IsNullOrWhiteSpace(Part)));

        return new EnrollmentChecklistDto
        {
            StudentId = Student.Id,
            StudentNumber = Student.StudentNumber,
            FullName = FullName,
            AcademicTermId = AcademicTermId,
            Documents = Items,
            PendingCharges = Charges.Count,
            PendingAmount = PendingAmount,
            DocumentsComplete = !MissingMandatory,
            PaymentClear = Charges.Count == 0,
            CanConfirm = Blocking.Count == 0,
            Warnings = Warnings,
            BlockingReasons = Blocking,
        };
    }

    public async Task<ChecklistDocumentDto> RegisterDocumentAsync(
        long StudentId,
        RegisterStudentDocumentRequest Request,
        CancellationToken Cancellation = default)
    {
        _ = await _StudentRepository.GetByIdAsync(StudentId, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        var Types = await _ChecklistRepository.GetDocumentTypesAsync(Cancellation);
        var Type = Types.FirstOrDefault(Item => Item.Id == Request.DocumentTypeId)
            ?? throw new NotFoundException("Documents.TypeNotFound");

        var Existing = (await _ChecklistRepository.GetStudentDocumentsAsync(StudentId, Cancellation))
            .FirstOrDefault(Item => Item.DocumentTypeId == Request.DocumentTypeId);

        if (Existing is null)
        {
            var NewDocument = new StudentDocument
            {
                StudentId = StudentId,
                DocumentTypeId = Type.Id,
                Status = "Delivered",
                FilePath = Normalize(Request.FilePath),
                Notes = Normalize(Request.Notes),
            };

            await _ChecklistRepository.AddDocumentAsync(NewDocument, Cancellation);
            await _UnitOfWork.SaveChangesAsync(Cancellation);

            await _AuditService.LogAsync(
                "Create",
                "Enrollments",
                nameof(StudentDocument),
                NewDocument.Id.ToString(CultureInfo.InvariantCulture),
                null,
                new { StudentId, Type.Name, NewDocument.Status },
                Cancellation);

            return ToItem(Type, NewDocument);
        }

        // Reenvío: vuelve a Delivered para que se valide de nuevo
        Existing.Status = "Delivered";
        Existing.FilePath = Normalize(Request.FilePath) ?? Existing.FilePath;
        Existing.Notes = Normalize(Request.Notes) ?? Existing.Notes;

        _ChecklistRepository.UpdateDocument(Existing);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Enrollments",
            nameof(StudentDocument),
            Existing.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { StudentId, Type.Name, Existing.Status },
            Cancellation);

        return ToItem(Type, Existing);
    }

    public async Task<ChecklistDocumentDto> ValidateDocumentAsync(
        long DocumentId,
        CancellationToken Cancellation = default)
    {
        var Document = await _ChecklistRepository.GetDocumentAsync(DocumentId, Cancellation)
            ?? throw new NotFoundException("Documents.NotFound");

        Document.Status = "Validated";
        Document.ValidatedBy = _CurrentUser.UserId;
        Document.ValidatedAt = DateTime.UtcNow;

        _ChecklistRepository.UpdateDocument(Document);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Enrollments",
            nameof(StudentDocument),
            Document.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { Document.StudentId, Document.Status },
            Cancellation);

        var Types = await _ChecklistRepository.GetDocumentTypesAsync(Cancellation);
        var Type = Types.First(Item => Item.Id == Document.DocumentTypeId);

        return ToItem(Type, Document);
    }

    public async Task ConfirmAsync(long EnrollmentId, CancellationToken Cancellation = default)
    {
        var Enrollment = await _EnrollmentRepository.GetByIdAsync(EnrollmentId, Cancellation)
            ?? throw new NotFoundException("Enrollments.NotFound");

        if (string.Equals(Enrollment.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            throw BusinessRuleException.Conflict("Enrollments.AlreadyCancelled");
        }

        if (string.Equals(Enrollment.Status, "Confirmed", StringComparison.OrdinalIgnoreCase))
        {
            throw BusinessRuleException.Conflict("Enrollments.AlreadyConfirmed");
        }

        var Checklist = await GetAsync(Enrollment.StudentId, Enrollment.AcademicTermId, Cancellation);

        // Solo los adeudos bloquean; la documentación pendiente se registra como aviso
        if (!Checklist.CanConfirm)
        {
            throw BusinessRuleException.Conflict(Checklist.BlockingReasons[0]);
        }

        var Before = new { Enrollment.Status };

        Enrollment.Status = "Confirmed";
        _EnrollmentRepository.Update(Enrollment);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Enrollments",
            nameof(Enrollment),
            Enrollment.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Enrollment.Status, Checklist.DocumentsComplete, Checklist.Warnings },
            Cancellation);
    }

    private static ChecklistDocumentDto ToItem(DocumentType Type, StudentDocument Document)
        => new()
        {
            DocumentTypeId = Type.Id,
            Name = Type.Name,
            IsMandatory = Type.IsMandatory,
            StudentDocumentId = Document.Id,
            Status = Document.Status,
            FilePath = Document.FilePath,
            Notes = Document.Notes,
        };

    private static string? Normalize(string? Value)
        => string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
