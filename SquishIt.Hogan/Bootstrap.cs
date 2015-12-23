using SquishIt.Framework;

namespace SquishIt.Hogan
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Bundle.RegisterScriptPreprocessor(new HoganPreprocessor());
        }
    }
}
