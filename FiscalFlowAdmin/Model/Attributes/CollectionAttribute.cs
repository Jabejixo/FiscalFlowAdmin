namespace FiscalFlowAdmin.Model.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CollectionAttribute : Attribute
{
    public string DisplayMember { get; }

    public CollectionAttribute(string displayMember)
    {
        DisplayMember = displayMember;
    }
}