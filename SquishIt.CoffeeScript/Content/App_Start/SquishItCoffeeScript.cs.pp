[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SquishItCoffeeScript), "Start")]

namespace $rootnamespace$.App_Start
{
    using SquishIt.Framework;
    using SquishIt.CoffeeScript;

    public class SquishItCoffeeScript
    {
        public static void Start()
        {
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
        }
    }
}