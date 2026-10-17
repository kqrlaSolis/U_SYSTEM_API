// Consulta de la bitácora: paginado con filtros y nombre de usuario resuelto.
using USystem.Application.Dtos;
using USystem.Domain.Common;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _AuditLogRepository;
    private readonly IUserRepository _UserRepository;

    public AuditLogService(
        IAuditLogRepository AuditLogRepository,
        IUserRepository UserRepository)
    {
        _AuditLogRepository = AuditLogRepository;
        _UserRepository = UserRepository;
    }

    public async Task<PagedResult<AuditLogDto>> GetPagedAsync(
        AuditLogQuery Query,
        CancellationToken Cancellation = default)
    {
        var Page = await _AuditLogRepository.GetPagedAsync(Query, Cancellation);

        var UserIds = Page.Items
            .Where(Entry => Entry.UserId is not null)
            .Select(Entry => Entry.UserId!.Value)
            .Distinct()
            .ToArray();

        var Usernames = await _UserRepository.GetUsernamesAsync(UserIds, Cancellation);

        var Items = Page.Items
            .Select(Entry => ToDto(
                Entry,
                Entry.UserId is not null && Usernames.TryGetValue(Entry.UserId.Value, out var Username)
                    ? Username
                    : null))
            .ToList();

        return PagedResult<AuditLogDto>.Create(Items, Page.TotalCount, Query);
    }

    public async Task<AuditLogDto> GetByIdAsync(long Id, CancellationToken Cancellation = default)
    {
        var Entry = await _AuditLogRepository.GetByIdAsync(Id, Cancellation)
            ?? throw new NotFoundException("Api.NotFound");

        string? Username = null;
        if (Entry.UserId is not null)
        {
            var Usernames = await _UserRepository.GetUsernamesAsync(new[] { Entry.UserId.Value }, Cancellation);
            Usernames.TryGetValue(Entry.UserId.Value, out Username);
        }

        return ToDto(Entry, Username);
    }

    private static AuditLogDto ToDto(AuditLog Entry, string? Username) => new()
    {
        Id = Entry.Id,
        UserId = Entry.UserId,
        Username = Username,
        Action = Entry.Action,
        Module = Entry.Module,
        Entity = Entry.Entity,
        EntityId = Entry.EntityId,
        OldValue = Entry.OldValue,
        NewValue = Entry.NewValue,
        IpAddress = Entry.IpAddress,
        CreatedAt = Entry.CreatedAt,
    };
}
