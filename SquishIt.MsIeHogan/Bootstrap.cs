using SquishIt.Framework;

namespace SquishIt.MsIeHogan
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Bundle.RegisterScriptPreprocessor(new HoganPreprocessor());
        }
    }
}
