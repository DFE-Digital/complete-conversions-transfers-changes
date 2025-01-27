using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.Complete.Extensions;

public static class ModelStateExtensions
{
    public static void ClearErrorsForProperties(this ModelStateDictionary modelState, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (modelState.TryGetValue(propertyName, out var modelStateEntry))
            {
                modelStateEntry.Errors.Clear();
            }
        }
    }
}