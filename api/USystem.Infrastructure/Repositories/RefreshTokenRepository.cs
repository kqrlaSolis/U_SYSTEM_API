// Tokens de sesión: alta, búsqueda por hash y revocación.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly USystemDbContext _Context;

    public RefreshTokenRepository(USystemDbContext Context) => _Context = Context;

    public async Task AddAsync(RefreshToken Token, CancellationToken Cancellation = default)
        => await _Context.RefreshTokens.AddAsync(Token, Cancellation);

    public Task<RefreshToken?> GetByHashAsync(string TokenHash, CancellationToken Cancellation = default)
        => _Context.RefreshTokens.FirstOrDefaultAsync(Token => Token.TokenHash == TokenHash, Cancellation);

    public void Update(RefreshToken Token) => _Context.RefreshTokens.Update(Token);

    public async Task RevokeAllForUserAsync(
        long UserId,
        DateTime RevokedAt,
        CancellationToken Cancellation = default)
    {
        var Tokens = await _Context.RefreshTokens
            .Where(Token => Token.UserId == UserId && Token.RevokedAt == null)
            .ToListAsync(Cancellation);

        foreach (var Token in Tokens)
        {
            Token.RevokedAt = RevokedAt;
        }
    }
}
