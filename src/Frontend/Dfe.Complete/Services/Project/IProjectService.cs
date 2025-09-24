using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectCompletion;

namespace Dfe.Complete.Services.Project
{
    public interface IProjectService
    {
        public TransferCompletionModel GetTransferProjectCompletionResult(DateOnly? SignificantDate, TransferTaskListViewModel taskList);
        public ConversionCompletionModel GetConversionProjectCompletionResult(DateOnly? SignificantDate, ConversionTaskListViewModel taskList);
    }
}
