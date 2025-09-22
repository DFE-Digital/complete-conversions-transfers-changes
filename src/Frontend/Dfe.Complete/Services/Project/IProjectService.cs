using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project
{
    public interface IProjectService
    {
        public TransferCompletionValidationResultModel GetTransferProjectCompletionResult(DateOnly? SignificantDate, TransferTaskListViewModel taskList);
        public ConversionCompletionValidationResultModel GetConversionProjectCompletionResult(DateOnly? SignificantDate, ConversionTaskListViewModel taskList);
    }
}
