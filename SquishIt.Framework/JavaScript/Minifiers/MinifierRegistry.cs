using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SquishIt.Framework.JavaScript.Minifiers
{
    public class MinifierRegistry
    {
        private static Dictionary<string, IJavaScriptCompressor> registry = new Dictionary<string, IJavaScriptCompressor>();

        static MinifierRegistry()
        {
            var minifierTypes = Assembly.GetAssembly(typeof (MsMinifier)).GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("SquishIt.Framework.JavaScript.Minifiers"))
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .Where(t => typeof (IJavaScriptCompressor).IsAssignableFrom(t));

            foreach (Type type in minifierTypes)
            {
                var compressor = (IJavaScriptCompressor)Activator.CreateInstance(type);
                registry.Add(compressor.Identifier, compressor);
            }
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