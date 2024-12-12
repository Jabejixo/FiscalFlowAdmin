using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_debt")]
[Index("BillId", Name = "finances_ap_bill_id_83d405_idx")]
[Index("BillId", Name = "finances_app_debt_bill_id_c93ab690")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Debt : Base
{
    [Column("amount")]
    [Precision(12, 2)]
    [Display(Name = "Сумма")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(1)]
    [Required(ErrorMessage = "Сумма обязательна.")]
    [Tooltip("Сумма долга.")]
    public decimal Amount { get; set; }

    [Column("debtor")]
    [StringLength(30)]
    [Display(Name = "Должник")]
    [Order(2)]
    [Required(ErrorMessage = "Должник обязателен.")]
    [Tooltip("Имя должника.")]
    public string Debtor { get; set; } = null!;

    [Column("description")]
    [Display(Name = "Описание")]
    [Order(3)]
    [Required(ErrorMessage = "Описание обязательно.")]
    [Tooltip("Описание долга.")]
    public string Description { get; set; } = null!;

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