using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("token_blacklist_blacklistedtoken")]
[Index("TokenId", Name = "token_blacklist_blacklistedtoken_token_id_key", IsUnique = true)]
[AddINotifyPropertyChangedInterface]
public sealed partial class BlacklistedToken : Base
{
    [Column("blacklisted_at")]
    public DateTime BlacklistedAt { get; set; }

    [Column("token_id")]
    public long TokenId { get; set; }

    [ForeignKey("TokenId")]
    public OutstandingToken OutstandingToken { get; set; } = null!;
}
