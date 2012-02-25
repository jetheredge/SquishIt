using System;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Minifiers.JavaScript;

namespace SquishIt.Framework
{
    public class Configuration
    {
        /// <summary>
        /// Use Yahoo YUI Compressor for CSS minification by default.
        /// </summary>
        public Configuration UseYuiForCssMinification()
        {
            _defaultCssMinifier = typeof (YuiCompressor);
            return this;
        }

        /// <summary>
        /// Use Microsoft Ajax Minifier for CSS minification by default.
        /// </summary>
        public Configuration UseMsAjaxForCssMinification()
        {
            _defaultCssMinifier = typeof (MsCompressor);
            return this;
        }

        /// <summary>
        /// By default, perform no minification of CSS.
        /// </summary>
        public Configuration UseNoCssMinification()
        {
            _defaultCssMinifier = typeof (NullCompressor);
            return this;
        }

        /// <summary>
        /// Use Microsoft Ajax Minifier for Javascript minification by default.
        /// </summary>
        public Configuration UseMsAjaxForJsMinification()
        {
            _defaultJsMinifier = typeof (MsMinifier);
            return this;
        }

        /// <summary>
        /// Use Yahoo YUI Compressor for Javascript minification by default.
        /// </summary>
        public Configuration UseYuiForJsMinification()
        {
            _defaultJsMinifier = typeof (YuiMinifier);
            return this;
        }

        /// <summary>
        /// Use Google Closure for Javascript minification by default.
        /// </summary>
        public Configuration UseClosureForMinification()
        {
            _defaultJsMinifier = typeof (ClosureMinifier);
            return this;
        }

        /// <summary>
        /// By default, perform no minification of Javascript.
        /// </summary>
        public Configuration UseNoJsMinification()
        {
            _defaultJsMinifier = typeof (NullMinifier);
            return this;
        }

        /// <summary>
        /// Use Douglas Crockford's JsMin for Javascript minification by default.
        /// </summary>
        public Configuration UseJsMinForJsMinification()
        {
            _defaultJsMinifier = typeof (JsMinMinifier);
            return this;
        }

        static Type _defaultCssMinifier = typeof (MsCompressor);
        static Type _defaultJsMinifier = typeof (MsMinifier);

        internal static IMinifier<CSSBundle> DefaultCssMinifier()
        {
            return (IMinifier<CSSBundle>)Activator.CreateInstance(_defaultCssMinifier);
        }

        public static IMinifier<JavaScriptBundle> DefaultJsMinifier()
        {
            return (IMinifier<JavaScriptBundle>)Activator.CreateInstance(_defaultJsMinifier);
        }
    }
}
