// Pruebas del middleware global de errores.
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using USystem.Api.Middlewares;
using USystem.Application.Interfaces;
using USystem.Domain.Exceptions;

namespace USystem.Tests.UnitTests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task BusinessRuleConflictReturns409()
    {
        var Context = await InvokeAsync(
            _ => throw BusinessRuleException.Conflict("Users.DuplicateEmail"));

        var Body = await ReadBodyAsync(Context);

        Assert.Equal(StatusCodes.Status409Conflict, Context.Response.StatusCode);
        Assert.False(Body.GetProperty("Success").GetBoolean());
        Assert.Equal("msg:Users.DuplicateEmail", Body.GetProperty("Message").GetString());
        Assert.Equal("Users.DuplicateEmail", Body.GetProperty("Errors")[0].GetProperty("Code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(Body.GetProperty("TraceId").GetString()));
        Assert.True(Context.Response.Headers.ContainsKey("X-Trace-Id"));
    }

    [Fact]
    public async Task NotFoundReturns404()
    {
        var Context = await InvokeAsync(_ => throw new NotFoundException("Users.NotFound"));

        Assert.Equal(StatusCodes.Status404NotFound, Context.Response.StatusCode);
    }

    [Fact]
    public async Task ForbiddenReturns403()
    {
        var Context = await InvokeAsync(_ => throw new ForbiddenException());

        Assert.Equal(StatusCodes.Status403Forbidden, Context.Response.StatusCode);
    }

    [Fact]
    public async Task ValidationExceptionReturns400WithField()
    {
        var Context = await InvokeAsync(_ => throw new USystem.Domain.Exceptions.ValidationException(
            "Validation.Required",
            "Email"));

        var Body = await ReadBodyAsync(Context);

        Assert.Equal(StatusCodes.Status400BadRequest, Context.Response.StatusCode);
        Assert.Equal("Email", Body.GetProperty("Errors")[0].GetProperty("Field").GetString());
    }

    [Fact]
    public async Task FluentValidationErrorsAreMapped()
    {
        var AssertFailures = new[]
        {
            new ValidationFailure("Email", "invalid") { ErrorCode = "Validation.InvalidEmail" },
            new ValidationFailure("Username", "taken") { ErrorCode = "Users.DuplicateUsername" },
        };

        var Context = await InvokeAsync(_ => throw new FluentValidation.ValidationException(AssertFailures));
        var Body = await ReadBodyAsync(Context);

        Assert.Equal(StatusCodes.Status400BadRequest, Context.Response.StatusCode);
        Assert.Equal(2, Body.GetProperty("Errors").GetArrayLength());
        Assert.Equal("Email", Body.GetProperty("Errors")[0].GetProperty("Field").GetString());
        Assert.Equal(
            "msg:Validation.InvalidEmail",
            Body.GetProperty("Errors")[0].GetProperty("Message").GetString());
    }

    [Fact]
    public async Task UnexpectedExceptionReturns500()
    {
        var Context = await InvokeAsync(_ => throw new InvalidOperationException("boom"));
        var Body = await ReadBodyAsync(Context);

        Assert.Equal(StatusCodes.Status500InternalServerError, Context.Response.StatusCode);
        Assert.Equal("msg:Api.InternalError", Body.GetProperty("Message").GetString());
    }

    private static async Task<HttpContext> InvokeAsync(RequestDelegate Next)
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
            .Returns((string Code, IReadOnlyDictionary<string, string>? Values, string? Language) => $"msg:{Code}");

        var Middleware = new ExceptionHandlingMiddleware(
            Next,
            NullLogger<ExceptionHandlingMiddleware>.Instance,
            Messages.Object);

        var Context = new DefaultHttpContext();
        Context.Response.Body = new MemoryStream();
        Context.Request.Headers.AcceptLanguage = "es-MX";

        await Middleware.InvokeAsync(Context);
        return Context;
    }

    private static async Task<JsonElement> ReadBodyAsync(HttpContext Context)
    {
        Context.Response.Body.Position = 0;

        using var Reader = new StreamReader(Context.Response.Body, leaveOpen: true);
        var Json = await Reader.ReadToEndAsync();

        return JsonDocument.Parse(Json).RootElement.Clone();
    }
}
