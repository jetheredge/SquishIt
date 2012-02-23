using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Base
{
    public abstract class BundleBase<T> where T : BundleBase<T>
    {
        private static readonly Dictionary<string, string> renderPathCache = new Dictionary<string, string>();

        private const string DEFAULT_GROUP = "default";
        protected string BaseOutputHref = String.Empty;
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected ICurrentDirectoryWrapper currentDirectoryWrapper;
        protected IHasher hasher;
        protected abstract IMinifier<T> DefaultMinifier { get; }
        protected abstract HashSet<string> allowedExtensions { get; }
        protected abstract string tagFormat { get; }
        protected HashSet<string> arbitrary = new HashSet<string>();
        protected bool typeless;
        protected abstract string Template { get; }
        protected abstract string CachePrefix { get; }

        private IMinifier<T> minifier;
        protected IMinifier<T> Minifier
        {
            get
            {
                return minifier ?? DefaultMinifier;
            }
            set { minifier = value; }
        }

        protected string HashKeyName { get; set; }
        private bool ShouldRenderOnlyIfOutputFileIsMissing { get; set; }
        internal List<string> DependentFiles = new List<string>();
        internal BundleState bundleState = new BundleState();
        
        private static Dictionary<string,BundleState> bundleStateCache = new Dictionary<string,BundleState>();

        private IBundleCache bundleCache;

        protected BundleBase(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDebugStatusReader debugStatusReader, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache)
        {
            this.fileWriterFactory = fileWriterFactory;
            this.fileReaderFactory = fileReaderFactory;
            this.debugStatusReader = debugStatusReader;
            this.currentDirectoryWrapper = currentDirectoryWrapper;
            this.hasher = hasher;
            ShouldRenderOnlyIfOutputFileIsMissing = false;
            HashKeyName = "r";
            this.bundleCache = bundleCache;
        }

        private List<string> GetFiles(List<Asset> assets)
        {
            var inputFiles = GetInputFiles(assets);
            var resolvedFilePaths = new List<string>();

            foreach (Input input in inputFiles)
            {
                resolvedFilePaths.AddRange(input.TryResolve(allowedExtensions));
            }

            return resolvedFilePaths;
        }

        private Input GetInputFile(Asset asset)
        {
            if (!asset.IsEmbeddedResource)
            {
                if (debugStatusReader.IsDebuggingEnabled())
                {
                    return GetFileSystemPath(asset.LocalPath);
                }

                if (asset.IsRemoteDownload)
                {
                    return GetHttpPath(asset.RemotePath);
                }
                else
                {
                    return GetFileSystemPath(asset.LocalPath);
                }
            }
            else
            {
                return GetEmbeddedResourcePath(asset.RemotePath);
            }
        }

        private List<Input> GetInputFiles(List<Asset> assets)
        {
            var inputFiles = new List<Input>();
            foreach (var asset in assets)
            {
                inputFiles.Add(GetInputFile(asset));
            }
            return inputFiles;
        }

        private Input GetFileSystemPath(string localPath)
        {
            string mappedPath = FileSystem.ResolveAppRelativePathToFileSystem(localPath);
            return new Input(mappedPath, ResolverFactory.Get<FileSystemResolver>());
        }

        private Input GetHttpPath(string remotePath)
        {
            return new Input(remotePath, ResolverFactory.Get<HttpResolver>());
        }

        private Input GetEmbeddedResourcePath(string resourcePath)
        {
            return new Input(resourcePath, ResolverFactory.Get<EmbeddedResourceResolver>());
        }

        protected static IPreprocessor FindPreprocessor(string file) {
            return Bundle.Preprocessors.FirstOrDefault(p => p.ValidFor(file));
        }

        private string ExpandAppRelativePath(string file)
        {
            if (file.StartsWith("~/"))
            {
                string appRelativePath = HttpRuntime.AppDomainAppVirtualPath;
                if (appRelativePath != null && !appRelativePath.EndsWith("/"))
                    appRelativePath += "/";
                return file.Replace("~/", appRelativePath);
            }
            return file;
        }

        protected string ReadFile(string file)
        {
            using (var sr = fileReaderFactory.GetFileReader(file))
            {
                return sr.ReadToEnd();
            }
        }

        protected bool FileExists(string file)
        {
            return fileReaderFactory.FileExists(file);
        }

        private string GetAdditionalAttributes(BundleState bundleState)
        {
            var result = new StringBuilder();
            foreach (var attribute in bundleState.Attributes)
            {
                result.Append(attribute.Key);
                result.Append("=\"");
                result.Append(attribute.Value);
                result.Append("\" ");
            }
            return result.ToString();
        }

        private string GetFilesForRemote(List<string> remoteAssetPaths, BundleState bundleState)
        {
            var sb = new StringBuilder();
            foreach (var uri in remoteAssetPaths)
            {
                sb.Append(FillTemplate(bundleState, uri));
            }

            return sb.ToString();
        }

        private void AddAsset(Asset asset)
        {
            bundleState.Assets.Add(asset);
        }

        public T WithoutTypeAttribute () {
            this.typeless = true;
            return (T)this;
        }

        [Obsolete]
        public T Add(params string[] filesPath)
        {
            foreach (var filePath in filesPath)
                Add(filePath);

            return (T)this;
        }

        public T Add(string fileOrFolderPath)
        {
            AddAsset(new Asset(fileOrFolderPath));
            return (T)this;
        }

        public T AddString(string content)
        {
            arbitrary.Add(content);
            return (T)this;
        }

        public T AddString(string format, params object[] values)
        {
            var content = string.Format(format, values);
            return AddString(content);
        }
        
        public T AddRemote(string localPath, string remotePath)
        {
            return AddRemote(localPath, remotePath, false);
        }

        public T AddRemote(string localPath, string remotePath, bool downloadRemote)
        {
            var asset = new Asset(localPath, remotePath);
            asset.DownloadRemote = downloadRemote;
            AddAsset(asset);
            return (T)this;
        }

        public T AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddAsset(new Asset(localPath, embeddedResourcePath, 0, true));
            return (T)this;
        }

        public T RenderOnlyIfOutputFileMissing()
        {
            ShouldRenderOnlyIfOutputFileIsMissing = true;
            return (T)this;
        }

        public T ForceDebug()
        {
            debugStatusReader.ForceDebug();
            bundleState.ForceDebug = true;
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

        public string Render(string renderTo)
        {
            string key = renderTo;
            return Render(renderTo, key, new FileRenderer(fileWriterFactory));
        }

        private string Render(string renderTo, string key, IRenderer renderer)
        {
            key = CachePrefix + key;
            if (!String.IsNullOrEmpty(BaseOutputHref))
            {
                key = BaseOutputHref + key;
            }

            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(key);
            }
            return RenderRelease(key, renderTo, renderer);
        }

        public string RenderNamed(string name)
        {
            bundleState = GetCachedGroupBundle(name);
            var content = bundleCache.GetContent(CachePrefix + name);
            if (content == null)
            {
                AsNamed(name, bundleState.Path);
                return bundleCache.GetContent(CachePrefix + name);
            }
            return content;
        }

        public string RenderCached(string name)
        {
            bundleState = GetCachedGroupBundle(name);
            var content = CacheRenderer.Get(CachePrefix, name);
            if (content == null)
            {
                AsCached(name, bundleState.Path);
                return CacheRenderer.Get(CachePrefix, name);
            }
            return content;
        }

        public string RenderCachedAssetTag(string name)
        {
            bundleState = GetCachedGroupBundle(name);
            return Render(null, name, new CacheRenderer(CachePrefix, name));
        }

        public void AsNamed(string name, string renderTo)
        {
            Render(renderTo, name, new FileRenderer(fileWriterFactory));
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

        protected string RenderDebug(string name = null)
        {
            string content = null;
            
            DependentFiles.Clear();

            var renderedFiles = new HashSet<string>();

            BeforeRenderDebug();

            var sb = new StringBuilder();
            var attributes = GetAdditionalAttributes(bundleState);
            var assets = bundleState.Assets;

            DependentFiles.AddRange(GetFiles(assets));
            foreach (var asset in assets)
            {
                var inputFile = GetInputFile(asset);
                var files = inputFile.TryResolve(allowedExtensions);

                if (asset.IsEmbeddedResource)
                {
                    var tsb = new StringBuilder();

                    foreach (var fn in files)
                    {
                        tsb.Append(ReadFile(fn) + "\n\n\n");
                    }

                    var renderer = new FileRenderer(fileWriterFactory);
                    var processedFile = ExpandAppRelativePath(asset.LocalPath);
                    renderer.Render(tsb.ToString(), FileSystem.ResolveAppRelativePathToFileSystem(processedFile));
                    sb.AppendLine(FillTemplate(bundleState, processedFile));
                }
                else if (asset.RemotePath != null)
                {
                    sb.AppendLine(FillTemplate(bundleState, ExpandAppRelativePath(asset.LocalPath)));
                }
                else
                {
                    foreach (var file in files)
                    {
                        if (!renderedFiles.Contains(file))
                        {
                            var fileBase = FileSystem.ResolveAppRelativePathToFileSystem(asset.LocalPath);
                            var newPath = file.Replace(fileBase, "");
                            var path = ExpandAppRelativePath(asset.LocalPath + newPath.Replace("\\", "/"));
                            sb.AppendLine(FillTemplate(bundleState, path));
                            renderedFiles.Add(file);
                        }
                    }
                }
            }

            foreach (var cntnt in arbitrary)
            {
                sb.AppendLine(string.Format(tagFormat, cntnt));
            }

            content = sb.ToString();

            if (bundleCache.ContainsKey(name))
            {
                bundleCache.Remove(name);
            }
            bundleCache.Add(name, content, DependentFiles);

            return content;
        }

        private string RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            string content;
            if (!bundleCache.TryGetValue(key, out content))
            {
                using(new CriticalRenderingSection(renderTo))
                {
                    if (!bundleCache.TryGetValue(key, out content))
                    {
                        var uniqueFiles = new List<string>();
                        string minifiedContent = null;
                        string hash = null;
                        bool hashInFileName = false;

                        DependentFiles.Clear();

                        if (renderTo == null)
                        {
                            renderTo = renderPathCache[CachePrefix + "." + key];
                        }
                        else
                        {
                            renderPathCache[CachePrefix + "." + key] = renderTo;
                        }

                        string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(renderTo);
                        var renderToPath = ExpandAppRelativePath(renderTo);

                        if (!String.IsNullOrEmpty(BaseOutputHref))
                        {
                            renderToPath = String.Concat(BaseOutputHref.TrimEnd('/'), "/", renderToPath.TrimStart('/'));
                        }

                        var remoteAssetPaths = new List<string>();
                        foreach (var asset in bundleState.Assets)
                        {
                            if (asset.IsRemote)
                            {
                                remoteAssetPaths.Add(asset.RemotePath);
                            }
                        }

                        uniqueFiles.AddRange(GetFiles(bundleState.Assets.Where(asset =>
                            asset.IsEmbeddedResource ||
                            asset.IsLocal ||
                            asset.IsRemoteDownload).ToList()).Distinct());

                        DependentFiles.AddRange(uniqueFiles);

                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            minifiedContent = Minifier.Minify(BeforeMinify(outputFile, uniqueFiles, arbitrary));
                            hash = hasher.GetHash(minifiedContent);
                            renderToPath = renderToPath.Replace("#", hash);
                            outputFile = outputFile.Replace("#", hash);
                        }

                        if (ShouldRenderOnlyIfOutputFileIsMissing && FileExists(outputFile))
                        {
                            minifiedContent = ReadFile(outputFile);
                        }
                        else
                        {
                            minifiedContent = minifiedContent ?? Minifier.Minify(BeforeMinify(outputFile, uniqueFiles, arbitrary));
                            renderer.Render(minifiedContent, outputFile);
                        }

                        if (hash == null && !string.IsNullOrEmpty(HashKeyName))
                        {
                            hash = hasher.GetHash(minifiedContent);
                        }

                        string renderedTag;
                        if (hashInFileName)
                        {
                            renderedTag = FillTemplate(bundleState, renderToPath);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(HashKeyName))
                            {
                                renderedTag = FillTemplate(bundleState, renderToPath);
                            }
                            else if (renderToPath.Contains("?"))
                            {
                                renderedTag = FillTemplate(bundleState, renderToPath + "&" + HashKeyName + "=" + hash);
                            }
                            else
                            {
                                renderedTag = FillTemplate(bundleState, renderToPath + "?" + HashKeyName + "=" + hash);
                            }
                        }

                        content += String.Concat(GetFilesForRemote(remoteAssetPaths, bundleState), renderedTag);
                    }
                }
                bundleCache.Add(key, content, DependentFiles);
            }

            return content;
        }

        public void ClearCache()
        {
            bundleCache.ClearTestingCache();
        }

        private void AddAttributes(Dictionary<string, string> attributes, bool merge = true)
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

        private string FillTemplate(BundleState bundleState, string path)
        {
            return string.Format(Template, GetAdditionalAttributes(bundleState), path);
        }

        public T HashKeyNamed(string hashQueryStringKeyName)
        {
            HashKeyName = hashQueryStringKeyName;
            return (T)this;
        }

        protected virtual string BeforeMinify(string outputFile, List<string> files, IEnumerable<string> arbitraryContent)
        {
            var sb = new StringBuilder();
            var allContent = files.Select(ReadFile).Union(arbitraryContent);
            foreach (var content in allContent)
            {
                sb.Append(ReadFile(content) + "\n");
            }

            return sb.ToString();
        }

        internal virtual void BeforeRenderDebug()
        {
            
        }

        private BundleState GetCachedGroupBundle(string name)
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
    }
}