// Pruebas del modelo de EF Core: llaves compuestas, borrado lógico y campos de auditoría.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Infrastructure.Persistence;

namespace USystem.Tests.UnitTests;

public class USystemDbContextTests
{
    private static USystemDbContext CreateContext()
    {
        var Options = new DbContextOptionsBuilder<USystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new USystemDbContext(Options);
    }

    private static User BuildUser(long Id) => new()
    {
        Id = Id,
        FirstName = "Ana",
        PaternalLastName = "Pérez",
        Email = $"ana{Id}@escuela.mx",
        Username = $"ana{Id}",
        PasswordHash = "hash",
    };

    [Fact]
    public void MapsEveryTableOfTheDataModel()
    {
        using var Context = CreateContext();

        Assert.Equal(37, Context.Model.GetEntityTypes().Count());
    }

    [Fact]
    public void JoinTablesUseCompositeKeys()
    {
        using var Context = CreateContext();

        Assert.Equal(2, PrimaryKeyCount<RolePermission>(Context));
        Assert.Equal(2, PrimaryKeyCount<UserRole>(Context));
        Assert.Equal(2, PrimaryKeyCount<TeacherSubject>(Context));
        Assert.Equal(2, PrimaryKeyCount<StudyPlanSubject>(Context));
        Assert.Equal(3, PrimaryKeyCount<SubjectPrerequisite>(Context));
        Assert.Equal(2, PrimaryKeyCount<EnrollmentClassGroup>(Context));
    }

    [Fact]
    public async Task SaveChangesStampsCreatedAt()
    {
        using var Context = CreateContext();

        Context.Users.Add(BuildUser(1));
        await Context.SaveChangesAsync();

        var Stored = await Context.Users.SingleAsync();
        Assert.NotEqual(default, Stored.CreatedAt);
    }

    [Fact]
    public async Task DeletedRowsAreHiddenByTheGlobalFilter()
    {
        using var Context = CreateContext();

        Context.Users.Add(BuildUser(1));
        Context.Users.Add(new User
        {
            Id = 2,
            FirstName = "Beto",
            PaternalLastName = "Ruiz",
            Email = "beto@escuela.mx",
            Username = "beto",
            PasswordHash = "hash",
            DeletedAt = DateTime.UtcNow,
        });

        await Context.SaveChangesAsync();

        var Visible = await Context.Users.ToListAsync();
        Assert.Single(Visible);
        Assert.Equal(1, Visible[0].Id);
    }

    [Fact]
    public async Task RemoveBecomesSoftDelete()
    {
        using var Context = CreateContext();

        var NewUser = BuildUser(1);
        Context.Users.Add(NewUser);
        await Context.SaveChangesAsync();

        Context.Users.Remove(NewUser);
        await Context.SaveChangesAsync();

        Assert.Empty(await Context.Users.ToListAsync());

        var Stored = await Context.Users.IgnoreQueryFilters().SingleAsync();
        Assert.NotNull(Stored.DeletedAt);
    }

    private static int PrimaryKeyCount<TEntity>(USystemDbContext Context) where TEntity : class
        => Context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Count ?? 0;
}
