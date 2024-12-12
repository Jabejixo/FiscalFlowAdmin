using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using FiscalFlowAdmin.Model.Attributes;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlowAdmin.Model;
[Table("audit_actionlog")]
public class ActionLog : Base
{
    [Required]
    [MaxLength(10)]
    [Column("action")]
    [Display(Name = "Действие")]
    public string Action { get; set; }  // action varchar

    [ForeignKey("User")]
    [Column("user_id")]
    [Display(Name = "Пользователь")]
    [FormIgnore]
    public long? UserId { get; set; }  // user_id
    
    [Display(Name = "Действие")]
    [Column("content_type_id")]
    public int ContentTypeId { get; set; }  // content_type_id

    [MaxLength(255)]
    [Display(Name = "Объект")]
    [Column("object_id")]
    public string ObjectId { get; set; }  // object_id varchar

    [MaxLength(200)]
    [Column("object_repr")]
    [Display(Name = "Что произошло")]
    public string ObjectRepr { get; set; }  // object_repr varchar

    [Column("changes")] 
    [Display(Name = "Изменения")]
    public string? Changes { get; set; }  // changes jsonField

    [Column("timestamp")]
    [Display(Name = "Дата")]
    public DateTime Timestamp { get; set; }  // timestamp with time zone
   
}

