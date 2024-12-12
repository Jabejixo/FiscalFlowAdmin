using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using FiscalFlowAdmin.Model.Attributes;

namespace FiscalFlowAdmin.Model;

[Table("reminders_notification")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Notification : Base
{
    [Column("subject")]
    [StringLength(255)]
    [Display(Name = "Тема")]
    [Order(1)]
    [Required(ErrorMessage = "Тема обязательна.")]
    [Tooltip("Тема уведомления.")]
    public string Subject { get; set; } = null!;

    [Column("description")]
    [Display(Name = "Описание")]
    [Order(2)]
    [Tooltip("Подробное описание уведомления.")]
    public string? Description { get; set; }

    [Column("send_time")]
    [Display(Name = "Время отправки")]
    [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
    [Order(3)]
    [Tooltip("Время, когда уведомление должно быть отправлено.")]
    public TimeOnly? SendTime { get; set; }
}