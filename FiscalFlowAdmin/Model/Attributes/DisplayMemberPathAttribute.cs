namespace FiscalFlowAdmin.Model.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayMemberPathAttribute : Attribute
{
    public string PropertyName { get; }

    public DisplayMemberPathAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }
}