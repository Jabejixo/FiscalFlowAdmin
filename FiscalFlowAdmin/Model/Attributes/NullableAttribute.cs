namespace FiscalFlowAdmin.Model.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NullableAttribute : Attribute
{
    public bool AllowNull { get; }

    public NullableAttribute(bool allowNull = true)
    {
        AllowNull = allowNull;
    }
}