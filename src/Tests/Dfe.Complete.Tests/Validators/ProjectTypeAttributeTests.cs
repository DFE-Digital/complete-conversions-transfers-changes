using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Validators;

namespace Dfe.Complete.Tests.Validators
{
    public class ProjectTypeAttributeTests
    {
        private class ValidateProjectTypeTestModel
        {
            [ProjectType]
            public ProjectType? ProjectType { get; set; }
        }

        private static bool IsValid(ValidateProjectTypeTestModel model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = [];
            return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        }

        [Theory]
        [InlineData(ProjectType.Conversion)]
        [InlineData(ProjectType.Transfer)]
        public void ValidProjectTypes_ShouldPassValidation(ProjectType projectType)
        {
            var model = new ValidateProjectTypeTestModel { ProjectType = projectType };

            var isValid = IsValid(model, out var results);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        // Any value that isn't Conversion or Transfer should fail; using an out-of-range enum value is safe here.
        [InlineData((ProjectType)123)]
        public void InvalidProjectTypes_ShouldFailValidation(ProjectType projectType)
        {
            var model = new ValidateProjectTypeTestModel { ProjectType = projectType };

            var isValid = IsValid(model, out var results);

            Assert.False(isValid);
            Assert.Single(results);
            Assert.Equal(ValidationConstants.ProjectTypeValidationMessage, results[0].ErrorMessage);
        }
    }
}