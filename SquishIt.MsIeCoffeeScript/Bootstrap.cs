using SquishIt.Framework;

namespace SquishIt.MsIeCoffeeScript
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}
