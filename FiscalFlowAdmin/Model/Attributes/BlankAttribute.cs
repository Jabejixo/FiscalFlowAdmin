namespace FiscalFlowAdmin.Model.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class BlankAttribute : Attribute
{
    public bool AllowBlank { get; }

    public BlankAttribute(bool allowBlank = true)
    {
        AllowBlank = allowBlank;
    }
}