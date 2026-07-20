using Dfe.Complete.Pages.Projects.TaskList.Tasks;
using FluentAssertions;

namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks;

public class CheckboxSelectionHelperTests
{
    [Fact]
    public void IsSelected_WhenOptionsIsNull_ReturnsFalse()
    {
        var result = CheckboxSelectionHelper.IsSelected(null, "confirm-number");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsSelected_WhenOptionExistsWithDifferentCasing_ReturnsTrue()
    {
        List<string> selectedOptions = ["Confirm-Number", "send-form"];

        var result = CheckboxSelectionHelper.IsSelected(selectedOptions, "confirm-number");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSelected_WhenOptionDoesNotExist_ReturnsFalse()
    {
        List<string> selectedOptions = ["confirm-published-number", "send-form"];

        var result = CheckboxSelectionHelper.IsSelected(selectedOptions, "confirm-number");

        result.Should().BeFalse();
    }

    [Fact]
    public void BuildSelectedOptions_WhenMappingsContainTrueFalseAndNull_IncludesOnlyTrueOptionsInOrder()
    {
        var result = CheckboxSelectionHelper.BuildSelectedOptions(
            ("not-applicable", true),
            ("confirm-published-number", false),
            ("confirm-number", null),
            ("check-returned-form", true),
            ("send-form", false));

        result.Should().Equal("not-applicable", "check-returned-form");
    }

    [Fact]
    public void BuildSelectedOptions_WhenNoMappingsAreTrue_ReturnsEmptyList()
    {
        var result = CheckboxSelectionHelper.BuildSelectedOptions(
            ("not-applicable", false),
            ("confirm-number", null),
            ("send-form", false));

        result.Should().BeEmpty();
    }
}
