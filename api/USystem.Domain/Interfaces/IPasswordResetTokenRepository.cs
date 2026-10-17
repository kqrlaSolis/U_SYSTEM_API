// Tokens de recuperación de contraseña: un solo uso y con vigencia corta.
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(PasswordResetToken Token, CancellationToken Cancellation = default);

    Task<PasswordResetToken?> GetByHashAsync(string TokenHash, CancellationToken Cancellation = default);

    void Update(PasswordResetToken Token);

    Task InvalidateAllForUserAsync(long UserId, DateTime UsedAt, CancellationToken Cancellation = default);
}
