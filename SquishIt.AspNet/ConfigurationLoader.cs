using SquishIt.AspNet.Caches;
using SquishIt.AspNet.Utilities;
using SquishIt.Framework;

namespace SquishIt.AspNet
{
    public static class ConfigurationLoader
    {
        public static void Execute(ISquishItOptions options)
        {
            options.DefaultCacheImplementation = new CacheImplementation();
            options.DefaultPathTranslator = new PathTranslator();
            options.DefaultDebugStatusReader = new DebugStatusReader();
            options.TrustLevel = new TrustLevel();
            options.DefaultQueryStringManager = new QueryStringManager();
        }
    }
}
