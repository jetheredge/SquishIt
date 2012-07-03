[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItHogan), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework;
    using SquishIt.Hogan;

    public class SquishItHogan
    {
        public static void Start()
        {
            Bundle.RegisterScriptPreprocessor(new HoganPreprocessor());
        }
    }
}