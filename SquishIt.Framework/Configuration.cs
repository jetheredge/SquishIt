using System;
using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework.Base;
using SquishIt.Framework.Caches;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Invalidation;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework
{
    /// <summary>
    /// Configuration for SquishIt
    /// </summary>
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

        /// <summary>
        /// Register a preprocessor for global use.
        /// </summary>
        /// <typeparam name="T">IPreprocessor implementation type.</typeparam>
        /// <param name="instance">Constructed instance of T that can be used for processing.</param>
        void RegisterGlobalPreprocessor<T>(T instance) where T : IPreprocessor;

        /// <summary>
        /// Register a preprocessor for script use.
        /// </summary>
        /// <typeparam name="T">IPreprocessor implementation type.</typeparam>
        /// <param name="instance">Constructed instance of T that can be used for processing.</param>
        void RegisterScriptPreprocessor<T>(T instance) where T : IPreprocessor;

        /// <summary>
        /// Register a preprocessor for style use.
        /// </summary>
        /// <typeparam name="T">IPreprocessor implementation type.</typeparam>
        /// <param name="instance">Constructed instance of T that can be used for processing.</param>
        void RegisterStylePreprocessor<T>(T instance) where T : IPreprocessor;

        #region internals
        /// <summary>
        /// Platform-specific configuration.
        /// </summary>
        IPlatformConfiguration Platform { get; set; }

        /// <summary>
        /// Preprocessors registered with framework.
        /// </summary>
        IEnumerable<IPreprocessor> Preprocessors { get; }

        /// <summary>
        /// File extensions supported by this configuration.
        /// </summary>
        IEnumerable<string> AllowedGlobalExtensions { get; }

        /// <summary>
        /// Script file extensions supported by this configuration.
        /// </summary>
        IEnumerable<string> AllowedScriptExtensions { get; }

        /// <summary>
        /// Style file extensions supported by this configuration.
        /// </summary>
        IEnumerable<string> AllowedStyleExtensions { get; }

        /// <summary>
        /// Clear configured preprocessors.
        /// </summary>
        void ClearPreprocessors();
        #endregion
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

        public IPlatformConfiguration Platform { get; set; }

        public Configuration()
        {
            DefaultJavascriptMimeType = "application/javascript";
            DefaultCssMimeType = "text/css";
            DefaultCacheInvalidationStrategy = new DefaultCacheInvalidationStrategy();
            DefaultCssMinifier = new Minifiers.CSS.MsMinifier();
            DefaultHashKeyName = "r";
            DefaultJsMinifier = new Minifiers.JavaScript.MsMinifier();
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

        public static void Apply(Action<ISquishItOptions> configurer)
        {
            configurer(Instance);
        }

        internal readonly List<IPreprocessor> Preprocessors = new List<IPreprocessor>();
        internal readonly HashSet<String> AllowedGlobalExtensions = new HashSet<string>();
        internal readonly HashSet<String> AllowedScriptExtensions = new HashSet<string> { ".JS" };
        internal readonly HashSet<String> AllowedStyleExtensions = new HashSet<string> { ".CSS" };

        private IEnumerable<string> AllExtensions
        {
            get { return AllowedGlobalExtensions.Union(AllowedScriptExtensions).Union(AllowedStyleExtensions).Select(x => x.ToUpper()); }
        }

        /// <summary>
        /// Register a preprocessor instance to be used for all bundle types.
        /// </summary>
        /// <typeparam name="T"><see cref="IPreprocessor">IPreprocessor</see> implementation type.</typeparam>
        /// <param name="instance"><see cref="IPreprocessor">IPreprocessor</see> instance.</param>
        public void RegisterGlobalPreprocessor<T>(T instance) where T : IPreprocessor
        {
            ValidatePreprocessor<T>(instance);
            foreach (var ext in instance.Extensions)
            {
                AllowedGlobalExtensions.Add(ext.ToUpper());
            }
            Preprocessors.Add(instance);
            if (instance.IgnoreExtensions.NullSafeAny())
            {
                foreach (var ext in instance.IgnoreExtensions)
                {
                    AllowedGlobalExtensions.Add(ext.ToUpper());
                }
                Preprocessors.Add(new NullPreprocessor(instance.IgnoreExtensions));
            }
        }

        /// <summary>
        /// Register a preprocessor instance to be used for script bundles.
        /// </summary>
        /// <typeparam name="T"><see cref="IPreprocessor">IPreprocessor</see> implementation type.</typeparam>
        /// <param name="instance"><see cref="IPreprocessor">IPreprocessor</see> instance.</param>
        public void RegisterScriptPreprocessor<T>(T instance) where T : IPreprocessor
        {
            ValidatePreprocessor<T>(instance);
            foreach (var ext in instance.Extensions)
            {
                AllowedScriptExtensions.Add(ext.ToUpper());
            }
            Preprocessors.Add(instance);
            if (instance.IgnoreExtensions.NullSafeAny())
            {
                foreach (var ext in instance.IgnoreExtensions)
                {
                    AllowedScriptExtensions.Add(ext.ToUpper());
                }
                Preprocessors.Add(new NullPreprocessor(instance.IgnoreExtensions));
            }
        }

        /// <summary>
        /// Register a preprocessor instance to be used for all style bundles.
        /// </summary>
        /// <typeparam name="T"><see cref="IPreprocessor">IPreprocessor</see> implementation type.</typeparam>
        /// <param name="instance"><see cref="IPreprocessor">IPreprocessor</see> instance.</param>
        public void RegisterStylePreprocessor<T>(T instance) where T : IPreprocessor
        {
            ValidatePreprocessor<T>(instance);
            foreach (var ext in instance.Extensions)
            {
                AllowedStyleExtensions.Add(ext.ToUpper());
            }
            Preprocessors.Add(instance);
            if (instance.IgnoreExtensions.NullSafeAny())
            {
                foreach (var ext in instance.IgnoreExtensions)
                {
                    AllowedStyleExtensions.Add(ext.ToUpper());
                }
                Preprocessors.Add(new NullPreprocessor(instance.IgnoreExtensions));
            }
        }

        IEnumerable<IPreprocessor> ISquishItOptions.Preprocessors { get { return Preprocessors.AsEnumerable(); } }
        IEnumerable<string> ISquishItOptions.AllowedGlobalExtensions { get { return AllowedGlobalExtensions.AsEnumerable(); } }
        IEnumerable<string> ISquishItOptions.AllowedScriptExtensions { get { return AllowedScriptExtensions.AsEnumerable(); } }
        IEnumerable<string> ISquishItOptions.AllowedStyleExtensions { get { return AllowedStyleExtensions.AsEnumerable(); } }

        private void ValidatePreprocessor<T>(IPreprocessor instance)
        {
            if (Preprocessors.Any(p => p.GetType() == typeof(T)))
            {
                throw new InvalidOperationException(string.Format("Can't add multiple preprocessors of type: {0}", typeof(T).FullName));
            }

            foreach (var extension in instance.Extensions)
            {
                if (Enumerable.Contains(AllExtensions, extension))
                {
                    throw new InvalidOperationException(string.Format("Can't add multiple preprocessors for extension: {0}", extension));
                }
            }
        }

        public void ClearPreprocessors()
        {
            Preprocessors.Clear();

            AllowedGlobalExtensions.Clear();
            AllowedScriptExtensions.Clear();
            AllowedStyleExtensions.Clear();

            AllowedScriptExtensions.Add(".JS");
            AllowedStyleExtensions.Add(".CSS");
        }
    }
}