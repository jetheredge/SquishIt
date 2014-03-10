[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItNSass), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework;
    using SquishIt.NSass;

    public class SquishItNSass
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new NSassPreprocessor());
        }
    }
}