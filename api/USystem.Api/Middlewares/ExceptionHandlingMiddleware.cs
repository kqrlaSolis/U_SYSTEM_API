// Middleware global de errores: traduce las excepciones de dominio a ApiResponse con su código HTTP.
using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using USystem.Api.Common;
using USystem.Application.Common;
using USystem.Application.Interfaces;
using USystem.Domain.Exceptions;
using DomainValidationException = USystem.Domain.Exceptions.ValidationException;

namespace USystem.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _Next;
    private readonly ILogger<ExceptionHandlingMiddleware> _Logger;
    private readonly IErrorMessageProvider _Messages;

    public ExceptionHandlingMiddleware(
        RequestDelegate Next,
        ILogger<ExceptionHandlingMiddleware> Logger,
        IErrorMessageProvider Messages)
    {
        _Next = Next;
        _Logger = Logger;
        _Messages = Messages;
    }

    public async Task InvokeAsync(HttpContext Context)
    {
        try
        {
            await _Next(Context);
        }
        catch (Exception CaughtException)
        {
            await HandleAsync(Context, CaughtException);
        }
    }

    private async Task HandleAsync(HttpContext Context, Exception CaughtException)
    {
        var TraceId = Activity.Current?.Id ?? Context.TraceIdentifier;

        if (Context.Response.HasStarted)
        {
            // Ya se enviaron bytes: no se puede reescribir la respuesta, solo dejar rastro
            _Logger.LogError(
                CaughtException,
                "Error después de iniciar la respuesta HTTP (TraceId {TraceId})",
                TraceId);
            return;
        }

        var Language = _Messages.ResolveLanguage(Context.Request.Headers.AcceptLanguage.ToString());
        var (StatusCode, MessageCode, Errors, Level, LogMessage) = Map(CaughtException, Language);

        _Logger.Log(Level, CaughtException, "{LogMessage} (TraceId {TraceId})", LogMessage, TraceId);

        var Response = new ApiResponse<object>
        {
            Success = false,
            Message = _Messages.GetMessage(MessageCode, ValuesFor(CaughtException), Language),
            Errors = Errors,
            TraceId = TraceId,
        };

        Context.Response.Clear();
        Context.Response.StatusCode = StatusCode;
        Context.Response.Headers["X-Trace-Id"] = TraceId;
        await Context.Response.WriteAsJsonAsync(
            Response,
            ApiJson.SerializerOptions,
            "application/json",
            Context.RequestAborted);
    }

    private (int StatusCode, string MessageCode, List<ApiError> Errors, LogLevel Level, string LogMessage) Map(
        Exception CaughtException,
        string Language)
    {
        switch (CaughtException)
        {
            case DomainValidationException Validation:
                return (
                    StatusCodes.Status400BadRequest,
                    "Api.ValidationError",
                    new List<ApiError> { BuildError(Validation.Code, Validation.Field, Validation.Values, Language) },
                    LogLevel.Warning,
                    "Validación de negocio rechazada");

            case BusinessRuleException Business:
                return (
                    Business.StatusCode,
                    Business.Code,
                    new List<ApiError> { BuildError(Business.Code, string.Empty, Business.Values, Language) },
                    LogLevel.Warning,
                    "Regla de negocio rechazada");

            case NotFoundException NotFound:
                return (
                    StatusCodes.Status404NotFound,
                    NotFound.Code,
                    new List<ApiError>(),
                    LogLevel.Warning,
                    "Recurso no encontrado");

            case ForbiddenException Forbidden:
                return (
                    StatusCodes.Status403Forbidden,
                    Forbidden.Code,
                    new List<ApiError>(),
                    LogLevel.Warning,
                    "Operación no permitida para el usuario");

            case UnauthorizedAccessException:
                return (
                    StatusCodes.Status401Unauthorized,
                    "Api.Unauthenticated",
                    new List<ApiError>(),
                    LogLevel.Warning,
                    "Petición no autenticada");

            case FluentValidation.ValidationException FluentValidationException:
                return (
                    StatusCodes.Status400BadRequest,
                    "Api.ValidationError",
                    FluentValidationException.Errors.Select(Failure => BuildFluentError(Failure, Language)).ToList(),
                    LogLevel.Warning,
                    "Validación de datos rechazada");

            case DbUpdateException:
                return (
                    StatusCodes.Status500InternalServerError,
                    "Api.DatabaseError",
                    new List<ApiError>(),
                    LogLevel.Error,
                    "Error al guardar en la base de datos");

            default:
                return (
                    StatusCodes.Status500InternalServerError,
                    "Api.InternalError",
                    new List<ApiError>(),
                    LogLevel.Error,
                    "Error no controlado");
        }
    }

    private ApiError BuildError(
        string Code,
        string Field,
        IReadOnlyDictionary<string, string>? Values,
        string Language) => new()
        {
            Code = Code,
            Field = Field,
            Message = _Messages.GetMessage(Code, Values, Language),
        };

    private ApiError BuildFluentError(FluentValidation.Results.ValidationFailure Failure, string Language)
    {
        var Values = new Dictionary<string, string>(StringComparer.Ordinal) { ["Field"] = Failure.PropertyName };

        // El validador define el código i18n; si no lo define, se respeta su mensaje
        if (string.IsNullOrWhiteSpace(Failure.ErrorCode))
        {
            return new ApiError
            {
                Code = "Api.ValidationError",
                Field = Failure.PropertyName,
                Message = Failure.ErrorMessage,
            };
        }

        return new ApiError
        {
            Code = Failure.ErrorCode,
            Field = Failure.PropertyName,
            Message = _Messages.GetMessage(Failure.ErrorCode, Values, Language),
        };
    }

    private static IReadOnlyDictionary<string, string>? ValuesFor(Exception CaughtException)
        => CaughtException is USystemException DomainException ? DomainException.Values : null;
}
