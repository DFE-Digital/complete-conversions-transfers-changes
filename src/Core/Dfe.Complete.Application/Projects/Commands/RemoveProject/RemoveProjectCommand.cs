using Dfe.Complete.Application.Common.Exceptions;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Extensions;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Commands.RemoveProject
{
    public record RemoveProjectCommand(
        Urn Urn) : IRequest;

    public class RemoveProjectCommandHandler(
        IHostEnvironment hostEnvironment,
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<TransferTasksData> transferTaskRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RemoveProjectCommand>
    {
        public async Task Handle(RemoveProjectCommand request, CancellationToken cancellationToken)
        {
            // TODO: Remove temporary environment limiting
            // This is to prevent real projects from currently being deleted
            // This will have to be changed when we implement in app deletes
            // As well as making sure that we differentiate between soft and hard deletes
            if (!hostEnvironment.IsDevelopment() && !hostEnvironment.IsTest())
            {
                throw new NotDevOrTestEnvironmentException();
            }

            await unitOfWork.BeginTransactionAsync();

            try
            {
                var project = await projectRepository.Query().Include(p => p.Notes)
                    .FirstOrDefaultAsync(x => x.Urn == request.Urn, cancellationToken);

                if (project == null)
                {
                    return;
                }

                project.RemoveAllNotes();
                await projectRepository.UpdateAsync(project, cancellationToken);

                if (project is { TasksDataType: Domain.Enums.TaskType.Conversion, TasksDataId: not null })
                {
                    var conversionTaskList =
                        (ConversionTasksData?)await conversionTaskRepository.FindAsync(
                            task => task.Id == new TaskDataId(project.TasksDataId.Value), cancellationToken);
                    if (conversionTaskList is not null)
                    {
                        await conversionTaskRepository.RemoveAsync(conversionTaskList, cancellationToken);
                    }
                }
                else if (project is { TasksDataType: Domain.Enums.TaskType.Transfer, TasksDataId: not null })
                {
                    var transferTaskList =
                        (TransferTasksData?)await transferTaskRepository.FindAsync(
                            task => task.Id == new TaskDataId(project.TasksDataId.Value), cancellationToken);
                    if (transferTaskList is not null)
                    {
                        await transferTaskRepository.RemoveAsync(transferTaskList, cancellationToken);
                    }
                }

                await projectRepository.RemoveAsync(project, cancellationToken);

                await unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await unitOfWork.RollBackAsync();
                throw;
            }
        }
    }
}