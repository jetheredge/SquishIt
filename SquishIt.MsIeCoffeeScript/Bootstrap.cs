using SquishIt.Framework;

namespace SquishIt.MsIeCoffeeScript
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}
