using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_credit")]
[Index("BillId", Name = "finances_ap_bill_id_444031_idx")]
[Index("BillId", Name = "finances_app_credit_bill_id_d22a52cd")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Credit : Base
{
    [Column("amount")]
    [Precision(12, 2)]
    public decimal Amount { get; set; }

    [Column("term")]
    public int Term { get; set; }

    [Column("interest_rate")]
    [Precision(5, 2)]
    public decimal InterestRate { get; set; }

    [Column("paid_amount")]
    [Precision(12, 2)]
    public decimal PaidAmount { get; set; }

    [Column("remaining_amount")]
    [Precision(12, 2)]
    public decimal RemainingAmount { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [ForeignKey("BillId")]
    public Bill Bill { get; set; } = null!;
}
