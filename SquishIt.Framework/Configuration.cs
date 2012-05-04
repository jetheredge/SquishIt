using System;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Renderers;

namespace SquishIt.Framework
{
    public class Configuration
    {
        public Configuration UseMinifierForCss<TMinifier>()
            where TMinifier : IMinifier<CSSBundle>
        {
            return UseMinifierForCss(typeof (TMinifier));
        }

        public Configuration UseMinifierForCss(Type minifierType)
        {
            if (!typeof(IMinifier<CSSBundle>).IsAssignableFrom(minifierType))
                throw new InvalidCastException(
                    String.Format("Type '{0}' must implement '{1}' to be used for Css minification.", 
                        minifierType, typeof (IMinifier<CSSBundle>)));
            _defaultCssMinifier = minifierType;
            return this;
        }

        public Configuration UseMinifierForJs<TMinifier>()
            where TMinifier : IMinifier<JavaScriptBundle>
        {
            return UseMinifierForJs(typeof (TMinifier));
        }

        public Configuration UseMinifierForJs(Type minifierType)
        {
            if (!typeof(IMinifier<JavaScriptBundle>).IsAssignableFrom(minifierType))
                throw new InvalidCastException(
                    String.Format("Type '{0}' must implement '{1}' to be used for Javascript minification.",
                                  minifierType, typeof (IMinifier<JavaScriptBundle>)));
            _defaultJsMinifier = minifierType;
            return this;
        }

        /// <summary>
        /// Use Yahoo YUI Compressor for CSS minification by default.
        /// </summary>
        public Configuration UseYuiForCssMinification()
        {
            return UseMinifierForCss<YuiCompressor>();
        }

        /// <summary>
        /// Use Microsoft Ajax Minifier for CSS minification by default.
        /// </summary>
        public Configuration UseMsAjaxForCssMinification()
        {
            return UseMinifierForCss<MsCompressor>();
        }

        /// <summary>
        /// By default, perform no minification of CSS.
        /// </summary>
        public Configuration UseNoCssMinification()
        {
            return UseMinifierForCss<NullCompressor>();
        }

        /// <summary>
        /// Use Microsoft Ajax Minifier for Javascript minification by default.
        /// </summary>
        public Configuration UseMsAjaxForJsMinification()
        {
            return UseMinifierForJs<MsMinifier>();
        }

        /// <summary>
        /// Use Yahoo YUI Compressor for Javascript minification by default.
        /// </summary>
        public Configuration UseYuiForJsMinification()
        {
            return UseMinifierForJs<YuiMinifier>();
        }

        /// <summary>
        /// Use Google Closure for Javascript minification by default.
        /// </summary>
        public Configuration UseClosureForMinification()
        {
            return UseMinifierForJs<ClosureMinifier>();
        }

        /// <summary>
        /// By default, perform no minification of Javascript.
        /// </summary>
        public Configuration UseNoJsMinification()
        {
            return UseMinifierForJs<NullMinifier>();
        }

        /// <summary>
        /// Use Douglas Crockford's JsMin for Javascript minification by default.
        /// </summary>
        public Configuration UseJsMinForJsMinification()
        {
            return UseMinifierForJs<JsMinMinifier>();
        }

        static Type _defaultCssMinifier = typeof (MsCompressor);
        static Type _defaultJsMinifier = typeof (MsMinifier);

        internal static IMinifier<CSSBundle> DefaultCssMinifier()
        {
            return (IMinifier<CSSBundle>)Activator.CreateInstance(_defaultCssMinifier);
        }

        internal static IMinifier<JavaScriptBundle> DefaultJsMinifier()
        {
            return (IMinifier<JavaScriptBundle>)Activator.CreateInstance(_defaultJsMinifier);
        }

        static IRenderer _defaultReleaseRenderer;
        public Configuration UseReleaseRenderer(IRenderer instance)
        {
            _defaultReleaseRenderer = instance;
            return this;
        }

        internal static IRenderer DefaultReleaseRenderer()
        {
            return _defaultReleaseRenderer;
        }

        static string _defaultOutputBaseHref;
        public Configuration UseOutputBaseHref(string url)
        {
            _defaultOutputBaseHref = url;
            return this;
        }

        internal static string DefaultOutputBaseHref()
        {
            return _defaultOutputBaseHref;
        }
    }
}
