[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItMsIeHogan), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework;
    using SquishIt.MsIeHogan;

    public class SquishItMsIeHogan
    {
        public static void Start()
        {
            Bundle.RegisterScriptPreprocessor(new HoganPreprocessor());
        }
    }
}