// Contexto de EF Core para MySQL. Los nombres de tabla y columna coinciden con docs/DataModel/Scripts.
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;

namespace USystem.Infrastructure.Persistence;

public class USystemDbContext : DbContext
{
    // Llena CreatedAt/UpdatedAt/DeletedAt y los usuarios de auditoría
    private readonly ICurrentUser? _CurrentUser;

    public USystemDbContext(DbContextOptions<USystemDbContext> Options, ICurrentUser? CurrentUser = null)
        : base(Options)
    {
        _CurrentUser = CurrentUser;
    }

    // Seguridad
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Catálogos académicos
    public DbSet<Career> Careers => Set<Career>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<AcademicTerm> AcademicTerms => Set<AcademicTerm>();
    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
    public DbSet<StudyPlanSubject> StudyPlanSubjects => Set<StudyPlanSubject>();
    public DbSet<SubjectPrerequisite> SubjectPrerequisites => Set<SubjectPrerequisite>();
    public DbSet<CourseMaterial> CourseMaterials => Set<CourseMaterial>();

    // Operación académica
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<ClassGroup> ClassGroups => Set<ClassGroup>();
    public DbSet<ClassGroupSchedule> ClassGroupSchedules => Set<ClassGroupSchedule>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<EnrollmentClassGroup> EnrollmentClassGroups => Set<EnrollmentClassGroup>();

    // Seguimiento académico
    public DbSet<Grade> Grades => Set<Grade>();

    // Finanzas
    public DbSet<PaymentConcept> PaymentConcepts => Set<PaymentConcept>();
    public DbSet<PaymentPlan> PaymentPlans => Set<PaymentPlan>();
    public DbSet<Scholarship> Scholarships => Set<Scholarship>();
    public DbSet<StudentScholarship> StudentScholarships => Set<StudentScholarship>();
    public DbSet<Charge> Charges => Set<Charge>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Extras
    public DbSet<ProcedureType> ProcedureTypes => Set<ProcedureType>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder Builder)
    {
        base.OnModelCreating(Builder);

        ConfigureCompositeKeys(Builder);
        ConfigureSoftDelete(Builder);
        ConfigureDecimals(Builder);
    }

    // Las tablas de enlace no tienen columna Id: su llave es compuesta
    private static void ConfigureCompositeKeys(ModelBuilder Builder)
    {
        Builder.Entity<RolePermission>().HasKey(Entity => new { Entity.RoleId, Entity.PermissionId });
        Builder.Entity<UserRole>().HasKey(Entity => new { Entity.UserId, Entity.RoleId });
        Builder.Entity<TeacherSubject>().HasKey(Entity => new { Entity.TeacherId, Entity.SubjectId });
        Builder.Entity<StudyPlanSubject>().HasKey(Entity => new { Entity.StudyPlanId, Entity.SubjectId });
        Builder.Entity<SubjectPrerequisite>().HasKey(
            Entity => new { Entity.StudyPlanId, Entity.SubjectId, Entity.PrerequisiteSubjectId });
        Builder.Entity<EnrollmentClassGroup>().HasKey(Entity => new { Entity.EnrollmentId, Entity.ClassGroupId });
    }

    // Filtro global: ninguna consulta devuelve registros dados de baja lógicamente
    private static void ConfigureSoftDelete(ModelBuilder Builder)
    {
        foreach (var EntityType in Builder.Model.GetEntityTypes())
        {
            var ClrType = EntityType.ClrType;
            if (!typeof(AuditableEntity).IsAssignableFrom(ClrType))
            {
                continue;
            }

            var Parameter = Expression.Parameter(ClrType, "Entity");
            var DeletedAt = Expression.Property(Parameter, nameof(AuditableEntity.DeletedAt));
            var Filter = Expression.Lambda(
                Expression.Equal(DeletedAt, Expression.Constant(null, typeof(DateTime?))),
                Parameter);

            Builder.Entity(ClrType).HasQueryFilter(Filter);
        }
    }

    // Montos y calificaciones con cuatro decimales; el esquema SQL es la fuente de verdad de los tipos
    private static void ConfigureDecimals(ModelBuilder Builder)
    {
        foreach (var EntityType in Builder.Model.GetEntityTypes())
        {
            foreach (var Property in EntityType.GetProperties())
            {
                if (Property.ClrType == typeof(decimal) || Property.ClrType == typeof(decimal?))
                {
                    Property.SetPrecision(18);
                    Property.SetScale(4);
                }
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken Cancellation = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(Cancellation);
    }

    private void ApplyAuditFields()
    {
        var Now = DateTime.UtcNow;
        var CurrentUserId = _CurrentUser?.UserId;

        foreach (var Entry in ChangeTracker.Entries<AuditableEntity>().ToList())
        {
            switch (Entry.State)
            {
                case EntityState.Added:
                    if (Entry.Entity.CreatedAt == default)
                    {
                        Entry.Entity.CreatedAt = Now;
                    }

                    Entry.Entity.CreatedBy ??= CurrentUserId;
                    break;

                case EntityState.Modified:
                    Entry.Entity.UpdatedAt = Now;
                    Entry.Entity.UpdatedBy ??= CurrentUserId;
                    break;

                case EntityState.Deleted:
                    // Borrado lógico: la fila nunca se elimina físicamente
                    Entry.State = EntityState.Modified;
                    Entry.Entity.DeletedAt = Now;
                    Entry.Entity.UpdatedAt = Now;
                    Entry.Entity.UpdatedBy ??= CurrentUserId;
                    break;
            }
        }
    }
}
