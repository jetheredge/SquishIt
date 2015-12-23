using SquishIt.Framework.Utilities;

namespace SquishIt.Framework
{
    public class Bootstrap
    {
        public static void Shutdown()
        {
            TempFileResolutionCache.Clear();
        }
    }
}
