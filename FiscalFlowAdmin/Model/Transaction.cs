using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

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
    [FormIgnore]
    [DataGridIgnore]
    public bool IsDeleted { get; set; }

    [Column("amount")]
    [Precision(12, 2)]
    [Display(Name = "Сумма")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(1)]
    [Required(ErrorMessage = "Сумма обязательна.")]
    [Tooltip("Сумма транзакции.")]
    public decimal Amount { get; set; }

    [Column("date")]
    [Display(Name = "Дата транзакции")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Order(2)]
    [Required(ErrorMessage = "Дата транзакции обязательна.")]
    [Tooltip("Дата транзакции.")]
    public DateOnly Date { get; set; }

    [Column("bill_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Счет")]
    [Tooltip("Связанный ID счета.")]
    public long BillId { get; set; }

    [Column("category_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Категория")]
    [Tooltip("Связанный ID категории.")]
    public long CategoryId { get; set; }

    [ForeignKey("BillId")]
    [Display(Name = "Счет")]
    [Tooltip("Чей счет")]
    [DisplayMemberPath("Profile")] // При необходимости скорректируйте на основе свойств Bill
    public Bill Bill { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [Display(Name = "Категория транзакции")]
    [Tooltip("Укажите категорию транзакции")]
    [DisplayMemberPath("Name")]
    public TransactionCategory Category { get; set; } = null!;
}