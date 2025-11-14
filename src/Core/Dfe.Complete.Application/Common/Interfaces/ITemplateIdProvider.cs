namespace Dfe.Complete.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for resolving logical template keys to provider-specific template IDs.
    /// Abstracts template management from email sending logic.
    /// </summary>
    public interface ITemplateIdProvider
    {
        /// <summary>
        /// Gets the provider template ID for a given logical template key.
        /// </summary>
        /// <param name="templateKey">Logical template key (e.g., "NewAccountAdded")</param>
        /// <returns>Provider template ID (e.g., GUID for GOV.UK Notify)</returns>
        /// <exception cref="KeyNotFoundException">Thrown when template key is not found</exception>
        string GetTemplateId(string templateKey);

        /// <summary>
        /// Checks if a template exists for the given key.
        /// </summary>
        /// <param name="templateKey">Logical template key</param>
        /// <returns>True if template exists, false otherwise</returns>
        bool TemplateExists(string templateKey);
    }
}

