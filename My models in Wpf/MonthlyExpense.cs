using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_monthlyexpense")]
[Index("BillId", Name = "finances_ap_bill_id_746c78_idx")]
[Index("BillId", Name = "finances_app_monthlyexpense_bill_id_37680f98")]
[AddINotifyPropertyChangedInterface]
public sealed partial class MonthlyExpense : Base
{
    [Column("amount")]
    [Precision(12, 2)]
    public decimal Amount { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("next_payment_date")]
    public DateOnly NextPaymentDate { get; set; }

    [Column("period")]
    [StringLength(10)]
    public string Period { get; set; } = null!;

    [Column("bill_id")]
    public long BillId { get; set; }

    [ForeignKey("BillId")]
    public Bill Bill { get; set; } = null!;
}
