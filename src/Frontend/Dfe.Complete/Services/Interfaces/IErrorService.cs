using Microsoft.AspNetCore.Mvc.ModelBinding;
using Dfe.Complete.Models;

namespace Dfe.Complete.Services.Interfaces
{
    public interface IErrorService
    {
        void AddError(string key, string message);
        void AddErrors(IEnumerable<string> keys, ModelStateDictionary modelState);
        void AddErrors(ModelStateDictionary modelState);
        Error GetError(string key);
        string GetErrorMessage(string key);
        IEnumerable<Error> GetErrors();
        bool HasErrors();
        bool HasErrorForKey(string key);
    }
}
