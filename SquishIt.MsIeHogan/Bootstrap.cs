using SquishIt.Framework;

namespace SquishIt.MsIeHogan
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterScriptPreprocessor(new HoganPreprocessor());
        }
    }
}
