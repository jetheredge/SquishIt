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
            Configuration.Apply(RegisterPlatform);
        }

        public static void RegisterPlatform(ISquishItOptions options)
        {
            options.Platform = new PlatformConfiguration
            {
                CacheImplementation = new CacheImplementation(),
                PathTranslator = new PathTranslator(),
                DebugStatusReader = new DebugStatusReader(),
                TrustLevel = new TrustLevel(),
                QueryStringUtility = new QueryStringUtility()
            };
        }
    }
}
