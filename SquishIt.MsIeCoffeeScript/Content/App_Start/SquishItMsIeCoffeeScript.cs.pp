[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItMsIeCoffeeScript), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework;
    using SquishIt.MsIeCoffeeScript;

    public class SquishItMsIeCoffeeScript
    {
        public static void Start()
        {
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}