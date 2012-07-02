[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItSass), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework;
    using SquishIt.Sass;

    public class SquishItSass
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new SassPreprocessor());
        }
    }
}