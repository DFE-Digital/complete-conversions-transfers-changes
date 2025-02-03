using Dfe.Complete.Application.Projects.Queries.GetProject;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Dfe.Complete.Validators;

public partial class TrnAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var trn = value as string;

        if (string.IsNullOrEmpty(trn))
            return new ValidationResult("Enter a Trust reference number (TRN)");

        if (TrnRegex().IsMatch(trn) == false)
            return new ValidationResult("The Trust reference number must be 'TR' followed by 5 numbers, e.g. TR01234");

        var sender = (ISender)validationContext.GetService(typeof(ISender));
        var result = sender.Send(new GetProjectByTrnQuery(trn));

        if (!result.Result.IsSuccess)
        {
            throw new Exception(result.Result.Error);
        }

        var existingProject = result.Result.Value;

        if (existingProject != null && !string.IsNullOrEmpty(existingProject.NewTrustName))
        {
            return new ValidationResult($"A trust with this TRN already exists. It is called {existingProject.NewTrustName}. Check the trust name you have entered for this conversion/transfer");
        }

        return ValidationResult.Success;    
    }

    [GeneratedRegex("^TR\\d{5}$")]
    private static partial Regex TrnRegex();
}