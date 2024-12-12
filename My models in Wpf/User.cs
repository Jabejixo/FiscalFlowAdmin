using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("authentication_user")]
[Index("Email", Name = "authentication_user_email_key", IsUnique = true)]
[AddINotifyPropertyChangedInterface]
public sealed partial class User : Base
{
    [Column("password")]
    [StringLength(128)]
    public string Password { get; set; } = null!;

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("is_superuser")]
    public bool IsSuperuser { get; set; }

    [Column("email")]
    [StringLength(254)]
    public string Email { get; set; } = null!;

    [Column("first_name")]
    [StringLength(30)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [StringLength(30)]
    public string? LastName { get; set; }

    [Column("birthday")]
    public DateOnly? Birthday { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_confirmed_email")]
    public bool? IsConfirmedEmail { get; set; }

    [Column("is_staff")]
    public bool IsStaff { get; set; }
    
}

