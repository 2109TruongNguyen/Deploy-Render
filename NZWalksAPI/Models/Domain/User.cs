using Microsoft.AspNetCore.Identity;

namespace NZWalksAPI.Models.Domain;

public class User : IdentityUser<Guid>
{
    public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}