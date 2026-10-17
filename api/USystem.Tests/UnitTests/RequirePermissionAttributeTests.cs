// Pruebas del atributo que exige permisos en los controladores.
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using USystem.Api.Attributes;
using USystem.Application.Interfaces;
using USystem.Infrastructure.Security;

namespace USystem.Tests.UnitTests;

public class RequirePermissionAttributeTests
{
    private const string Permission = "Users.View";

    [Fact]
    public void AnonymousRequestGetsUnauthorized()
    {
        var Context = BuildFilterContext(new ClaimsPrincipal(new ClaimsIdentity()));

        new RequirePermissionAttribute(Permission).OnAuthorization(Context);

        Assert.IsType<UnauthorizedObjectResult>(Context.Result);
    }

    [Fact]
    public void AuthenticatedWithoutClaimGetsForbidden()
    {
        var Identity = new ClaimsIdentity(new[] { new Claim("Permission", "Users.Create") }, "Test");
        var Context = BuildFilterContext(new ClaimsPrincipal(Identity));

        new RequirePermissionAttribute(Permission).OnAuthorization(Context);

        var Result = Assert.IsType<ObjectResult>(Context.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, Result.StatusCode);
    }

    [Fact]
    public void AuthenticatedWithClaimIsAllowed()
    {
        var Identity = new ClaimsIdentity(
            new[] { new Claim(JwtTokenGenerator.PermissionClaimType, Permission) },
            "Test");
        var Context = BuildFilterContext(new ClaimsPrincipal(Identity));

        new RequirePermissionAttribute(Permission).OnAuthorization(Context);

        Assert.Null(Context.Result);
    }

    [Fact]
    public void EveryPermissionIsRequired()
    {
        var Identity = new ClaimsIdentity(
            new[] { new Claim(JwtTokenGenerator.PermissionClaimType, Permission) },
            "Test");
        var Context = BuildFilterContext(new ClaimsPrincipal(Identity));

        new RequirePermissionAttribute(Permission, "Users.Create").OnAuthorization(Context);

        var Result = Assert.IsType<ObjectResult>(Context.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, Result.StatusCode);
    }

    private static AuthorizationFilterContext BuildFilterContext(ClaimsPrincipal User)
    {
        var HttpContext = new DefaultHttpContext { User = User };
        HttpContext.RequestServices = BuildServices();
        HttpContext.Request.Headers.AcceptLanguage = "es-MX";

        var ActionContext = new ActionContext(HttpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(ActionContext, new List<IFilterMetadata>());
    }

    private static IServiceProvider BuildServices()
    {
        var Messages = new Mock<IErrorMessageProvider>();
        Messages
            .Setup(Provider => Provider.ResolveLanguage(It.IsAny<string?>()))
            .Returns("es");
        Messages
            .Setup(Provider => Provider.GetMessage(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, string>?>(),
                It.IsAny<string?>()))
            .Returns((string Code, IReadOnlyDictionary<string, string>? Values, string? Language) => Code);

        var Services = new ServiceCollection();
        Services.AddSingleton(Messages.Object);
        return Services.BuildServiceProvider();
    }
}
