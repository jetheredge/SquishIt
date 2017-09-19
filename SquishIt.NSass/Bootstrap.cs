using SquishIt.Framework;

namespace SquishIt.NSass
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterScriptPreprocessor(new NSassPreprocessor());
        }
    }
}
