using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("authentication_devicetoken")]
[Index("ProfileId", Name = "authentication_devicetoken_profile_id_0fbaea76")]
[AddINotifyPropertyChangedInterface]
public sealed partial class DeviceToken : Base
{
    [Column("token")]
    [StringLength(255)]
    [Display(Name = "Токен устройства")]
    [Order(1)]
    [Required(ErrorMessage = "Токен обязателен.")]
    [Tooltip("Токен аутентификации для устройства.")]
    public string Token { get; set; } = null!;

    [Column("profile_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Профиль")]
    [Tooltip("Связанный ID профиля.")]
    public long ProfileId { get; set; }

    [ForeignKey("ProfileId")]
    [FormIgnore]
    [DataGridIgnore]
    public Profile Profile { get; set; } = null!;
}