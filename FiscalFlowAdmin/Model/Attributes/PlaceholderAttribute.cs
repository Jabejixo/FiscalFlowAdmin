namespace FiscalFlowAdmin.Model.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PlaceholderAttribute : Attribute
{
    public string Text { get; }

    public PlaceholderAttribute(string text)
    {
        Text = text;
    }
}