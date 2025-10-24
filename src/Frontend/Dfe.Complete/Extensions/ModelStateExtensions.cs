using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Dfe.Complete.Extensions
{
    public static class ModelStateExtensions
    {
        public static void RemoveError(this ModelStateDictionary modelState, string key, string errorMessage)
        {
            if (modelState.ContainsKey(key))
            {
                var entry = modelState[key];
                if(entry == null)
                {
                    return;
                }
                var remainingErrors = entry.Errors
                    .Where(e => e.ErrorMessage != errorMessage)
                    .ToList();

                entry.Errors.Clear();
                foreach (var err in remainingErrors)
                {
                    entry.Errors.Add(err);
                }
            }
        }

        public static bool IsActuallyValid(this ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(v => v.Errors).LongCount() == 0;
        }
    }
}
