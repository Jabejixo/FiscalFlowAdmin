using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("finances_app_bill")]
[Index("CurrencyId", Name = "finances_ap_currenc_17b09c_idx")]
[Index("ProfileId", Name = "finances_ap_profile_a16c59_idx")]
[Index("CurrencyId", Name = "finances_app_bill_currency_id_88fc4021")]
[Index("ProfileId", Name = "finances_app_bill_profile_id_f1838735")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Bill : Base
{
    [Column("is_main")]
    [Display(Name = "Основной счет")]
    [Order(1)]
    [Tooltip("Указывает, является ли это основной счет.")]
    public bool IsMain { get; set; }

    [Column("balance")]
    [Precision(12, 2)]
    [Display(Name = "Баланс")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Order(2)]
    [Required(ErrorMessage = "Баланс обязателен.")]
    [Tooltip("Текущий баланс счета.")]
    public decimal Balance { get; set; }

    [Column("is_deleted")]
    [FormIgnore]
    [DataGridIgnore]
    public bool? IsDeleted { get; set; }

    [Column("profile_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Профиль")]
    [Tooltip("Связанный ID профиля.")]
    public long ProfileId { get; set; }

    [Column("currency_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Валюта")]
    [Tooltip("Валюта, связанная со счетом.")]
    public long CurrencyId { get; set; }

    [ForeignKey("CurrencyId")]
    [DisplayMemberPath("Name")]
    [Display(Name = "Валюта")]
    [Tooltip("Чей профиль")]
    public Currency Currency { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [Display(Name = "Профиль пользователя")]
    [Tooltip("Чей счет")]
    [DisplayMemberPath("User.Email")] 
    public Profile Profile { get; set; } = null!;
}