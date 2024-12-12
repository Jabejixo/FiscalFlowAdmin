namespace FiscalFlowAdmin.Model.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class OrderAttribute : Attribute
{
    public int Order { get; }

    public OrderAttribute(int order)
    {
        Order = order;
    }
}