using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("authentication_devicetoken")]
[Index("ProfileId", Name = "authentication_devicetoken_profile_id_0fbaea76")]
[AddINotifyPropertyChangedInterface]
public sealed partial class DeviceToken : Base
{
    [Column("token")]
    [StringLength(255)]
    public string Token { get; set; } = null!;

    [Column("profile_id")]
    public long ProfileId { get; set; }

    [ForeignKey("ProfileId")]
    public Profile Profile { get; set; } = null!;
}
