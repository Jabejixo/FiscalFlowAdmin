using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_dailycategoryexpense")]
[Index("CategoryId", Name = "finances_ap_categor_1843f5_idx")]
[Index("DailyReportId", Name = "finances_ap_daily_r_36cd3e_idx")]
[Index("CategoryId", Name = "finances_app_dailycategoryexpense_category_id_c1b2af05")]
[Index("DailyReportId", Name = "finances_app_dailycategoryexpense_daily_report_id_ee07bcdc")]
[AddINotifyPropertyChangedInterface]
public sealed partial class DailyCategoryExpense : Base
{
    [Column("expense_amount")]
    [Precision(12, 2)]
    public decimal ExpenseAmount { get; set; }

    [Column("daily_report_id")]
    public long DailyReportId { get; set; }

    [Column("category_id")]
    public long CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public TransactionCategory Category { get; set; } = null!;

    [ForeignKey("DailyReportId")]
    public DailyReport DailyReport { get; set; } = null!;
}
