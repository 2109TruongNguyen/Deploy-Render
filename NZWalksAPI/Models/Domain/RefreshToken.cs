using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.Domain;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; } = false;
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}