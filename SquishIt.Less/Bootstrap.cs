using SquishIt.Framework;

namespace SquishIt.Less
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Configuration.Instance.RegisterStylePreprocessor(new LessPreprocessor());
        }
    }
}
