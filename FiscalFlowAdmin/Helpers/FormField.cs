using System.ComponentModel.DataAnnotations;

namespace FiscalFlowAdmin.Helpers;

public class FormField
{
    public string PropertyName { get; set; }
    public string DisplayName { get; set; }
    public Type PropertyType { get; set; }
    public object Value { get; set; }
    public int Order { get; set; }
    public IEnumerable<ValidationAttribute> ValidationAttributes { get; set; }
} 