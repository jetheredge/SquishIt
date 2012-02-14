using System;
using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework
{
    public class Bundle
    {
        internal static readonly List<IPreprocessor> GlobalPreprocessors = new List<IPreprocessor>();
        internal static readonly List<IPreprocessor> ScriptPreprocessors = new List<IPreprocessor>();
        internal static readonly List<IPreprocessor> StylePreprocessors = new List<IPreprocessor>();

        internal static readonly HashSet<String> AllowedGlobalExtensions = new HashSet<string>();
        internal static readonly HashSet<String> AllowedScriptExtensions = new HashSet<string> { ".JS" };
        internal static readonly HashSet<String> AllowedStyleExtensions = new HashSet<string> { ".CSS" };

        internal static IEnumerable<IPreprocessor> Preprocessors
        {
            get { return GlobalPreprocessors.Union(ScriptPreprocessors).Union(StylePreprocessors); }
        }

        //TODO: provide a way to globally register preprocessor, registering extension w/ both JS and CSS allowed extensions?
        public static void RegisterGlobalPreprocessor<T>(T instance) where T : IPreprocessor
        {
            if(Preprocessors.Any(p => p.GetType() == typeof(T)))
            {
                throw new InvalidOperationException(string.Format("Can't add multiple preprocessors of type {0}", typeof(T).FullName));
            }
            foreach (var ext in instance.Extensions) AllowedGlobalExtensions.Add(ext.ToUpper());
            GlobalPreprocessors.Add(instance);
        }

        public static void RegisterScriptPreprocessor<T>(T instance) where T : IPreprocessor 
        {
            if (Preprocessors.Any(p => p.GetType() == typeof(T))) 
            {
                throw new InvalidOperationException(string.Format("Can't add multiple preprocessors of type {0}", typeof(T).FullName));
            }
            foreach (var ext in instance.Extensions) AllowedScriptExtensions.Add(ext.ToUpper());
            ScriptPreprocessors.Add(instance);
        }

        public static void RegisterStylePreprocessor<T>(T instance) where T : IPreprocessor 
        {
            if (Preprocessors.Any(p => p.GetType() == typeof(T))) 
            {
                throw new InvalidOperationException(string.Format("Can't add multiple preprocessors of type {0}", typeof(T).FullName));
            }
            foreach (var ext in instance.Extensions) AllowedStyleExtensions.Add(ext.ToUpper());
            StylePreprocessors.Add(instance);
        }

        public static void ClearPreprocessors()
        {
            GlobalPreprocessors.Clear();
            ScriptPreprocessors.Clear();
            StylePreprocessors.Clear();

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
    }
}