﻿using Dfe.Complete.Application.Common.Exceptions;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
            if (!hostEnvironment.IsDevelopment())
            {
                throw new NotDevEnvironmentException();
            }

            await unitOfWork.BeginTransactionAsync();

            try
            {
                var project = await projectRepository.Query().Include(p => p.Notes).FirstOrDefaultAsync(x => x.Urn == request.Urn, cancellationToken);

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
                        (TransferTasksData?)await transferTaskRepository.GetAsync(
                            new TaskDataId(project.TasksDataId.Value),
                            cancellationToken);
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