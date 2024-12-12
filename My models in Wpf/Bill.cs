using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_bill")]
[Index("CurrencyId", Name = "finances_ap_currenc_17b09c_idx")]
[Index("ProfileId", Name = "finances_ap_profile_a16c59_idx")]
[Index("CurrencyId", Name = "finances_app_bill_currency_id_88fc4021")]
[Index("ProfileId", Name = "finances_app_bill_profile_id_f1838735")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Bill : Base
{
    [Column("is_main")]
    public bool IsMain { get; set; }

    [Column("balance")]
    [Precision(12, 2)]
    public decimal Balance { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("profile_id")]
    public long ProfileId { get; set; }

    [Column("currency_id")]
    public long CurrencyId { get; set; }

    [ForeignKey("CurrencyId")]
    public Currency Currency { get; set; } = null!;

    [ForeignKey("ProfileId")]
    public Profile Profile { get; set; } = null!;
}
