using SquishIt.Framework;

namespace SquishIt.NSass
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Bundle.RegisterScriptPreprocessor(new NSassPreprocessor());
        }
    }
}
