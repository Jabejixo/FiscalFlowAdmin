using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_dailyreport")]
[Index("ProfileId", Name = "finances_ap_profile_af7197_idx")]
[Index("ProfileId", Name = "finances_app_dailyreport_profile_id_cdac02f8")]
[AddINotifyPropertyChangedInterface]
public sealed partial class DailyReport : Base
{
    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("total_income")]
    [Precision(12, 2)]
    public decimal TotalIncome { get; set; }

    [Column("total_expense")]
    [Precision(12, 2)]
    public decimal TotalExpense { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("profile_id")]
    public long ProfileId { get; set; }

    [InverseProperty("DailyReport")]
    public ICollection<DailyCategoryExpense> FinancesAppDailycategoryexpenses { get; set; } = new List<DailyCategoryExpense>();

    [ForeignKey("ProfileId")]
    public Profile Profile { get; set; } = null!;
}
