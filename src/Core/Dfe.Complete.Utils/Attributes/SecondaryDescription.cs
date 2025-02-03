namespace Dfe.Complete.Utils.Attributes;

using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class SecondaryDescriptionAttribute(string secondaryDescription) : Attribute
{
    public string SecondaryDescription { get; } = secondaryDescription;
}
