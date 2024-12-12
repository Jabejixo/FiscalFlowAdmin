using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace FiscalFlowAdmin.Model;

[Table("reminders_notification")]
[AddINotifyPropertyChangedInterface]
public sealed partial class Notification : Base
{

    [Column("subject")]
    [StringLength(255)]
    public string Subject { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("send_time")]
    public TimeOnly? SendTime { get; set; }
}
