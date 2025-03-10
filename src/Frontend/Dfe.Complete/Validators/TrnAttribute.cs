using Dfe.Complete.Application.Projects.Queries.GetProject;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public partial class TrnAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        try
        {
            var trn = value as string;

            if (string.IsNullOrEmpty(trn))
                return new ValidationResult("Enter a Trust reference number (TRN)");

            if (!TrnRegex().IsMatch(trn))
                return new ValidationResult(
                    "The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234");

            var sender = (ISender)validationContext.GetService(typeof(ISender));
            var result = sender.Send(new GetProjectByTrnQuery(trn));

            if (!result.Result.IsSuccess)
                throw new NotFoundException(result.Result.Error);

            var existingProject = result.Result.Value;

            if (existingProject != null && !string.IsNullOrEmpty(existingProject.NewTrustName))
            {
                return new ValidationResult(
                    $"A trust with this TRN already exists. It is called {existingProject.NewTrustName}. Check the trust name you have entered for this conversion/transfer");
            }

            return ValidationResult.Success;
        }
        catch (NotFoundException notFoundException)
        {
            return new ValidationResult(notFoundException.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [GeneratedRegex("^TR\\d{5}$")]
    private static partial Regex TrnRegex();
}