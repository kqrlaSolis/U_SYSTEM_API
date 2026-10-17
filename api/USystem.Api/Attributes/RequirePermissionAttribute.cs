// Valida que el JWT incluya uno o varios permisos, por ejemplo [RequirePermission("Users.Create")].
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using USystem.Application.Common;
using USystem.Application.Interfaces;
using USystem.Infrastructure.Security;

namespace USystem.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _Permissions;

    // Se pueden exigir varios permisos a la vez: todos son obligatorios
    public RequirePermissionAttribute(params string[] Permissions)
        => _Permissions = Permissions ?? Array.Empty<string>();

    public void OnAuthorization(AuthorizationFilterContext Context)
    {
        var HttpContext = Context.HttpContext;
        var Messages = HttpContext.RequestServices.GetRequiredService<IErrorMessageProvider>();
        var Language = Messages.ResolveLanguage(HttpContext.Request.Headers.AcceptLanguage.ToString());

        var User = HttpContext.User;
        if (User.Identity?.IsAuthenticated != true)
        {
            var Message = Messages.GetMessage("Api.Unauthenticated", null, Language);
            Context.Result = new UnauthorizedObjectResult(BuildFailure("Api.Unauthenticated", string.Empty, Message));
            return;
        }

        var MissingPermission = _Permissions
            .FirstOrDefault(Permission => !User.HasClaim(JwtTokenGenerator.PermissionClaimType, Permission));

        if (MissingPermission is not null)
        {
            var Message = Messages.GetMessage("Api.Forbidden", null, Language);
            var Failure = BuildFailure("Api.Forbidden", MissingPermission, Message);
            Context.Result = new ObjectResult(Failure) { StatusCode = StatusCodes.Status403Forbidden };
        }
    }

    private static ApiResponse<object> BuildFailure(string Code, string Field, string Message) => new()
    {
        Success = false,
        Message = Message,
        Errors = new List<ApiError>
        {
            new()
            {
                Code = Code,
                Field = Field,
                Message = Message,
            },
        },
    };
}
