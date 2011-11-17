using System;
using System.Collections.Generic;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework
{
    public class Bundle
    {
        internal static readonly List<IPreprocessor> JsPreprocessors = new List<IPreprocessor>();
        internal static readonly List<IPreprocessor> CssPreprocessors = new List<IPreprocessor>();

        public static void RegisterCssPreprocessor<T>() where T : IPreprocessor
        {
            CssPreprocessors.Add(Activator.CreateInstance<T>());
        }
        
        public static void RegisterJsPreprocessor<T>() where T : IPreprocessor
        {
            JsPreprocessors.Add(Activator.CreateInstance<T>());
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