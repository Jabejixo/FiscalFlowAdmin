using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_transactioncategory")]
[AddINotifyPropertyChangedInterface]
public sealed partial class TransactionCategory : Base
{
    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("is_income")]
    public bool IsIncome { get; set; }

}
