using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Domain.Models.User;

public class User:IdentityUser<Guid>
{    
    [Required]
    [StringLength(50)]
    public string Login { get; set; }
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    [StringLength(50)]
    public string Patronymic { get; set; }

    public string Salt { get; set; }
    public bool IsTemporaryPassword { get; set; }
    public bool IsBlocked { get; set; } = false;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? LastForgot { get; set; }
}