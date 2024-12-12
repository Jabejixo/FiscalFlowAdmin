using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FiscalFlowAdmin.Model;

[Table("authentication_profile")]
[Index("UserId", Name = "authentication_profile_user_id_key", IsUnique = true)]
[AddINotifyPropertyChangedInterface]
public sealed partial class Profile : Base
{
    [Column("image")]
    [StringLength(100)]
    [Display(Name = "Изображение профиля")]
    [Order(1)]
    [Tooltip("URL или путь к изображению профиля.")]
    public string? Image { get; set; }

    [Column("user_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Пользователь")]
    [Tooltip("Связанный ID пользователя.")]
    public long UserId { get; set; }

    [ForeignKey("UserId")]
    [DisplayMemberPath("Email")]
    [Display(Name = "Пользователь")]
    [Tooltip("Чей профиль")]
    
    public User User { get; set; } = null!;
}