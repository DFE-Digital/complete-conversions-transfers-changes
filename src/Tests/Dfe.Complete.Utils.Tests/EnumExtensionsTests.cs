using Dfe.Complete.Utils.Attributes;
using FluentAssertions;
using Xunit;

namespace Dfe.Complete.Utils.Tests;

public class EnumExtensionsTests
{
    private enum TestEnum
    {
        [DisplayDescription("First Value Display")]
        FirstValue,

        [DisplayDescription("Second Value Display")]
        SecondValue,

        ValueWithoutAttribute
    }

    [Fact]
    public void ToDisplayDescription_WithNullSource_ReturnsEmptyString()
    {
        TestEnum? nullEnum = null;

        var result = nullEnum.ToDisplayDescription();

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ToDisplayDescription_WithDisplayDescriptionAttribute_ReturnsAttributeValue()
    {
        var enumValue = TestEnum.FirstValue;

        var result = enumValue.ToDisplayDescription();

        result.Should().Be("First Value Display");
    }

    [Fact]
    public void ToDisplayDescription_WithoutDisplayDescriptionAttribute_ReturnsEnumName()
    {
        var enumValue = TestEnum.ValueWithoutAttribute;

        var result = enumValue.ToDisplayDescription();

        result.Should().Be("ValueWithoutAttribute");
    }

    [Fact]
    public void ToDisplayDescription_WithDifferentEnumValues_ReturnsCorrectDescriptions()
    {
        TestEnum.FirstValue.ToDisplayDescription().Should().Be("First Value Display");
        TestEnum.SecondValue.ToDisplayDescription().Should().Be("Second Value Display");
        TestEnum.ValueWithoutAttribute.ToDisplayDescription().Should().Be("ValueWithoutAttribute");
    }
}