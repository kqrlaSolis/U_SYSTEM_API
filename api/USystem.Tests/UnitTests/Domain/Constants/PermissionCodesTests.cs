// Pruebas del catálogo de permisos (PermissionCodes).
using USystem.Domain.Constants;

namespace USystem.Tests.UnitTests.Domain.Constants;

public class PermissionCodesTests
{
    [Fact]
    public void CodesAreUniqueAndComplete()
    {
        Assert.Equal(79, PermissionCodes.All.Count);
        Assert.Equal(PermissionCodes.All.Count, PermissionCodes.All.Distinct().Count());
    }

    [Fact]
    public void EveryCodeUsesTheModuleActionFormat()
    {
        foreach (var Code in PermissionCodes.All)
        {
            Assert.Matches(@"^[A-Z][A-Za-z]*\.[A-Z][A-Za-z]*$", Code);
        }
    }

    [Fact]
    public void NestedConstantsMatchTheAllList()
    {
        Assert.Contains(PermissionCodes.Users.Create, PermissionCodes.All);
        Assert.Contains(PermissionCodes.Grades.CloseReport, PermissionCodes.All);
        Assert.Contains(PermissionCodes.ReEnrollments.AuthorizeException, PermissionCodes.All);
        Assert.Contains(PermissionCodes.Settings.Edit, PermissionCodes.All);
    }
}
