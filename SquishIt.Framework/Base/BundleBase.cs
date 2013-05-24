using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquishIt.Framework.Invalidation;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Base
{
    public abstract partial class BundleBase<T> where T : BundleBase<T>
    {
        static readonly Dictionary<string, string> renderPathCache = new Dictionary<string, string>();
        static readonly Dictionary<string, BundleState> bundleStateCache = new Dictionary<string, BundleState>();

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
        readonly IBundleCache bundleCache;
        protected string BaseOutputHref = Configuration.Instance.DefaultOutputBaseHref();
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected IDirectoryWrapper directoryWrapper;
        protected IHasher hasher;

        IMinifier<T> minifier;

        protected IMinifier<T> Minifier
        {
            get { return minifier ?? DefaultMinifier; }
            set { minifier = value; }
        }

        protected BundleBase(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDebugStatusReader debugStatusReader, IDirectoryWrapper directoryWrapper, IHasher hasher, IBundleCache bundleCache)
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
        }

        protected bool IsDebuggingEnabled()
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

        public T WithoutTypeAttribute()
        {
            bundleState.Typeless = true;
            return (T)this;
        }

        public T Add(string filePath)
        {
            AddAsset(new Asset { LocalPath = filePath });
            return (T)this;
        }

        public T AddMinified(string filePath)
        {
            AddAsset(new Asset { LocalPath = filePath, Minify = false });
            return (T)this;
        }

        public T AddDirectory(string folderPath, bool recursive = true)
        {
            return AddDirectory(folderPath, recursive, true);
        }

        public T AddMinifiedDirectory(string folderPath, bool recursive = true)
        {
            return AddDirectory(folderPath, recursive, false);
        }

        T AddDirectory(string folderPath, bool recursive, bool minify)
        {
            AddAsset(new Asset { LocalPath = folderPath, IsRecursive = recursive, Minify = minify });
            return (T)this;
        }

        public T AddString(string content)
        {
            return AddString(content, defaultExtension, true);
        }

        public T AddString(string content, string extension)
        {
            return AddString(content, extension, true);
        }

        public T AddMinifiedString(string content)
        {
            return AddString(content, defaultExtension, false);
        }

        public T AddMinifiedString(string content, string extension)
        {
            return AddString(content, extension, false);
        }

        T AddString(string content, string extension, bool minify)
        {
            if (bundleState.Assets.All(ac => ac.Content != content))
                bundleState.Assets.Add(new Asset { Content = content, Extension = extension, Minify = minify });
            return (T)this;
        }

        public T AddString(string format, object[] values)
        {
            return AddString(format, defaultExtension, values);
        }

        public T AddString(string format, string extension, object[] values)
        {
            var content = string.Format(format, values);
            return AddString(content, extension);
        }

        public T AddRemote(string localPath, string remotePath)
        {
            return AddRemote(localPath, remotePath, false);
        }

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

        public T AddDynamic(string siteRelativePath)
        {
            var absolutePath = BuildAbsolutePath(siteRelativePath);
            return AddRemote(siteRelativePath, absolutePath, true);
        }

        public T AddRootEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddAsset(new Asset { LocalPath = localPath, RemotePath = embeddedResourcePath, Order = 0, IsEmbeddedResource = true, IsEmbeddedInRootNamespace = true });
            return (T)this;
        }

        public T AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddAsset(new Asset { LocalPath = localPath, RemotePath = embeddedResourcePath, Order = 0, IsEmbeddedResource = true });
            return (T)this;
        }

        public T RenderOnlyIfOutputFileMissing()
        {
            bundleState.ShouldRenderOnlyIfOutputFileIsMissing = true;
            return (T)this;
        }

        public T ForceDebug()
        {
            debugStatusReader.ForceDebug();
            bundleState.ForceDebug = true;
            return (T)this;
        }

        public T ForceDebugIf(Func<bool> predicate)
        {
            bundleState.DebugPredicate = predicate;
            return (T)this;
        }

        public T ForceRelease()
        {
            debugStatusReader.ForceRelease();
            bundleState.ForceRelease = true;
            return (T)this;
        }

        public T WithOutputBaseHref(string href)
        {
            BaseOutputHref = href;
            return (T)this;
        }

        public T WithReleaseFileRenderer(IRenderer renderer)
        {
            bundleState.ReleaseFileRenderer = renderer;
            return (T)this;
        }

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

        public T WithAttribute(string name, string value)
        {
            AddAttributes(new Dictionary<string, string> { { name, value } });
            return (T)this;
        }

        public T WithAttributes(Dictionary<string, string> attributes, bool merge = true)
        {
            AddAttributes(attributes, merge: merge);
            return (T)this;
        }

        public T WithMinifier<TMin>() where TMin : IMinifier<T>
        {
            Minifier = MinifierFactory.Get<T, TMin>();
            return (T)this;
        }

        public T WithMinifier<TMin>(TMin minifier) where TMin : IMinifier<T>
        {
            Minifier = minifier;
            return (T)this;
        }

        string FillTemplate(BundleState bundleState, string path)
        {
            return string.Format(Template, GetAdditionalAttributes(bundleState), path);
        }

        public T HashKeyNamed(string hashQueryStringKeyName)
        {
            bundleState.HashKeyName = hashQueryStringKeyName;
            return (T)this;
        }

        public T WithoutRevisionHash()
        {
            return HashKeyNamed(string.Empty);
        }

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


        public string Render(string renderTo)
        {
            string key = renderTo;
            return Render(renderTo, key, GetFileRenderer());
        }

        public string RenderCachedAssetTag(string name)
        {
            bundleState = GetCachedBundleState(name);
            return Render(null, name, new CacheRenderer(CachePrefix, name));
        }

        public void AsNamed(string name, string renderTo)
        {
            Render(renderTo, name, GetFileRenderer());
            bundleState.Path = renderTo;
            bundleStateCache[CachePrefix + name] = bundleState;
        }

        public string AsCached(string name, string filePath)
        {
            string result = Render(filePath, name, new CacheRenderer(CachePrefix, name));
            bundleState.Path = filePath;
            bundleStateCache[CachePrefix + name] = bundleState;
            return result;
        }

        public string RenderNamed(string name)
        {
            bundleState = GetCachedBundleState(name);

            if (!bundleState.DebugPredicate.SafeExecute())
            {
                //TODO: this sucks
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
    }
}