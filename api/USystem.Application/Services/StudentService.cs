// Reglas de alumnos: matrícula generada, CURP única y baja bloqueada con inscripciones.
using System.Globalization;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _StudentRepository;
    private readonly ICareerRepository _CareerRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IAuditService _AuditService;

    public StudentService(
        IStudentRepository StudentRepository,
        ICareerRepository CareerRepository,
        IUnitOfWork UnitOfWork,
        IAuditService AuditService)
    {
        _StudentRepository = StudentRepository;
        _CareerRepository = CareerRepository;
        _UnitOfWork = UnitOfWork;
        _AuditService = AuditService;
    }

    public async Task<PagedResult<StudentDto>> GetPagedAsync(
        PagedQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _StudentRepository.GetPagedAsync(Query, Cancellation);
        var CareerNames = await LoadCareerNamesAsync(Page.Items, Cancellation);

        var Items = Page.Items
            .Select(Student => StudentDto.FromEntity(
                Student,
                CareerNames.GetValueOrDefault(Student.CareerId, string.Empty)))
            .ToList();

        return PagedResult<StudentDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<StudentDto> GetByIdAsync(long Id, CancellationToken Cancellation = default)
    {
        var Student = await _StudentRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        var CareerNames = await LoadCareerNamesAsync(new[] { Student }, Cancellation);
        return StudentDto.FromEntity(Student, CareerNames.GetValueOrDefault(Student.CareerId, string.Empty));
    }

    public async Task<StudentDto> CreateAsync(
        CreateStudentRequest Request,
        CancellationToken Cancellation = default)
    {
        var Curp = Request.Curp.Trim().ToUpperInvariant();

        if (await _StudentRepository.ExistsByCurpAsync(Curp, null, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Students.CurpTaken",
                new Dictionary<string, string> { ["Curp"] = Curp });
        }

        var NewStudent = new Student
        {
            StudentNumber = await BuildStudentNumberAsync(Cancellation),
            CareerId = Request.CareerId,
            StudyPlanId = Request.StudyPlanId,
            EnrollmentTermId = Request.EnrollmentTermId,
            FirstName = Request.FirstName.Trim(),
            PaternalLastName = Request.PaternalLastName.Trim(),
            MaternalLastName = Normalize(Request.MaternalLastName),
            Curp = Curp,
            BirthDate = Request.BirthDate,
            Gender = Request.Gender.Trim(),
            Email = Normalize(Request.Email),
            Phone = Normalize(Request.Phone),
            AddressLine = Normalize(Request.AddressLine),
            City = Normalize(Request.City),
            State = Normalize(Request.State),
            PostalCode = Normalize(Request.PostalCode),
            Status = "Active",
        };

        await _StudentRepository.AddAsync(NewStudent, Cancellation);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Create",
            "Students",
            nameof(Student),
            NewStudent.Id.ToString(CultureInfo.InvariantCulture),
            null,
            new { NewStudent.StudentNumber, NewStudent.FirstName, NewStudent.PaternalLastName, NewStudent.CareerId },
            Cancellation);

        var CareerNames = await LoadCareerNamesAsync(new[] { NewStudent }, Cancellation);
        return StudentDto.FromEntity(
            NewStudent,
            CareerNames.GetValueOrDefault(NewStudent.CareerId, string.Empty));
    }

    public async Task<StudentDto> UpdateAsync(
        long Id,
        UpdateStudentRequest Request,
        CancellationToken Cancellation = default)
    {
        var Student = await _StudentRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        var Curp = Request.Curp.Trim().ToUpperInvariant();

        if (await _StudentRepository.ExistsByCurpAsync(Curp, Student.Id, true, Cancellation))
        {
            throw BusinessRuleException.Conflict(
                "Students.CurpTaken",
                new Dictionary<string, string> { ["Curp"] = Curp });
        }

        var Before = new { Student.CareerId, Student.StudyPlanId, Student.Status, Student.Curp };

        Student.CareerId = Request.CareerId;
        Student.StudyPlanId = Request.StudyPlanId;
        Student.EnrollmentTermId = Request.EnrollmentTermId;
        Student.FirstName = Request.FirstName.Trim();
        Student.PaternalLastName = Request.PaternalLastName.Trim();
        Student.MaternalLastName = Normalize(Request.MaternalLastName);
        Student.Curp = Curp;
        Student.BirthDate = Request.BirthDate;
        Student.Gender = Request.Gender.Trim();
        Student.Email = Normalize(Request.Email);
        Student.Phone = Normalize(Request.Phone);
        Student.AddressLine = Normalize(Request.AddressLine);
        Student.City = Normalize(Request.City);
        Student.State = Normalize(Request.State);
        Student.PostalCode = Normalize(Request.PostalCode);
        Student.Status = Request.Status.Trim();

        _StudentRepository.Update(Student);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Update",
            "Students",
            nameof(Student),
            Student.Id.ToString(CultureInfo.InvariantCulture),
            Before,
            new { Student.CareerId, Student.StudyPlanId, Student.Status, Student.Curp },
            Cancellation);

        var CareerNames = await LoadCareerNamesAsync(new[] { Student }, Cancellation);
        return StudentDto.FromEntity(Student, CareerNames.GetValueOrDefault(Student.CareerId, string.Empty));
    }

    public async Task DeleteAsync(long Id, CancellationToken Cancellation = default)
    {
        var Student = await _StudentRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Students.NotFound");

        if (await _StudentRepository.HasEnrollmentsAsync(Student.Id, Cancellation))
        {
            throw new BusinessRuleException(
                "Students.InUse",
                new Dictionary<string, string> { ["StudentNumber"] = Student.StudentNumber });
        }

        _StudentRepository.Remove(Student);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        await _AuditService.LogAsync(
            "Delete",
            "Students",
            nameof(Student),
            Student.Id.ToString(CultureInfo.InvariantCulture),
            new { Student.StudentNumber, Student.FirstName, Student.PaternalLastName },
            null,
            Cancellation);
    }

    // Matrícula: A + año + consecutivo de 4 dígitos (A20260001)
    private async Task<string> BuildStudentNumberAsync(CancellationToken Cancellation)
    {
        var Year = DateTime.UtcNow.Year;
        var Prefix = $"A{Year}";
        var Last = await _StudentRepository.GetLastStudentNumberAsync(Prefix, Cancellation);

        var Sequence = 1;
        if (Last is not null && Last.Length > Prefix.Length
            && int.TryParse(Last[Prefix.Length..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var Parsed))
        {
            Sequence = Parsed + 1;
        }

        return $"{Prefix}{Sequence.ToString("D4", CultureInfo.InvariantCulture)}";
    }

    private async Task<Dictionary<int, string>> LoadCareerNamesAsync(
        IReadOnlyCollection<Student> Students,
        CancellationToken Cancellation)
    {
        var CareerIds = Students.Select(Student => Student.CareerId).Distinct().ToList();
        if (CareerIds.Count == 0)
        {
            return new Dictionary<int, string>();
        }

        var Careers = await _CareerRepository.GetByIdsAsync(CareerIds, Cancellation);
        return Careers.ToDictionary(Career => Career.Id, Career => Career.Name);
    }

    private static string? Normalize(string? Value)
        => string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
