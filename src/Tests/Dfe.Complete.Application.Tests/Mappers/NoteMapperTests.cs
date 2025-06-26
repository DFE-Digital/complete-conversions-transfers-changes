using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using AutoMapper;
using Dfe.Complete.Application.Common.Mappers;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;

namespace Dfe.Complete.Application.Tests.Mappers
{
    public class NoteMapperTests
    {
        private readonly IMapper _mapper;

        public NoteMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapping>();
            });

            _mapper = config.CreateMapper();
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Map_NoteToNoteDto_ShouldMapAllPropertiesCorrectly(Domain.Entities.Note note)
        {
            // Act
            var noteDto = _mapper.Map<NoteDto>(note);

            // Assert
            Assert.NotNull(noteDto);
            Assert.Equal(note.Id, noteDto.Id);
            Assert.Equal(note.CreatedAt, noteDto.CreatedAt);
            Assert.Equal(note.UpdatedAt, noteDto.UpdatedAt);
            Assert.Equal(note.Body, noteDto.Body);
            Assert.Equal(note.ProjectId, noteDto.ProjectId);
            Assert.Equal(note.UserId, noteDto.UserId);
            Assert.Equal(note.TaskIdentifier, noteDto.TaskIdentifier);
            Assert.Equal(note.NotableId, noteDto.NotableId);
            Assert.Equal(note.NotableType, noteDto.NotableType);
            Assert.Equal(note.Project, noteDto.Project);
            Assert.Equal(note.User, noteDto.User);
        }
    }
}
