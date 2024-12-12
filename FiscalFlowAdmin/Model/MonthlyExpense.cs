using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_monthlyexpense")]
[Index("BillId", Name = "finances_ap_bill_id_746c78_idx")]
[Index("BillId", Name = "finances_app_monthlyexpense_bill_id_37680f98")]
[AddINotifyPropertyChangedInterface]
public sealed partial class MonthlyExpense : Base
{
    [Column("amount")]
    [Precision(12, 2)]
    [Display(Name = "Сумма")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(1)]
    [Required(ErrorMessage = "Сумма обязательна.")]
    [Tooltip("Сумма ежемесячного расхода.")]
    public decimal Amount { get; set; }

    [Column("is_deleted")]
    [FormIgnore]
    [DataGridIgnore]
    public bool IsDeleted { get; set; }

    [Column("next_payment_date")]
    [Display(Name = "Дата следующего платежа")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Order(2)]
    [Required(ErrorMessage = "Дата следующего платежа обязательна.")]
    [Tooltip("Дата следующего платежа.")]
    public DateOnly NextPaymentDate { get; set; }

    [Column("period")]
    [StringLength(10)]
    [Display(Name = "Период")]
    [Order(3)]
    [Required(ErrorMessage = "Период обязательный.")]
    [Tooltip("Период платежа (например, ежемесячно, ежеквартально).")]
    public string Period { get; set; } = null!;

    [Column("bill_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Счет")]
    [Tooltip("Связанный ID счета.")]
    public long BillId { get; set; }

    [ForeignKey("BillId")]
    [DisplayMemberPath("Balance")] // При необходимости скорректируйте на основе свойств Bill
    public Bill Bill { get; set; } = null!;
}