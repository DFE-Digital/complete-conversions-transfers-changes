namespace Dfe.Complete.Utils.Attributes;

using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class DisplayDescriptionAttribute(string displayDescription) : Attribute
{
    public string DisplayDescription { get; } = displayDescription;
}
