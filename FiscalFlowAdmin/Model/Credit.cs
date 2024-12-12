using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_credit")]
[Index("BillId", Name = "finances_ap_bill_id_444031_idx")]
[Index("BillId", Name = "finances_app_credit_bill_id_d22a52cd")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Credit : Base
{
    [Column("amount")]
    [Precision(12, 2)]
    [Display(Name = "Сумма")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(1)]
    [Required(ErrorMessage = "Сумма обязательна.")]
    [Tooltip("Общая сумма кредита.")]
    public decimal Amount { get; set; }

    [Column("term")]
    [Display(Name = "Срок (месяцы)")]
    [Order(2)]
    [Required(ErrorMessage = "Срок обязательен.")]
    [Tooltip("Срок кредита в месяцах.")]
    public int Term { get; set; }

    [Column("interest_rate")]
    [Precision(5, 2)]
    [Display(Name = "Процентная ставка (%)")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(3)]
    [Required(ErrorMessage = "Процентная ставка обязательна.")]
    [Tooltip("Процентная ставка по кредиту.")]
    public decimal InterestRate { get; set; }

    [Column("paid_amount")]
    [Precision(12, 2)]
    [Display(Name = "Оплачено")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(4)]
    [Tooltip("Сумма, уже выплаченная.")]
    public decimal PaidAmount { get; set; }

    [Column("remaining_amount")]
    [Precision(12, 2)]
    [Display(Name = "Оставшаяся сумма")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(5)]
    [Tooltip("Сумма, остающаяся к выплате.")]
    public decimal RemainingAmount { get; set; }

    [Column("is_deleted")]
    [FormIgnore]
    [DataGridIgnore]
    public bool IsDeleted { get; set; }

    [Column("bill_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Счет")]
    [Tooltip("Связанный ID счета.")]
    public long BillId { get; set; }

    [ForeignKey("BillId")]
    [FormIgnore]
    [DataGridIgnore]
    [DisplayMemberPath("Id")] // При необходимости скорректируйте на основе свойств Bill
    public Bill Bill { get; set; } = null!;
}
