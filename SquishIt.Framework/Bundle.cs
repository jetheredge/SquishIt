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

        internal static readonly HashSet<String> AllowedGlobalExtensions = new HashSet<string>();
        internal static readonly HashSet<String> AllowedScriptExtensions = new HashSet<string> { ".JS" };
        internal static readonly HashSet<String> AllowedStyleExtensions = new HashSet<string> { ".CSS" };

        public static void RegisterGlobalPreprocessor<T>(T instance) where T : IPreprocessor
        {
            ValidatePreprocessor<T>(instance);
            foreach (var ext in instance.Extensions) AllowedGlobalExtensions.Add(ext.ToUpper());
            Preprocessors.Add(instance);
        }

        public static void RegisterScriptPreprocessor<T>(T instance) where T : IPreprocessor 
        {
            ValidatePreprocessor<T>(instance);
            foreach (var ext in instance.Extensions) AllowedScriptExtensions.Add(ext.ToUpper());
            Preprocessors.Add(instance);
        }

        public static void RegisterStylePreprocessor<T>(T instance) where T : IPreprocessor 
        {
            ValidatePreprocessor<T>(instance);
            foreach (var ext in instance.Extensions) AllowedStyleExtensions.Add(ext.ToUpper());
            Preprocessors.Add(instance);
        }

        static void ValidatePreprocessor<T>(IPreprocessor instance)
        {
            if(Preprocessors.Any(p => p.GetType() == typeof(T)))
            {
                throw new InvalidOperationException(string.Format("Can't add multiple preprocessors of type: {0}", typeof(T).FullName));
            }

            foreach(var extension in instance.Extensions)
            {
                if (AllExtensions.Contains(extension))
                {
                    throw new InvalidOperationException(string.Format("Can't add multiple preprocessors for extension: {0}", extension));
                }
            }
        }

        static IEnumerable<string> AllExtensions
        {
            get { return AllowedGlobalExtensions.Union(AllowedScriptExtensions).Union(AllowedStyleExtensions).Select(x => x.ToUpper()); }
        }

        public static void ClearPreprocessors()
        {
            Preprocessors.Clear();

            AllowedGlobalExtensions.Clear();
            AllowedScriptExtensions.Clear();
            AllowedStyleExtensions.Clear();

            AllowedScriptExtensions.Add(".JS");
            AllowedStyleExtensions.Add(".CSS");
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
            return Configuration.Instance;
        }
    }
}