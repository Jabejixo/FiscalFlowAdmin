using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("reminders_notification_profiles")]
[Index("NotificationId", "ProfileId", Name = "reminders_notification_p_notification_id_profile__a95e6e84_uniq", IsUnique = true)]
[Index("NotificationId", Name = "reminders_notification_profiles_notification_id_3654fc2b")]
[Index("ProfileId", Name = "reminders_notification_profiles_profile_id_b218a596")]
[AddINotifyPropertyChangedInterface]
public sealed partial class NotificationProfile : Base
{
    [Column("notification_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Уведомление")]
    [Tooltip("Связанный ID уведомления.")]
    public long NotificationId { get; set; }

    [Column("profile_id")]
    [FormIgnore]
    [DataGridIgnore]
    [Display(Name = "Профиль")]
    [Tooltip("Связанный ID профиля.")]
    public long ProfileId { get; set; }

    [ForeignKey("NotificationId")]
    [FormIgnore]
    [DataGridIgnore]
    [DisplayMemberPath("Subject")]
    public Notification Notification { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [FormIgnore]
    [DataGridIgnore]
    [DisplayMemberPath("FirstName")] // При необходимости скорректируйте на основе свойств Profile
    public Profile Profile { get; set; } = null!;
}