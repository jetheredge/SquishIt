using System.Collections.Generic;

namespace SquishIt.Framework
{
    /// <summary>
    /// Data class detailing results of preprocessing.
    /// </summary>
    public interface IProcessResult
    {
        /// <summary>
        /// Preprocessed content.
        /// </summary>
        string Result { get; }

        /// <summary>
        /// File dependencies introduced when preprocessing.
        /// </summary>
        IEnumerable<string> Dependencies { get; }
    }
}
