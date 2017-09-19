using SquishIt.Framework.Caches;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework
{
    public interface IPlatformConfiguration
    {
        /// <summary>
        /// Path translation implementation for platform.  
        /// </summary>
        IPathTranslator PathTranslator { get; set; }

        /// <summary>
        /// Cache implementation for platform.
        /// </summary>
        ICacheImplementation CacheImplementation { get; set; }

        /// <summary>
        /// Debug status reader for platform.
        /// </summary>
        IDebugStatusReader DebugStatusReader { get; set; }

        /// <summary>
        /// Trust level provider for platform.
        /// </summary>
        ITrustLevel TrustLevel { get; set; }

        /// <summary>
        /// QueryStringUtility for platform.
        /// </summary>
        IQueryStringUtility QueryStringUtility { get; set; }
    }

    /// <inheritdoc />
    public class PlatformConfiguration : IPlatformConfiguration
    {
        public ICacheImplementation CacheImplementation { get; set; }
        public IDebugStatusReader DebugStatusReader { get; set; }
        public ITrustLevel TrustLevel { get; set; }
        public IQueryStringUtility QueryStringUtility { get; set; }
        public IPathTranslator PathTranslator { get; set; }
    }
}