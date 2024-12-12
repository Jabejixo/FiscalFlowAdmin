using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("authentication_profile")]
[Index("UserId", Name = "authentication_profile_user_id_key", IsUnique = true)]
[AddINotifyPropertyChangedInterface]
public sealed partial class Profile : Base
{
    [Column("image")]
    [StringLength(100)]
    public string? Image { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
