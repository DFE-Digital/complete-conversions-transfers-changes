using Dfe.Complete.Models;

namespace Dfe.Complete.Services.Project
{
    public interface IProjectService
    {
        public List<string> GetTransferProjectCompletionValidationResult(DateOnly? SignificantDate, bool SignificantDateProvisional, TransferTaskListViewModel taskList);
        public List<string> GetConversionProjectCompletionValidationResult(DateOnly? SignificantDate, bool SignificantDateProvisional, ConversionTaskListViewModel taskList);
    }
}
