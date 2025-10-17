using Dfe.Complete.Application.Notes.Queries.QueryFilters;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Tests.QueryFilters
{
    public class NoteQueryObjectTests
    {
        private static NoteId GetNoteId() => new(Guid.NewGuid());
        private static IQueryable<Note> SampleNotes() => new[]
        {
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000001")), TaskIdentifier = NoteTaskIdentifier.Handover.ToDescription() },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")), TaskIdentifier = NoteTaskIdentifier.LandQuestionnaire.ToDescription() },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")), TaskIdentifier = NoteTaskIdentifier.LandQuestionnaire.ToDescription() },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")), TaskIdentifier = NoteTaskIdentifier.LandRegistryTitlePlans.ToDescription() },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")) },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")) },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")), NotableId = Guid.NewGuid()},
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")), NotableType = nameof(Project) },
            new Note { Id = GetNoteId(), ProjectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000003")), TaskIdentifier = NoteTaskIdentifier.LandRegistryTitlePlans.ToDescription() }
        }.AsQueryable();

        [Fact]
        public void NoteIdQuery_NullId_DoesNotFilter()
        {
            var query = new NoteIdQuery(null);
            var filteredNotes = query.Apply(SampleNotes()).ToList();

            Assert.Equal(9, filteredNotes.Count);
            Assert.Equivalent(filteredNotes, SampleNotes().ToList());
        }

        [Fact]
        public void NoteIdQuery_FiltersCorrectly()
        {
            var noteId = new NoteId(Guid.Parse("00000000-0000-0000-0000-000000000002"));

            var notes = SampleNotes().ToList();
            notes[0].Id = noteId;

            var query = new NoteIdQuery(noteId);
            var filteredNotes = query.Apply(notes.AsQueryable()).ToList();

            Assert.Single(filteredNotes);
            Assert.Equal(noteId, filteredNotes[0].Id);
        }

        [Fact]
        public void ProjectNoteByIdQuery_NullId_DoesNotFilter()
        {
            var query = new ProjectNoteByIdQuery(null);
            var filteredNotes = query.Apply(SampleNotes()).ToList();

            Assert.Equal(9, filteredNotes.Count);
            Assert.Equivalent(filteredNotes, SampleNotes().ToList());
        }

        [Fact]
        public void ProjectNoteByIdQuery_FiltersCorrectly()
        {
            var projectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002"));

            var query = new ProjectNoteByIdQuery(projectId);
            var filteredNotes = query.Apply(SampleNotes()).ToList();

            Assert.Equal(2, filteredNotes.Count);
            Assert.All(filteredNotes, note =>
            {
                Assert.Null(note.TaskIdentifier);
                Assert.Null(note.NotableId);
                Assert.Null(note.NotableType);
            });
        }

        [Fact]
        public void ProjectTaskNoteByIdQuery_FiltersCorrectly()
        {
            var projectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var taskIdentifier = NoteTaskIdentifier.LandQuestionnaire;

            var query = new ProjectTaskNoteByIdQuery(projectId, taskIdentifier);
            var filteredNotes = query.Apply(SampleNotes()).ToList();

            Assert.Equal(2, filteredNotes.Count);
            Assert.All(filteredNotes, note =>
            {
                Assert.Equal(projectId, note.ProjectId);
                Assert.Equal(taskIdentifier.ToDescription(), note.TaskIdentifier);
            });
        }

        [Fact]
        public void ProjectTaskNoteByIdQuery_ReturnsNoResults()
        {
            var projectId = new ProjectId(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var taskIdentifier = NoteTaskIdentifier.Handover;

            var query = new ProjectTaskNoteByIdQuery(projectId, taskIdentifier);
            var filteredNotes = query.Apply(SampleNotes()).ToList();

            Assert.Empty(filteredNotes);
        }
    }
}
