using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_transactioncategory")]
[AddINotifyPropertyChangedInterface]
public sealed partial class TransactionCategory : Base
{
    [Column("name")]
    [StringLength(50)]
    [Display(Name = "Название категории")]
    [Order(1)]
    [Required(ErrorMessage = "Название категории обязательно.")]
    [Tooltip("Название категории транзакции.")]
    public string Name { get; set; } = null!;

    [Column("is_deleted")]
    [FormIgnore]
    [DataGridIgnore]
    public bool IsDeleted { get; set; }

    [Column("is_income")]
    [Display(Name = "Доход")]
    [Order(2)]
    [Required(ErrorMessage = "Поле доход обязательно.")]
    [Tooltip("Указывает, является ли категория доходной.")]
    public bool IsIncome { get; set; } =  false;
}