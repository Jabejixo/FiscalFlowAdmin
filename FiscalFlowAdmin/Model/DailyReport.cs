using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_dailyreport")]
[Index("ProfileId", Name = "finances_ap_profile_af7197_idx")]
[Index("ProfileId", Name = "finances_app_dailyreport_profile_id_cdac02f8")]
[AddINotifyPropertyChangedInterface]
public sealed partial class DailyReport : Base
{
    [Column("date")]
    [Display(Name = "Дата отчета")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Order(1)]
    [Required(ErrorMessage = "Дата обязательна.")]
    [Tooltip("Дата ежедневного отчета.")]
    public DateOnly Date { get; set; }

    [Column("total_income")]
    [Precision(12, 2)]
    [Display(Name = "Общий доход")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(2)]
    [Required(ErrorMessage = "Общий доход обязателен.")]
    [Tooltip("Общий доход за день.")]
    public decimal TotalIncome { get; set; }

    [Column("total_expense")]
    [Precision(12, 2)]
    [Display(Name = "Общие расходы")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(3)]
    [Required(ErrorMessage = "Общие расходы обязательны.")]
    [Tooltip("Общие расходы за день.")]
    public decimal TotalExpense { get; set; }

    [Column("is_deleted")]
    [FormIgnore]
    [DataGridIgnore]
    public bool IsDeleted { get; set; }

    [Column("profile_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Профиль")]
    [Tooltip("Связанный ID профиля.")]
    public long ProfileId { get; set; }

    [InverseProperty("DailyReport")]
    [FormIgnore]
    [DataGridIgnore]
    public ICollection<DailyCategoryExpense> FinancesAppDailycategoryexpenses { get; set; } = new List<DailyCategoryExpense>();

    [ForeignKey("ProfileId")]
    [FormIgnore]
    [DataGridIgnore]
    [DisplayMemberPath("FirstName")] // При необходимости скорректируйте на основе свойств Profile
    public Profile Profile { get; set; } = null!;
}
