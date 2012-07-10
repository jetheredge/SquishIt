using System;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Renderers;
using MsMinifier = SquishIt.Framework.Minifiers.CSS.MsMinifier;
using NullMinifier = SquishIt.Framework.Minifiers.CSS.NullMinifier;
using YuiMinifier = SquishIt.Framework.Minifiers.CSS.YuiMinifier;

namespace SquishIt.Framework
{
    public class Configuration
    {
        static Configuration instance;
        Type _defaultCssMinifier = typeof (MsMinifier);
        Type _defaultJsMinifier = typeof (Minifiers.JavaScript.MsMinifier);
        string _defaultOutputBaseHref;
        IRenderer _defaultReleaseRenderer;

        public static Configuration Instance
        {
            get { return (instance = instance ?? new Configuration()); }
        }

        public Configuration UseMinifierForCss<TMinifier>()
            where TMinifier : IMinifier<CSSBundle>
        {
            return UseMinifierForCss(typeof (TMinifier));
        }

        public Configuration UseMinifierForCss(Type minifierType)
        {
            if (!typeof (IMinifier<CSSBundle>).IsAssignableFrom(minifierType))
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
            if (!typeof (IMinifier<JavaScriptBundle>).IsAssignableFrom(minifierType))
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
            return UseMinifierForCss<YuiMinifier>();
        }

        /// <summary>
        /// Use Microsoft Ajax Minifier for CSS minification by default.
        /// </summary>
        public Configuration UseMsAjaxForCssMinification()
        {
            return UseMinifierForCss<MsMinifier>();
        }

        /// <summary>
        /// By default, perform no minification of CSS.
        /// </summary>
        public Configuration UseNoCssMinification()
        {
            return UseMinifierForCss<NullMinifier>();
        }

        /// <summary>
        /// Use Microsoft Ajax Minifier for Javascript minification by default.
        /// </summary>
        public Configuration UseMsAjaxForJsMinification()
        {
            return UseMinifierForJs<Minifiers.JavaScript.MsMinifier>();
        }

        /// <summary>
        /// Use Yahoo YUI Compressor for Javascript minification by default.
        /// </summary>
        public Configuration UseYuiForJsMinification()
        {
            return UseMinifierForJs<Minifiers.JavaScript.YuiMinifier>();
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
            return UseMinifierForJs<Minifiers.JavaScript.NullMinifier>();
        }

        /// <summary>
        /// Use Douglas Crockford's JsMin for Javascript minification by default.
        /// </summary>
        public Configuration UseJsMinForJsMinification()
        {
            return UseMinifierForJs<JsMinMinifier>();
        }

        internal IMinifier<CSSBundle> DefaultCssMinifier()
        {
            return (IMinifier<CSSBundle>) Activator.CreateInstance(_defaultCssMinifier, true);
        }

        internal IMinifier<JavaScriptBundle> DefaultJsMinifier()
        {
            return (IMinifier<JavaScriptBundle>) Activator.CreateInstance(_defaultJsMinifier, true);
        }

        public Configuration UseReleaseRenderer(IRenderer releaseRenderer)
        {
            _defaultReleaseRenderer = releaseRenderer;
            return this;
        }

        internal IRenderer DefaultReleaseRenderer()
        {
            return _defaultReleaseRenderer;
        }

        public Configuration UseDefaultOutputBaseHref(string url)
        {
            _defaultOutputBaseHref = url;
            return this;
        }

        internal string DefaultOutputBaseHref()
        {
            return _defaultOutputBaseHref;
        }
    }
}