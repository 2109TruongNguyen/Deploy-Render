using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NZWalksAPI.Models.Domain;

public class Role : IdentityRole<Guid>
{
    public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
}