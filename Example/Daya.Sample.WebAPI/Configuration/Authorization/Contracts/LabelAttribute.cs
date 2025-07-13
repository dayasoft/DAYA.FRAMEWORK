[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class LabelAttribute : Attribute
{
    public LabelAttribute(
        string value,
        string localization)
    {
        Label = value;
        Localization = localization;
    }

    public string Label { get; set; }
    public string Localization { get; set; }
}