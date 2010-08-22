using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SquishIt.Framework.Css.Compressors
{
    public class CssCompressorRegistry
    {
        private static Dictionary<string, ICssCompressor> registry = new Dictionary<string, ICssCompressor>();

        static CssCompressorRegistry()
        {
            var minifierTypes = Assembly.GetAssembly(typeof(MsCompressor)).GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("SquishIt.Framework.Css.Compressors"))
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .Where(t => typeof(ICssCompressor).IsAssignableFrom(t));

            foreach (Type type in minifierTypes)
            {
                var compressor = (ICssCompressor)Activator.CreateInstance(type);
                registry.Add(compressor.Identifier, compressor);
            }
        }        

        public static ICssCompressor Get(string identifier)
        {
            if (registry.ContainsKey(identifier))
            {
                return registry[identifier];    
            }
            return registry[NullCompressor.Identifier];
        }
    }
}