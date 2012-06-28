[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItHogan), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework.Hogan;
    using SquishIt.Hogan;

    public class SquishItHogan
    {
        public static void Start()
        {
            Bundle.RegisterTemplatePreprocessor(new HoganPreprocessor());
        }
    }
}