using SquishIt.Framework;

namespace SquishIt.Sass
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterScriptPreprocessor(new SassPreprocessor());
        }
    }
}
