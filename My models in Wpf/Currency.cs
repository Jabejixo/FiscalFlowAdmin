using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_currency")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Currency : Base
{
    [Column("name")]
    [StringLength(5)]
    public string Name { get; set; } = null!;

    [Column("value")]
    [Precision(12, 2)]
    public decimal Value { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
    
}
