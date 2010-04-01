using System.Collections.Generic;

namespace Bundler.Framework.Css.Compressors
{
    public class CssCompressorRegistry
    {
        private static Dictionary<string, ICssCompressor> registry = new Dictionary<string, ICssCompressor>();

        static CssCompressorRegistry()
        {
            registry.Add(NullCompressor.Identifier, new NullCompressor());
            registry.Add(YuiCompressor.Identifier, new YuiCompressor());
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