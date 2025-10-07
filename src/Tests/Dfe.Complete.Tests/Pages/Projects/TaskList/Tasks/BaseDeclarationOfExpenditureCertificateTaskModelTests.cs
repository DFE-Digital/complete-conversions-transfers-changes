using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.DeclarationOfExpenditureCertificateTask;
using Dfe.Complete.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks
{
    public class BaseDeclarationOfExpenditureCertificateTaskModelTests
    {
        private DeclarationOfExpenditureCertificateTaskModel GetModel(Mock<ISender>  sender, Mock<IErrorService> errorService, int days)
        { 
            var auth = new Mock<IAuthorizationService>();
            var logger = new Mock<ILogger<DeclarationOfExpenditureCertificateTaskModel>>(); 
            var model = new DeclarationOfExpenditureCertificateTaskModel(
            sender.Object, auth.Object, logger.Object, errorService.Object)
            {
                TaskIdentifier = NoteTaskIdentifier.DeclarationOfExpenditureCertificate,
                NotApplicable = false,
                ReceivedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(days)),
                TasksDataId = Guid.NewGuid(),
                Type = ProjectType.Conversion,

            };
            var httpContext = new DefaultHttpContext();
            model.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return model;
        }
        [Fact]
        public async Task OnPost_InvalidReceivedDate_AddsModelErrorAndReturnsPage()
        {
            // Arrange  
            var sender = new Mock<ISender>();
            var errorService = new Mock<IErrorService>();
            var model = GetModel(sender, errorService, 1);

            // Act
            var result = await model.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);
            errorService.Verify(es => es.AddErrors(It.IsAny<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>()), Times.Once);
            Assert.False(model.ModelState.IsValid);
            Assert.Contains("received-date", model.ModelState.Keys);
        }

        [Fact]
        public async Task OnPost_ValidModel_SendsCommandAndRedirects()
        {
            // Arrange
            var errorService = new Mock<IErrorService>();
            var sender = new Mock<ISender>();
            sender.Setup(s => s.Send(It.IsAny<UpdateDeclarationOfExpenditureCertificateTaskCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));
            var model = GetModel(sender, errorService, -1);

            // Act
            var result = await model.OnPost();

            // Assert
            Assert.IsType<RedirectResult>(result);
            sender.Verify(s => s.Send(It.IsAny<UpdateDeclarationOfExpenditureCertificateTaskCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
