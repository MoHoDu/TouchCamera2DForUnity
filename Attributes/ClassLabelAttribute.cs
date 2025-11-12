using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ClassLabelAttribute : Attribute
{
    public string DisplayName { get; }

    public ClassLabelAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}
