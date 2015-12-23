using SquishIt.Framework;

namespace SquishIt.Less
{
    public class Bootstrap
    {
        public static void Initialize()
        {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
        }
    }
}
