// Refresh tokens: se guardan como hash y se revocan, nunca se reutilizan.
using USystem.Domain.Entities;

namespace USystem.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken Token, CancellationToken Cancellation = default);

    Task<RefreshToken?> GetByHashAsync(string TokenHash, CancellationToken Cancellation = default);

    void Update(RefreshToken Token);

    // Cierra todas las sesiones del usuario (cambio de contraseña, baja, etc.)
    Task RevokeAllForUserAsync(long UserId, DateTime RevokedAt, CancellationToken Cancellation = default);
}
