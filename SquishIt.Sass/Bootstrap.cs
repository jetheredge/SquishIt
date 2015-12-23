using SquishIt.Framework;

namespace SquishIt.Sass
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Bundle.RegisterScriptPreprocessor(new SassPreprocessor());
        }
    }
}
