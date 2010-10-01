using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SquishIt.Framework.JavaScript.Minifiers
{
    public class MinifierRegistry
    {
        private static Dictionary<string, IJavaScriptMinifier> registry = new Dictionary<string, IJavaScriptMinifier>();

        static MinifierRegistry()
        {
            var minifierTypes = Assembly.GetAssembly(typeof (MsMinifier)).GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("SquishIt.Framework.JavaScript.Minifiers"))
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .Where(t => typeof (IJavaScriptMinifier).IsAssignableFrom(t));

            foreach (Type type in minifierTypes)
            {
                var compressor = (IJavaScriptMinifier)Activator.CreateInstance(type);
                registry.Add(compressor.Identifier, compressor);
            }
        }        

        public static IJavaScriptMinifier Get(string identifier)
        {
            if (registry.ContainsKey(identifier))
            {
                return registry[identifier];    
            }
            return registry[NullMinifier.Identifier];
        }
    }
}