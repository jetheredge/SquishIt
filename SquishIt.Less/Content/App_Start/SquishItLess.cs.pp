[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItLess), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework.Css;
    using SquishIt.Less;

    public class SquishItLess
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
        }
    }
}