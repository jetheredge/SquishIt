using System;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Invalidation;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework
{
    public interface ISquishItOptions
    {
        /// <summary>
        /// Cache invalidation strategy used by default (can be overridden at bundle level using .WithCacheInvalidationStrategy)
        /// </summary>
        ICacheInvalidationStrategy DefaultCacheInvalidationStrategy { get; set; }

        /// <summary>
        /// CSS minifier used by default (can be overridden at bundle level using .WithMinifier)
        /// </summary>
        IMinifier<CSSBundle> DefaultCssMinifier { get; set; }

        /// <summary>
        /// JS minifier used by default (can be overridden at bundle level using .WithMinifier)
        /// </summary>
        IMinifier<JavaScriptBundle> DefaultJsMinifier { get; set; }

        /// <summary>
        /// Debug predicate used to analyze bundle contents at runtime (defaults to null, can be overridden at bundle level using .ForceDebugIf)
        /// </summary>
        Func<bool> DefaultDebugPredicate { get; set; }

        /// <summary>
        /// Name for hash key rendered in URLs - can be overridden at bundle level using HashKeyNamed OR WithoutRevisionHash
        /// </summary>
        string DefaultHashKeyName { get; set; }

        /// <summary>
        /// Hash calculation strategy to use - can't be overridden at bundle level
        /// </summary>
        IHasher DefaultHasher { get; set; }
        
        /// <summary>
        /// Base URL for CDN scenarios - can be overridden at bundle level using .WithOutputBaseHref
        /// </summary>
        string DefaultOutputBaseHref { get; set; }

        /// <summary>
        /// Path translator to use for CSS rewrites - can't be overridden at bundle level
        /// </summary>
        IPathTranslator DefaultPathTranslator { get; set; }

        /// <summary>
        /// Renderer to use for release file outputs - can be overridden at bundle level using .WithReleaseFileRenderer
        /// </summary>
        IRenderer DefaultReleaseRenderer { get; set; }

        /// <summary>
        /// Temp path provider to use for embedded resource resolution - can't be overridden at bundle level
        /// </summary>
        ITempPathProvider DefaultTempPathProvider { get; set; }

        /// <summary>
        /// File opener to use when reading / writing content (writing only for file system rendering) - can't be overridden at bundle level
        /// </summary>
        IRetryableFileOpener DefaultRetryableFileOpener { get; set; }

        /// <summary>
        ///     Mime-type used to serve Javascript content. Defaults to "application/javascript".
        ///     To enable gzip compression in IIS change this to "application/x-javascript".
        /// </summary>
        string DefaultJavascriptMimeType { get; set; }

        /// <summary>
        ///     Mime-type used to serve CSS content. Defaults to "text/css".
        /// </summary>
        string DefaultCssMimeType { get; set; }
    }

    public class Configuration : ISquishItOptions
    {
        public ICacheInvalidationStrategy DefaultCacheInvalidationStrategy { get; set; }

        public IMinifier<CSSBundle> DefaultCssMinifier { get; set; }

        public Func<bool> DefaultDebugPredicate { get; set; }

        public string DefaultHashKeyName { get; set; }

        public IHasher DefaultHasher { get; set; }

        public IMinifier<JavaScriptBundle> DefaultJsMinifier { get; set; }

        public string DefaultOutputBaseHref { get; set; }

        public IPathTranslator DefaultPathTranslator { get; set; }

        public IRenderer DefaultReleaseRenderer { get; set; }

        public ITempPathProvider DefaultTempPathProvider { get; set; }

        public IRetryableFileOpener DefaultRetryableFileOpener { get; set; }

        /// <summary>
        ///     Mime-type used to serve Javascript content. Defaults to "application/javascript".
        ///     To enable gzip compression in IIS change this to "application/x-javascript".
        /// </summary>
        public string DefaultJavascriptMimeType { get; set; }

        /// <summary>
        ///     Mime-type used to serve CSS content. Defaults to "text/css".
        /// </summary>
        public string DefaultCssMimeType { get; set; }

        public Configuration()
        {
            DefaultJavascriptMimeType = "application/javascript";
            DefaultCssMimeType = "text/css";
            DefaultCacheInvalidationStrategy = new DefaultCacheInvalidationStrategy();
            DefaultCssMinifier = new Minifiers.CSS.MsMinifier();
            DefaultHashKeyName = "r";
            DefaultJsMinifier = new Minifiers.JavaScript.MsMinifier();
            DefaultPathTranslator = new PathTranslator();
            DefaultTempPathProvider = new TempPathProvider();
            DefaultRetryableFileOpener = new RetryableFileOpener();
            DefaultHasher = new Hasher(DefaultRetryableFileOpener);
        }

        private static ISquishItOptions instance;

        public static ISquishItOptions Instance
        {
            get { return (instance = instance ?? new Configuration()); }
            internal set { instance = value; }
        }

        public static void Apply(Action<ISquishItOptions> configTransformer)
        {
            configTransformer(Instance);
        }
    }
}