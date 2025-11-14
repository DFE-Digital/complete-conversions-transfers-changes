namespace Dfe.Complete.Application.Common.Interfaces
{
    /// <summary>
    /// Builds project URLs for email notifications and other purposes.
    /// </summary>
    public interface IProjectUrlBuilder
    {
        /// <summary>
        /// Builds a URL to view a project.
        /// </summary>
        /// <param name="projectReference">The project reference/ID</param>
        /// <returns>Full URL to the project</returns>
        string BuildProjectUrl(string projectReference);
    }
}

