using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

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
    [Display(Name = "Сумма расхода")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(1)]
    [Required(ErrorMessage = "Сумма расхода обязательна.")]
    [Tooltip("Сумма расхода по категории.")]
    public decimal ExpenseAmount { get; set; }

    [Column("daily_report_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Ежедневный отчет")]
    [Tooltip("Связанный ID ежедневного отчета.")]
    public long DailyReportId { get; set; }

    [Column("category_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Категория")]
    [Tooltip("Связанный ID категории.")]
    public long CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [FormIgnore]
    [DataGridIgnore]
    [DisplayMemberPath("Name")]
    public TransactionCategory Category { get; set; } = null!;

    [ForeignKey("DailyReportId")]
    [FormIgnore]
    [DataGridIgnore]
    [DisplayMemberPath("Date")]
    public DailyReport DailyReport { get; set; } = null!;
}