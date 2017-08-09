using SquishIt.AspNet.Caches;
using SquishIt.AspNet.Utilities;
using SquishIt.Framework;

namespace SquishIt.AspNet
{
    /// <summary>
    /// This seems like the easiest way to load package specific configuration options
    /// </summary>
    public static class ConfigurationLoader
    {
        public static void Initialize()
        {
            Configuration.Apply(Load);
        }

        public static void Load(ISquishItOptions options)
        {
            options.CacheImplementation = new CacheImplementation();
            options.PathTranslator = new PathTranslator();
            options.DebugStatusReader = new DebugStatusReader();
            options.TrustLevel = new TrustLevel();
            options.QueryStringManager = new QueryStringManager();
        }
    }
}
