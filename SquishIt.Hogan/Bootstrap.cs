using SquishIt.Framework;

namespace SquishIt.Hogan
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterScriptPreprocessor(new HoganPreprocessor());
        }
    }
}
