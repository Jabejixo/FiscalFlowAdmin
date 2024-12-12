using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("token_blacklist_outstandingtoken")]
[Index("Jti", Name = "token_blacklist_outstandingtoken_jti_hex_d9bdf6f7_uniq", IsUnique = true)]
[Index("UserId", Name = "token_blacklist_outstandingtoken_user_id_83bc629a")]
[AddINotifyPropertyChangedInterface]
public sealed partial class OutstandingToken : Base
{
  
    [Column("token")]
    public string Token { get; set; } = null!;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("user_id")]
    public long? UserId { get; set; }

    [Column("jti")]
    [StringLength(255)]
    public string Jti { get; set; } = null!;
    
    public BlacklistedToken? TokenBlacklistBlacklistedtoken { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
