using System.Collections.Generic;

namespace SquishIt.Framework.JavaScript.Minifiers
{
    public class MinifierRegistry
    {
        private static Dictionary<string, IJavaScriptCompressor> registry = new Dictionary<string, IJavaScriptCompressor>();

        static MinifierRegistry()
        {
            registry.Add(ClosureMinifier.Identifier, new ClosureMinifier());
            registry.Add(JsMinMinifier.Identifier, new JsMinMinifier());
            registry.Add(NullMinifier.Identifier, new NullMinifier());
            registry.Add(YuiMinifier.Identifier, new YuiMinifier());
        }        

        public static IJavaScriptCompressor Get(string identifier)
        {
            if (registry.ContainsKey(identifier))
            {
                return registry[identifier];    
            }
            return registry[NullMinifier.Identifier];
        }
    }
}