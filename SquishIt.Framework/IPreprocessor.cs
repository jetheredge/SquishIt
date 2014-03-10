namespace SquishIt.Framework
{
    /// <summary>
    /// Interface for preprocessor implementations
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Report to caller whether preprocessor is active for provided file extension.
        /// </summary>
        /// <param name="extension">Extension caller has found on a file.</param>
        /// <returns>bool</returns>
        bool ValidFor(string extension);

        /// <summary>
        /// Process content.
        /// </summary>
        /// <param name="filePath">Disk location of content.</param>
        /// <param name="content">Content.</param>
        /// <returns><see cref="IProcessResult">IProcessResult</see></returns>
        IProcessResult Process(string filePath, string content);

        /// <summary>
        /// File extensions this preprocessor instance can be used for.
        /// </summary>
        string[] Extensions { get; }

        /// <summary>
        /// Extensions that should be "ignored" when calculating preprocessors to use from file path.
        /// </summary>
        string[] IgnoreExtensions { get; }
    }
}
