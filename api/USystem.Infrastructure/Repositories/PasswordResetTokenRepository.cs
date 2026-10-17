// Tokens de recuperación de contraseña: alta, búsqueda por hash e invalidación.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly USystemDbContext _Context;

    public PasswordResetTokenRepository(USystemDbContext Context) => _Context = Context;

    public async Task AddAsync(PasswordResetToken Token, CancellationToken Cancellation = default)
        => await _Context.PasswordResetTokens.AddAsync(Token, Cancellation);

    public Task<PasswordResetToken?> GetByHashAsync(
        string TokenHash,
        CancellationToken Cancellation = default)
        => _Context.PasswordResetTokens.FirstOrDefaultAsync(Token => Token.TokenHash == TokenHash, Cancellation);

    public void Update(PasswordResetToken Token) => _Context.PasswordResetTokens.Update(Token);

    public async Task InvalidateAllForUserAsync(
        long UserId,
        DateTime UsedAt,
        CancellationToken Cancellation = default)
    {
        var Tokens = await _Context.PasswordResetTokens
            .Where(Token => Token.UserId == UserId && Token.UsedAt == null)
            .ToListAsync(Cancellation);

        foreach (var Token in Tokens)
        {
            Token.UsedAt = UsedAt;
        }
    }
}
