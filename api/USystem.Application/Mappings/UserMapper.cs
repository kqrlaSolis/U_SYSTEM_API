// Mapeo de entidades a DTOs del módulo Users.
using USystem.Application.Dtos;
using USystem.Domain.Entities;

namespace USystem.Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(
        User User,
        IReadOnlyList<string>? Roles = null,
        IReadOnlyList<string>? Permissions = null) => new()
        {
            Id = User.Id,
            FirstName = User.FirstName,
            PaternalLastName = User.PaternalLastName,
            MaternalLastName = User.MaternalLastName,
            FullName = BuildFullName(User),
            Email = User.Email,
            Username = User.Username,
            Phone = User.Phone,
            IsActive = User.IsActive,
            MustChangePassword = User.MustChangePassword,
            CreatedAt = User.CreatedAt,
            Roles = Roles?.ToList() ?? new List<string>(),
            Permissions = Permissions?.ToList() ?? new List<string>(),
        };

    public static string BuildFullName(User User)
        => string.Join(
            ' ',
            new[] { User.FirstName, User.PaternalLastName, User.MaternalLastName }
                .Where(Part => !string.IsNullOrWhiteSpace(Part)));
}
