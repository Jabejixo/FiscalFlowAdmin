using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_currency")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Currency : Base
{
    [Column("name")]
    [StringLength(5)]
    [Display(Name = "Название валюты")]
    [Order(1)]
    [Required(ErrorMessage = "Название валюты обязательно.")]
    [Tooltip("Краткое название валюты (например, USD, EUR).")]
    public string Name { get; set; } = null!;

    [Column("value")]
    [Precision(12, 2)]
    [Display(Name = "Значение валюты")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(2)]
    [Required(ErrorMessage = "Значение валюты обязательно.")]
    [Tooltip("Значение валюты относительно базовой валюты.")]
    public decimal Value { get; set; }

    [Column("is_deleted")]
    [FormIgnore]
    [DataGridIgnore]
    public bool IsDeleted { get; set; }
}