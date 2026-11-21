// Fija el permiso de los endpoints que antes usaban el de otro módulo (ver docs/Security/Rbac.md).
using System.Collections.ObjectModel;
using System.Reflection;
using USystem.Api.Attributes;
using USystem.Api.Controllers;
using USystem.Domain.Constants;

namespace USystem.Tests.UnitTests.Api.Controllers;

public class EndpointPermissionTests
{
    public static TheoryData<Type, string, string> Endpoints => new()
    {
        // Kardex y avance: el Docente tiene Grades.View pero no Transcript ni AcademicProgress
        { typeof(TranscriptsController), nameof(TranscriptsController.Get), PermissionCodes.Transcript.View },
        { typeof(AcademicProgressController), nameof(AcademicProgressController.Get), PermissionCodes.AcademicProgress.View },
        // Reinscripción con sus propios permisos, no con los de Inscripciones
        { typeof(ReEnrollmentsController), nameof(ReEnrollmentsController.Get), PermissionCodes.ReEnrollments.View },
        { typeof(ReEnrollmentsController), nameof(ReEnrollmentsController.Create), PermissionCodes.ReEnrollments.Create },
        // Documentos del alumno con StudentDocuments.*
        { typeof(EnrollmentChecklistController), nameof(EnrollmentChecklistController.Register), PermissionCodes.StudentDocuments.Upload },
        { typeof(EnrollmentChecklistController), nameof(EnrollmentChecklistController.Validate), PermissionCodes.StudentDocuments.Validate },
        { typeof(EnrollmentChecklistController), nameof(EnrollmentChecklistController.Get), PermissionCodes.Enrollments.View },
        { typeof(EnrollmentChecklistController), nameof(EnrollmentChecklistController.Confirm), PermissionCodes.Enrollments.Create },
    };

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void EndpointRequiresTheModulePermission(Type Controller, string Action, string Expected)
    {
        var Method = Controller.GetMethod(Action, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"{Controller.Name}.{Action} no existe");

        var Attribute = Method.GetCustomAttributesData()
            .Single(Data => Data.AttributeType == typeof(RequirePermissionAttribute));

        // El constructor recibe params string[]: llega como un solo argumento de tipo arreglo
        var Values = (ReadOnlyCollection<CustomAttributeTypedArgument>)Attribute.ConstructorArguments[0].Value!;
        var Permissions = Values.Select(Value => (string)Value.Value!).ToArray();

        Assert.Equal(new[] { Expected }, Permissions);
    }
}
