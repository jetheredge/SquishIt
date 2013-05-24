using System;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Invalidation;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;
using MsMinifier = SquishIt.Framework.Minifiers.CSS.MsMinifier;
using NullMinifier = SquishIt.Framework.Minifiers.CSS.NullMinifier;
using YuiMinifier = SquishIt.Framework.Minifiers.CSS.YuiMinifier;

namespace SquishIt.Framework
{
    public class Configuration
    {
        static Configuration instance;
        IMinifier<CSSBundle> _defaultCssMinifier = new MsMinifier();
        IMinifier<JavaScriptBundle> _defaultJsMinifier = new Minifiers.JavaScript.MsMinifier();
        ICacheInvalidationStrategy _defaultCacheInvalidationStrategy = new DefaultCacheInvalidationStrategy();
        string _defaultHashKeyName = "r";
        string _defaultOutputBaseHref;
        IRenderer _defaultReleaseRenderer;
        Func<bool> _defaultDebugPredicate;
        IHasher _defaultHasher = new Hasher(new RetryableFileOpener());

        public static Configuration Instance
        {
            get { return (instance = instance ?? new Configuration()); }
            internal set { instance = value; }
        }

        public Configuration()
        {
            JavascriptMimeType = "application/javascript";
            CssMimeType = "text/css";
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
            return UseMinifierForCss((IMinifier<CSSBundle>) Activator.CreateInstance(minifierType, true));
        }

        public Configuration UseMinifierForCss(IMinifier<CSSBundle> minifier)
        {
            _defaultCssMinifier = minifier;
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
            return UseMinifierForJs((IMinifier<JavaScriptBundle>)Activator.CreateInstance(minifierType, true));
        }

        public Configuration UseMinifierForJs(IMinifier<JavaScriptBundle> minifier)
        {
            _defaultJsMinifier = minifier;
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
            return _defaultCssMinifier;
        }

        internal IMinifier<JavaScriptBundle> DefaultJsMinifier()
        {
            return _defaultJsMinifier;
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

        public Configuration UseConditionToForceDebugging(Func<bool> condition)
        {
            _defaultDebugPredicate = condition;
            return this;
        }

        internal Func<bool> DefaultDebugPredicate()
        {
            return _defaultDebugPredicate;
        }

        public Configuration UseHasher(IHasher hasher)
        {
            _defaultHasher = hasher;
            return this;
        }

        /// <summary>
        /// Mime-type used to serve Javascript content. Defaults to "application/javascript".
        /// To enable gzip compression in IIS change this to "application/x-javascript".
        /// </summary>
        public string JavascriptMimeType { get; set; }
        /// <summary>
        /// Mime-type used to serve CSS content. Defaults to "text/css".
        /// </summary>
        public string CssMimeType { get; set; }

        internal IHasher DefaultHasher()
        {
            return _defaultHasher;
        }

        public Configuration UseHashAsVirtualDirectoryInvalidationStrategy()
        {
            return UseCacheInvalidationStrategy(new HashAsVirtualDirectoryCacheInvalidationStrategy());
        }

        public Configuration UseCacheInvalidationStrategy(ICacheInvalidationStrategy strategy)
        {
            _defaultCacheInvalidationStrategy = strategy;
            return this;
        }

        public ICacheInvalidationStrategy DefaultCacheInvalidationStrategy()
        {
            return _defaultCacheInvalidationStrategy;
        }

        public Configuration UseHashKeyName(string hashKeyName)
        {
            _defaultHashKeyName = hashKeyName;
            return this;
        }

        public string DefaultHashKeyName()
        {
            return _defaultHashKeyName;
        }
    }
}