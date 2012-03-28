using System;
using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework
{
    public class Bundle
    {
        internal static readonly List<IPreprocessor> Preprocessors = new List<IPreprocessor>();
        
        internal static void RegisterPreprocessor<T>(T instance) where T : IPreprocessor
        {
            if(Preprocessors.Any(p => p.GetType() == typeof(T)))
            {
                throw new InvalidOperationException(string.Format("Can't add multiple preprocessors of type {0}", typeof(T).FullName));
            }
            Preprocessors.Add(instance);
        }

        public static void RemovePreprocessors(params string[] extensions)
        {
            var remove = Preprocessors.Where(pp => pp.Extensions.All(ext => extensions.Contains(ext))).ToList();
            foreach (var r in remove) Preprocessors.Remove(r);
        }

        public static JavaScriptBundle JavaScript()
        {
            return new JavaScriptBundle();
        }

        public static JavaScriptBundle JavaScript(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new JavaScriptBundle(debugStatusReader);
        }
       
        public static CSSBundle Css()
        {
            return new CSSBundle();
        }

        public static CSSBundle Css(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new CSSBundle(debugStatusReader);
        }

        public static Configuration ConfigureDefaults()
        {
            return new Configuration();
        }
    }
}