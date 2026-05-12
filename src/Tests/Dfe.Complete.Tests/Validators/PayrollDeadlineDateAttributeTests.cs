using Dfe.Complete.Validators;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dfe.Complete.Tests.Validators
{
    public class PayrollDeadlineDateAttributeTests
    {
        private class TestModel
        {
            [Display(Name = "Payroll deadline date")]
            [PayrollDeadlineDate("SignificantDate")]
            public DateOnly? PayrollDeadline { get; set; }
            public DateOnly? SignificantDate { get; set; }
        }

        private static ValidationContext GetContext(TestModel model)
        {
            return new ValidationContext(model, null, null)
            {
                MemberName = nameof(TestModel.PayrollDeadline)
            };
        }

        [Fact]
        public void ReturnsSuccess_WhenDateIsNull()
        {
            var model = new TestModel { PayrollDeadline = null, SignificantDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)) };
            var attr = new PayrollDeadlineDateAttribute("SignificantDate");
            var result = attr.GetValidationResult(model.PayrollDeadline, GetContext(model));
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsError_WhenDateIsInThePastOrToday()
        {
            var model = new TestModel { PayrollDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), SignificantDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)) };
            var attr = new PayrollDeadlineDateAttribute("SignificantDate");
            var result = attr.GetValidationResult(model.PayrollDeadline, GetContext(model));
            Assert.NotNull(result);
            Assert.Contains("must be in the future", result.ErrorMessage);

            model.PayrollDeadline = DateOnly.FromDateTime(DateTime.Now);
            result = attr.GetValidationResult(model.PayrollDeadline, GetContext(model));
            Assert.NotNull(result);
            Assert.Contains("must be in the future", result.ErrorMessage);
        }

        [Fact]
        public void ReturnsError_WhenDateIsOnOrAfterSignificantDate()
        {
            var future = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
            var model = new TestModel { PayrollDeadline = future, SignificantDate = future };
            var attr = new PayrollDeadlineDateAttribute("SignificantDate");
            var result = attr.GetValidationResult(model.PayrollDeadline, GetContext(model));
            Assert.NotNull(result);
            Assert.Contains("must be before the significant date", result.ErrorMessage);

            model.PayrollDeadline = future.AddDays(1);
            result = attr.GetValidationResult(model.PayrollDeadline, GetContext(model));
            Assert.NotNull(result);
            Assert.Contains("must be before the significant date", result.ErrorMessage);
        }

        [Fact]
        public void ReturnsSuccess_WhenDateIsValid()
        {
            var model = new TestModel
            {
                PayrollDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                SignificantDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10))
            };
            var attr = new PayrollDeadlineDateAttribute("SignificantDate");
            var result = attr.GetValidationResult(model.PayrollDeadline, GetContext(model));
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsSuccess_WhenSignificantDatePropertyDoesNotExist()
        {
            var model = new { PayrollDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(2)) };
            var attr = new PayrollDeadlineDateAttribute("NonExistentProperty");
            var context = new ValidationContext(model, null, null) { MemberName = "PayrollDeadline" };
            var result = attr.GetValidationResult(model.PayrollDeadline, context);
            Assert.Null(result);
        }
    }
}
