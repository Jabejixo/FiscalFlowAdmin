using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_transaction")]
[Index("BillId", Name = "finances_ap_bill_id_f7f665_idx")]
[Index("CategoryId", Name = "finances_ap_categor_75baae_idx")]
[Index("BillId", Name = "finances_app_transaction_bill_id_9a0a9218")]
[Index("CategoryId", Name = "finances_app_transaction_category_id_113b376f")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Transaction : Base
{
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("amount")]
    [Precision(12, 2)]
    public decimal Amount { get; set; }

    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("bill_id")]
    public long BillId { get; set; }

    [Column("category_id")]
    public long CategoryId { get; set; }

    [ForeignKey("BillId")]
    public Bill Bill { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public TransactionCategory Category { get; set; } = null!;
}
