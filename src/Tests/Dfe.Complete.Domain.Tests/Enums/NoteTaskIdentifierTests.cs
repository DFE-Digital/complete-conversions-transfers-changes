using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Domain.Tests.Enums
{ 
    public class NoteTaskIdentifierTests
    {
        [Theory]
        [InlineData(NoteTaskIdentifier.Handover, "handover")] 
        public void NoteTaskIdentifier_ShouldHaveCorrectDescription(NoteTaskIdentifier identifier, string expectedDescription)
        {
            // Act
            var description = identifier.ToDescription();

            // Assert
            Assert.Equal(expectedDescription, description);
        }
    } 
}
