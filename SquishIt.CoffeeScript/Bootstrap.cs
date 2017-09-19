using SquishIt.Framework;

namespace SquishIt.CoffeeScript
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}
