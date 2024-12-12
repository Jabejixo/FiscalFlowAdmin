using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_debt")]
[Index("BillId", Name = "finances_ap_bill_id_83d405_idx")]
[Index("BillId", Name = "finances_app_debt_bill_id_c93ab690")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Debt : Base
{
    [Column("amount")]
    [Precision(12, 2)]
    public decimal Amount { get; set; }

    [Column("debtor")]
    [StringLength(30)]
    public string Debtor { get; set; } = null!;

    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [ForeignKey("BillId")]
    public Bill Bill { get; set; } = null!;
}
