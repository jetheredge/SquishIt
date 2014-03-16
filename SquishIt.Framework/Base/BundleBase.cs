using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquishIt.Framework.Caches;
using SquishIt.Framework.Invalidation;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Base
{
    /// <summary>
    /// Base class for bundle implementations.  Configuration methods all return (T)this.
    /// </summary>
    /// <typeparam name="T">Type of bundle being implemented (Javascript or CSS).</typeparam>
    public abstract partial class BundleBase<T> : IRenderable where T : BundleBase<T>
    {
        static readonly Dictionary<string, string> renderPathCache = new Dictionary<string, string>();
        static readonly Dictionary<string, BundleState> bundleStateCache = new Dictionary<string, BundleState>();
        static readonly Dictionary<string, BundleState> rawContentBundleStateCache = new Dictionary<string, BundleState>();
 
        protected abstract IMinifier<T> DefaultMinifier { get; }
        protected abstract string tagFormat { get; }
        protected abstract string Template { get; }
        protected abstract string CachePrefix { get; }
        protected abstract IEnumerable<string> allowedExtensions { get; }
        protected abstract IEnumerable<string> disallowedExtensions { get; }
        protected abstract string defaultExtension { get; }
        protected string debugExtension { get { return ".squishit.debug" + defaultExtension.ToLowerInvariant(); } }
        protected abstract string ProcessFile(string file, string outputFile, Asset originalAsset);

        internal BundleState bundleState;
        readonly IContentCache bundleCache;
        readonly IContentCache rawContentCache;
        protected string BaseOutputHref = Configuration.Instance.DefaultOutputBaseHref();
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected IDirectoryWrapper directoryWrapper;
        protected IHasher hasher;
        protected IPathTranslator pathTranslator = Configuration.Instance.DefaultPathTranslator();

        IMinifier<T> minifier;

        protected IMinifier<T> Minifier
        {
            get { return minifier ?? DefaultMinifier; }
            set { minifier = value; }
        }

        protected BundleBase(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDebugStatusReader debugStatusReader, IDirectoryWrapper directoryWrapper, IHasher hasher, IContentCache bundleCache, IContentCache rawContentCache)
        {
            this.fileWriterFactory = fileWriterFactory;
            this.fileReaderFactory = fileReaderFactory;
            this.debugStatusReader = debugStatusReader;
            this.directoryWrapper = directoryWrapper;
            this.hasher = hasher;
            bundleState = new BundleState
                              {
                                  DebugPredicate = Configuration.Instance.DefaultDebugPredicate(),
                                  ShouldRenderOnlyIfOutputFileIsMissing = false,
                                  HashKeyName = Configuration.Instance.DefaultHashKeyName(),
                                  CacheInvalidationStrategy = Configuration.Instance.DefaultCacheInvalidationStrategy()
                              };
            this.bundleCache = bundleCache;
            this.rawContentCache = rawContentCache;
        }

        //TODO: should this be public?
        internal bool IsDebuggingEnabled()
        {
            return debugStatusReader.IsDebuggingEnabled(bundleState.DebugPredicate);
        }

        protected IRenderer GetFileRenderer()
        {
            return IsDebuggingEnabled() ? new FileRenderer(fileWriterFactory) :
                bundleState.ReleaseFileRenderer ??
                Configuration.Instance.DefaultReleaseRenderer() ??
                new FileRenderer(fileWriterFactory);
        }

        void AddAsset(Asset asset)
        {
            bundleState.Assets.Add(asset);
        }

        /// <summary>
        /// Specify that a bundle should be rendered without type="" in the html tag.
        /// </summary>
        public T WithoutTypeAttribute()
        {
            bundleState.Typeless = true;
            return (T)this;
        }

        /// <summary>
        /// Add a single file to a bundle.
        /// </summary>
        /// <param name="filePath">Path to file being added</param>
        public T Add(string filePath)
        {
            AddAsset(new Asset { LocalPath = filePath });
            return (T)this;
        }

        /// <summary>
        /// Add a single file that has already been minified to a bundle.  This will prevent the file from being minified again, a potential cause of bugs in combined file.
        /// </summary>
        /// <param name="filePath">Path to file being added</param>
        public T AddMinified(string filePath)
        {
            AddAsset(new Asset { LocalPath = filePath, Minify = false });
            return (T)this;
        }

        /// <summary>
        /// Add all files in a directory with extensions matching those known to bundle type.  Defaults to include subfolders.
        /// </summary>
        /// <param name="folderPath">Path to directory being added.</param>
        /// <param name="recursive">Include subfolders</param>
        public T AddDirectory(string folderPath, bool recursive = true)
        {
            return AddDirectory(folderPath, recursive, true);
        }

        /// <summary>
        /// Add all files in a directory with extensions matching those known to bundle type.  Defaults to include subfolders.  All files found will be considered pre-minified.   
        /// </summary>
        /// <param name="folderPath">Path to directory.</param>
        /// <param name="recursive">Include subfolders</param>
        public T AddMinifiedDirectory(string folderPath, bool recursive = true)
        {
            return AddDirectory(folderPath, recursive, false);
        }

        T AddDirectory(string folderPath, bool recursive, bool minify)
        {
            AddAsset(new Asset { LocalPath = folderPath, IsRecursive = recursive, Minify = minify });
            return (T)this;
        }

        /// <summary>
        /// Add arbitrary content that is not saved on disk.
        /// </summary>
        /// <param name="content">Content to include in bundle.</param>
        public T AddString(string content)
        {
            return AddString(content, defaultExtension, true);
        }

        /// <summary>
        /// Add arbitrary content that is not saved on disk with the assumption that it is treated as if found in a given directory.  This is useful for adding LESS content that needs to get imports relative to a particular location.
        /// </summary>
        /// <param name="content">Content to include in bundle.</param>
        /// <param name="extension">Extension that would be included in filename if content were saved to disk - this is needed to determine if the content should be preprocessed.</param>
        /// <param name="currentDirectory">Folder that file would reside in if content were saved to disk - this is used for processing relative imports within arbitrary content.</param>
        public T AddString(string content, string extension, string currentDirectory = null)
        {
            return AddString(content, extension, true, currentDirectory);
        }

        /// <summary>
        /// Add pre-minified arbitrary content (not saved on disk).
        /// </summary>
        /// <param name="content">Minified content to include in bundle.</param>
        public T AddMinifiedString(string content)
        {
            return AddString(content, defaultExtension, false);
        }

        /// <summary>
        /// Add pre-minified arbitrary content (not saved on disk) with the assumption that it is treated as if found in a given directory.  This is useful for adding LESS content that needs to get imports relative to a particular location.
        /// </summary>
        /// <param name="content">Minified content to include in bundle.</param>
        /// <param name="extension">Extension that would be included in filename if content were saved to disk - this is needed to determine if the content should be preprocessed.</param>
        /// <param name="currentDirectory">Folder that file would reside in if content were saved to disk - this is used for processing relative imports within arbitrary content.</param>
        public T AddMinifiedString(string content, string extension)
        {
            return AddString(content, extension, false);
        }

        T AddString(string content, string extension, bool minify, string currentDirectory = null)
        {
            if (bundleState.Assets.All(ac => ac.Content != content))
                bundleState.Assets.Add(new Asset { Content = content, Extension = extension, Minify = minify, ArbitraryWorkingDirectory = currentDirectory });
            return (T)this;
        }

        /// <summary>
        /// Add arbitrary content (not saved on disk) using string.Format to inject values.
        /// </summary>
        /// <param name="format">Content to include in bundle.</param>
        /// <param name="values">Values to be injected using string.Format.</param>
        public T AddString(string format, object[] values)
        {
            return AddString(format, defaultExtension, values);
        }

        /// <summary>
        /// Add arbitrary content (not saved on disk) using string.Format to inject values.
        /// </summary>
        /// <param name="format">Content to include in bundle.</param>
        /// <param name="extension">Extension that would be included in filename if content were saved to disk - this is needed to determine if the content should be preprocessed.</param>
        /// <param name="values">Values to be injected using string.Format.</param>
        public T AddString(string format, string extension, object[] values)
        {
            var content = string.Format(format, values);
            return AddString(content, extension);
        }

        /// <summary>
        /// Add a remote asset to bundle.
        /// </summary>
        /// <param name="localPath">Path to treat asset as if it comes from.</param>
        /// <param name="remotePath">URL to remote asset.</param>
        public T AddRemote(string localPath, string remotePath)
        {
            return AddRemote(localPath, remotePath, false);
        }

        /// <summary>
        /// Add a remote asset to bundle.
        /// </summary>
        /// <param name="localPath">Path to treat asset as if it comes from.</param>
        /// <param name="remotePath">URL to remote asset.</param>
        /// <param name="downloadRemote">Fetch remote content to include in bundle.</param>
        public T AddRemote(string localPath, string remotePath, bool downloadRemote)
        {
            var asset = new Asset
            {
                LocalPath = localPath,
                RemotePath = remotePath,
                DownloadRemote = downloadRemote
            };
            AddAsset(asset);
            return (T)this;
        }

        /// <summary>
        /// Add dynamic (app-generated) content - the generated proxy file SignalR serves to clients is a good example.
        /// </summary>
        /// <param name="siteRelativePath">Site-relative path to content (eg "signalr/hubs").</param>
        public T AddDynamic(string siteRelativePath)
        {
            var absolutePath = BuildAbsolutePath(siteRelativePath);
            return AddRemote(siteRelativePath, absolutePath, true);
        }

        /// <summary>
        /// Add embedded resource in root namespace.
        /// </summary>
        /// <param name="localPath">Path to treat asset as if it comes from.</param>
        /// <param name="embeddedResourcePath">Path to resource embedded in root namespace (eg "WebForms.js").</param>
        public T AddRootEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddAsset(new Asset { LocalPath = localPath, RemotePath = embeddedResourcePath, Order = 0, IsEmbeddedResource = true, IsEmbeddedInRootNamespace = true });
            return (T)this;
        }

        /// <summary>
        /// Add embedded resource.
        /// </summary>
        /// <param name="localPath">Path to treat asset as if it comes from.</param>
        /// <param name="embeddedResourcePath">Path to embedded resource (eg "SquishIt.Tests://EmbeddedResource.Embedded.css").</param>
        public T AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddAsset(new Asset { LocalPath = localPath, RemotePath = embeddedResourcePath, Order = 0, IsEmbeddedResource = true });
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to bypass writing to disk if the output file already exists.
        /// </summary>
        public T RenderOnlyIfOutputFileMissing()
        {
            bundleState.ShouldRenderOnlyIfOutputFileIsMissing = true;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to always render in debug mode (assets served separately and unminified).
        /// </summary>
        public T ForceDebug()
        {
            debugStatusReader.ForceDebug();
            bundleState.ForceDebug = true;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to render in debug mode (assets served separately and unminified) if a precondition is met.
        /// </summary>
        public T ForceDebugIf(Func<bool> predicate)
        {
            bundleState.DebugPredicate = predicate;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to always render in release mode (assets combined and minified).
        /// </summary>
        public T ForceRelease()
        {
            debugStatusReader.ForceRelease();
            bundleState.ForceRelease = true;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to prefix paths with given base URL - this is useful for cdn scenarios.
        /// </summary>
        /// <param name="href">Base path to CDN (eg "http://static.myapp.com").</param>
        public T WithOutputBaseHref(string href)
        {
            BaseOutputHref = href;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to use a non-standard file renderer.  This is useful if you want combined files uploaded to a static server or CDN.
        /// </summary>
        /// <param name="renderer">Implementation of <see cref="IRenderer">IRenderer</see> to be used when creating combined file.</param>
        public T WithReleaseFileRenderer(IRenderer renderer)
        {
            bundleState.ReleaseFileRenderer = renderer;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to use a non-standard cache invalidation strategy.
        /// </summary>
        /// <param name="strategy">Implementation of <see cref="ICacheInvalidationStrategy">ICacheInvalidationStrategy</see> to be used when generating content tag (eg <see cref="HashAsVirtualDirectoryCacheInvalidationStrategy">HashAsVirtualDirectoryCacheInvalidationStrategy</see>)</param>
        public T WithCacheInvalidationStrategy(ICacheInvalidationStrategy strategy)
        {
            bundleState.CacheInvalidationStrategy = strategy;
            return (T)this;
        }

        void AddAttributes(Dictionary<string, string> attributes, bool merge = true)
        {
            if (merge)
            {
                foreach (var attribute in attributes)
                {
                    bundleState.Attributes[attribute.Key] = attribute.Value;
                }
            }
            else
            {
                bundleState.Attributes = attributes;
            }
        }

        /// <summary>
        /// Include a given HTML attribute in rendered tag.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        public T WithAttribute(string name, string value)
        {
            AddAttributes(new Dictionary<string, string> { { name, value } });
            return (T)this;
        }

        /// <summary>
        /// Include a given HTML attribute in rendered tag.
        /// </summary>
        /// <param name="attributes">Attribute name/value pairs.</param>
        /// <param name="merge">Merge with attributes already added (false will overwrite).</param>
        public T WithAttributes(Dictionary<string, string> attributes, bool merge = true)
        {
            AddAttributes(attributes, merge: merge);
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to use a type other than the default minifier for given bundle type.
        /// </summary>
        /// <typeparam name="TMin">Type of <see cref="IMinifier">IMinifier</see> to use.</typeparam>
        public T WithMinifier<TMin>() where TMin : IMinifier<T>
        {
            Minifier = MinifierFactory.Get<T, TMin>();
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to use a minifier instance.
        /// </summary>
        /// <typeparam name="TMin">Instance of <see cref="IMinifier">IMinifier</see> to use.</typeparam>
        public T WithMinifier<TMin>(TMin minifier) where TMin : IMinifier<T>
        {
            Minifier = minifier;
            return (T)this;
        }

        string FillTemplate(BundleState bundleState, string path)
        {
            return string.Format(Template, GetAdditionalAttributes(bundleState), path);
        }

        /// <summary>
        /// Configure bundle to use a specific name for cache-breaking parameter (only used with querystring invalidation).
        /// </summary>
        /// <param name="hashQueryStringKeyName">Name of parameter to be added to content URLs.</param>
        public T HashKeyNamed(string hashQueryStringKeyName)
        {
            bundleState.HashKeyName = hashQueryStringKeyName;
            return (T)this;
        }

        /// <summary>
        /// Configure bundle to bypass cache invalidation.
        /// </summary>
        public T WithoutRevisionHash()
        {
            return HashKeyNamed(string.Empty);
        }

        /// <summary>
        /// Configure bundle to use provided preprocessor instance.
        /// </summary>
        /// <param name="instance"><see cref="IPreprocessor">IPreprocessor</see> to use when rendering bundle.</param>
        /// <returns></returns>
        public T WithPreprocessor(IPreprocessor instance)
        {
            bundleState.AddPreprocessor(instance);
            return (T)this;
        }

        protected abstract void AggregateContent(List<Asset> assets, StringBuilder sb, string outputFile);

        BundleState GetCachedBundleState(string name)
        {
            var bundle = bundleStateCache[CachePrefix + name];
            if (bundle.ForceDebug)
            {
                debugStatusReader.ForceDebug();
            }
            if (bundle.ForceRelease)
            {
                debugStatusReader.ForceRelease();
            }
            return bundle;
        }

        /// <summary>
        /// Render bundle to a file.
        /// </summary>
        /// <param name="renderTo">Path to combined file.</param>
        /// <returns>HTML tag.</returns>
        public string Render(string renderTo)
        {
            string key = renderTo;
            return Render(renderTo, key, GetFileRenderer());
        }

        /// <summary>
        /// Render tag for a cached bundle.
        /// </summary>
        /// <param name="name">Name of cached bundle.</param>
        /// <returns>HTML tag.</returns>
        public string RenderCachedAssetTag(string name)
        {
            bundleState = GetCachedBundleState(name);
            return Render(null, name, new CacheRenderer(CachePrefix, name));
        }

        /// <summary>
        /// Render bundle into the cache with a given name.
        /// </summary>
        /// <param name="name">Name of bundle in cache.</param>
        /// <param name="renderToFilePath">File system path that cached bundle would be rendered to (for import processing).</param>
        public void AsNamed(string name, string renderToFilePath)
        {
            Render(renderToFilePath, name, GetFileRenderer());
            bundleState.Path = renderToFilePath;
            bundleStateCache[CachePrefix + name] = bundleState;
        }

        /// <summary>
        /// Render bundle into cache and return tag.
        /// </summary>
        /// <param name="name">Name of bundle in cache.</param>
        /// <param name="renderToFilePath">File system path that cached bundle would be rendered to (for import processing).</param>
        /// <returns>HTML tag.</returns>
        public string AsCached(string name, string renderToFilePath)
        {
            string result = Render(renderToFilePath, name, new CacheRenderer(CachePrefix, name));
            bundleState.Path = renderToFilePath;
            bundleStateCache[CachePrefix + name] = bundleState;
            return result;
        }

        /// <summary>
        /// Render bundle with a given name.
        /// </summary>
        /// <param name="name">Name for bundle.</param>
        /// <returns>HTML tag.</returns>
        public string RenderNamed(string name)
        {
            bundleState = GetCachedBundleState(name);

            if (!bundleState.DebugPredicate.SafeExecute())
            {
                // Revisit https://github.com/jetheredge/SquishIt/pull/155 and https://github.com/jetheredge/SquishIt/issues/183
                //hopefully we can find a better way to satisfy both of these requirements
                var fullName = (BaseOutputHref ?? "") + CachePrefix + name;
                var content = bundleCache.GetContent(fullName);
                if (content == null)
                {
                    AsNamed(name, bundleState.Path);
                    return bundleCache.GetContent(CachePrefix + name);
                }
                return content;
            }
            return RenderDebug(bundleState.Path, name, GetFileRenderer());
        }

        /// <summary>
        /// Render bundle from cache with a given name.
        /// </summary>
        /// <param name="name">Name for cached bundle.</param>
        /// <returns>HTML tag.</returns>
        public string RenderCached(string name)
        {
            bundleState = GetCachedBundleState(name);
            var content = CacheRenderer.Get(CachePrefix, name);
            if (content == null)
            {
                AsCached(name, bundleState.Path);
                return CacheRenderer.Get(CachePrefix, name);
            }
            return content;
        }

        public void ClearCache()
        {
            bundleCache.ClearTestingCache();
        }

        /// <summary>
        /// Retrieve number of assets included in bundle.
        /// </summary>
        public int AssetCount
        {
            get
            {
                return bundleState == null ? 0
                    : bundleState.Assets == null ? 0 
                    : bundleState.Assets.Count;
            }
        }

        /// <summary>
        /// Render 'raw' content directly without building tags or writing to files (and save in cache by name)
        /// </summary>
        /// <returns>String representation of content, minified if needed.</returns>
        public string RenderRawContent(string bundleName)
        {
            var cacheKey = CachePrefix + "_raw_" + bundleName;

            string content;

            if (rawContentCache.ContainsKey(cacheKey))
            {
                rawContentCache.Remove(cacheKey);
            }
            if (rawContentBundleStateCache.ContainsKey(cacheKey))
            {
                rawContentBundleStateCache.Remove(cacheKey);
            }

            content = GetMinifiedContent(bundleState.Assets, string.Empty);
            rawContentCache.Add(cacheKey, content, bundleState.DependentFiles, IsDebuggingEnabled());
            rawContentBundleStateCache.Add(cacheKey, bundleState);
            return content;
        }

        /// <summary>
        /// Render cached 'raw' bundle content.
        /// </summary>
        /// <param name="bundleName">String representation of content according to cache.</param>
        /// <returns></returns>
        public string RenderCachedRawContent(string bundleName)
        {
            var cacheKey = CachePrefix + "_raw_" + bundleName;

            var output = rawContentCache.GetContent(cacheKey);
            if (output == null)
            {
                bundleState = rawContentBundleStateCache[cacheKey];
                if (bundleState == null)
                {
                    throw new InvalidOperationException(string.Format("No cached bundle state named {0} was found.", bundleName));
                }
                output = RenderRawContent(bundleName);
            }
            return output;
        }
    }
}