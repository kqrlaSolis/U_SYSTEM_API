// Edición de parámetros: valida tipo y rango, invalida la caché y deja rastro en la bitácora.
using USystem.Application.Common;
using USystem.Application.Dtos;
using USystem.Application.Interfaces;
using USystem.Domain.Entities;
using USystem.Domain.Exceptions;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISystemSettingRepository _SettingRepository;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly ISettingsProvider _SettingsProvider;
    private readonly IAuditService _AuditService;

    public SettingsService(
        ISystemSettingRepository SettingRepository,
        IUnitOfWork UnitOfWork,
        ISettingsProvider SettingsProvider,
        IAuditService AuditService)
    {
        _SettingRepository = SettingRepository;
        _UnitOfWork = UnitOfWork;
        _SettingsProvider = SettingsProvider;
        _AuditService = AuditService;
    }

    public async Task<IReadOnlyList<SystemSettingDto>> GetAllAsync(
        CancellationToken Cancellation = default)
    {
        var Settings = await _SettingRepository.GetAllAsync(Cancellation);

        return Settings
            .OrderBy(Setting => Setting.Category, StringComparer.Ordinal)
            .ThenBy(Setting => Setting.SettingKey, StringComparer.Ordinal)
            .Select(ToDto)
            .ToList();
    }

    public async Task<SystemSettingDto> UpdateAsync(
        string SettingKey,
        UpdateSettingRequest Request,
        CancellationToken Cancellation = default)
    {
        var Setting = await _SettingRepository.GetByKeyAsync(SettingKey, Cancellation)
            ?? throw new NotFoundException(
                "Settings.UnknownKey",
                new Dictionary<string, string> { ["Key"] = SettingKey });

        var NewValue = Request.SettingValue.Trim();

        if (!SettingRules.IsValid(Setting.SettingKey, Setting.ValueType, NewValue))
        {
            throw new ValidationException(
                "Settings.InvalidValue",
                nameof(Request.SettingValue),
                new Dictionary<string, string> { ["Key"] = Setting.SettingKey });
        }

        var OldValue = Setting.SettingValue;
        Setting.SettingValue = NewValue;
        _SettingRepository.Update(Setting);
        await _UnitOfWork.SaveChangesAsync(Cancellation);

        // El cambio aplica sin reiniciar la API
        _SettingsProvider.Invalidate();

        await _AuditService.LogAsync(
            "Update",
            "Settings",
            nameof(SystemSetting),
            Setting.SettingKey,
            new { SettingValue = OldValue },
            new { SettingValue = NewValue },
            Cancellation);

        return ToDto(Setting);
    }

    private static SystemSettingDto ToDto(SystemSetting Setting) => new()
    {
        Id = Setting.Id,
        SettingKey = Setting.SettingKey,
        SettingValue = Setting.SettingValue,
        ValueType = Setting.ValueType,
        Category = Setting.Category,
        Description = Setting.Description,
    };
}
