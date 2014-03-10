using System;
using System.Linq;

namespace SquishIt.Framework.Base
{
    /// <summary>
    /// Base class for implementing custom preprocessors.
    /// </summary>
    public abstract class Preprocessor : IPreprocessor
    {
        /// <summary>
        /// Report to caller whether preprocessor is active for provided file extension.
        /// </summary>
        /// <param name="extension">Extension caller has found on a file.</param>
        /// <returns>bool</returns>
        public bool ValidFor(string extension)
        {
            return Extensions.Contains(extension.StartsWith(".") ? extension : ("." + extension), StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Process content.
        /// </summary>
        /// <param name="filePath">Disk location of content.</param>
        /// <param name="content">Content.</param>
        /// <returns><see cref="IProcessResult">IProcessResult</see></returns>
        public abstract IProcessResult Process(string filePath, string content);

        /// <summary>
        /// File extensions this preprocessor instance can be used for.
        /// </summary>
        public abstract string[] Extensions { get; }

        /// <summary>
        /// Extensions that should be "ignored" when calculating preprocessors to use from file path.
        /// </summary>
        public virtual string[] IgnoreExtensions { get { return new string[0]; } }
    }
}