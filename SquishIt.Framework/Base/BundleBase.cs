using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private static Dictionary<string, string> renderPathCache = new Dictionary<string, string>();

        private const string DEFAULT_GROUP = "default";
        protected string BaseOutputHref = String.Empty;
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected ICurrentDirectoryWrapper currentDirectoryWrapper;
        protected IHasher hasher;
        protected abstract IMinifier<T> DefaultMinifier { get; }
        protected abstract string[] allowedExtensions { get; }
        protected abstract string tagFormat { get; }
        protected HashSet<string> arbitrary = new HashSet<string>();

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
        protected List<string> DependentFiles = new List<string>();
        internal Dictionary<string, GroupBundle> GroupBundles = new Dictionary<string, GroupBundle>
        {
            { DEFAULT_GROUP, new GroupBundle() }
        };

        private static Dictionary<string,Dictionary<string, GroupBundle>> groupBundlesCache = new Dictionary<string, Dictionary<string, GroupBundle>>();

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

        private string GetAdditionalAttributes(GroupBundle groupBundle)
        {
            var result = new StringBuilder();
            foreach (var attribute in groupBundle.Attributes)
            {
                result.Append(attribute.Key);
                result.Append("=\"");
                result.Append(attribute.Value);
                result.Append("\" ");
            }
            return result.ToString();
        }

        private string GetFilesForRemote(List<string> remoteAssetPaths, GroupBundle groupBundle)
        {
            var sb = new StringBuilder();
            foreach (var uri in remoteAssetPaths)
            {
                sb.Append(FillTemplate(groupBundle, uri));
            }

            return sb.ToString();
        }

        private void AddAsset(Asset asset, string group = DEFAULT_GROUP)
        {
            GroupBundle groupBundle;
            if (GroupBundles.TryGetValue(group, out groupBundle))
            {
                groupBundle.Assets.Add(asset);
            }
            else
            {
                groupBundle = new GroupBundle();
                groupBundle.Assets.Add(asset);
                GroupBundles[group] = groupBundle;
            }
        }

        public T Add(params string[] filesPath)
        {
            foreach (var filePath in filesPath)
                Add(filePath);

            return (T)this;
        }

        public T Add(string filePath)
        {
            AddAsset(new Asset(filePath));
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

        public T AddToGroup(string group, params string[] filesPath)
        {
            foreach (var filePath in filesPath)
                AddToGroup(group, filePath);

            return (T)this;
        }

        public T AddToGroup(string group, string filePath)
        {
            AddAsset(new Asset(filePath), group);
            return (T)this;
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
            return (T)this;
        }

        public T ForceRelease()
        {
            debugStatusReader.ForceRelease();
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

            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(key);
            }
            return RenderRelease(key, renderTo, renderer);
        }

        public string RenderNamed(string name)
        {
            GroupBundles = groupBundlesCache[CachePrefix + name];
            return bundleCache.GetContent(CachePrefix + name);
        }

        public string RenderCached(string name)
        {
            GroupBundles = groupBundlesCache[CachePrefix + name];
            return CacheRenderer.Get(CachePrefix, name);
        }

        public string RenderCachedAssetTag(string name)
        {
            GroupBundles = groupBundlesCache[CachePrefix + name];
            return Render(null, name, new CacheRenderer(CachePrefix, name));
        }

        public void AsNamed(string name, string renderTo)
        {
            Render(renderTo, name, new FileRenderer(fileWriterFactory));
            groupBundlesCache[CachePrefix + name] = GroupBundles;
        }

        public string AsCached(string name, string filePath)
        {
            string result = Render(filePath, name, new CacheRenderer(CachePrefix, name));
            groupBundlesCache[CachePrefix + name] = GroupBundles;
            return result;
        }

        protected string RenderDebug(string name = null)
        {
            string content = null;
            if (!bundleCache.TryGetValue(name, out content))
            {
                DependentFiles.Clear();

                var modifiedGroupBundles = BeforeRenderDebug();
                var sb = new StringBuilder();
                foreach (var groupBundleKVP in modifiedGroupBundles)
                {
                    var groupBundle = groupBundleKVP.Value;
                    var attributes = GetAdditionalAttributes(groupBundle);
                    var assets = groupBundle.Assets;

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
                            sb.AppendLine(FillTemplate(groupBundle, processedFile));
                        }
                        else if (asset.RemotePath != null)
                        {
                            sb.AppendLine(FillTemplate(groupBundle, ExpandAppRelativePath(asset.LocalPath)));
                        }
                        else
                        {
                            foreach (var file in files)
                            {
                                var relativePath = FileSystem.ResolveFileSystemPathToAppRelative(file);
                            	string path;
                                if (HttpContext.Current == null)
                                {
                                    path = (asset.LocalPath.StartsWith("~") ? "" : "/") + relativePath;
                                }
                                else
                                {
                                    if (HttpRuntime.AppDomainAppVirtualPath.EndsWith("/"))
                                    {
                                        path = HttpRuntime.AppDomainAppVirtualPath + relativePath;    
                                    }
                                    else
                                    {
                                        path = HttpRuntime.AppDomainAppVirtualPath + "/" + relativePath;
                                    }
                                }
                                sb.AppendLine(FillTemplate(groupBundle, path));
                            }
                        }
                    }
                }

                foreach(var cntnt in arbitrary)
                {
                    sb.AppendLine(string.Format(tagFormat, cntnt));
                }

                content = sb.ToString();
                bundleCache.Add(name, content, DependentFiles);
            }

            return content;
        }

        private string RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            string content;
            if (!bundleCache.TryGetValue(key, out content))
            {
                var files = new List<string>();
                foreach (var groupBundleKVP in GroupBundles)
                {
                    var group = groupBundleKVP.Key;
                    var groupBundle = groupBundleKVP.Value;

                    string minifiedContent = null;
                    string hash = null;
                    bool hashInFileName = false;

                    DependentFiles.Clear();

                    if (renderTo == null)
                    {
                        renderTo = renderPathCache[CachePrefix + "." + group + "." + key];
                    }
                    else
                    {
                        renderPathCache[CachePrefix + "." + group + "." + key] = renderTo;
                    }

                    string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(renderTo);
                    var renderToPath = ExpandAppRelativePath(renderTo);

                    if (!String.IsNullOrEmpty(BaseOutputHref))
                    {
                        renderToPath = String.Concat(BaseOutputHref.TrimEnd('/'), "/", renderToPath.TrimStart('/'));
                    }

                    var remoteAssetPaths = new List<string>();
                    foreach (var asset in groupBundle.Assets)
                    {
                        if (asset.IsRemote)
                        {
                            remoteAssetPaths.Add(asset.RemotePath);
                        }
                    }

                    files.AddRange(GetFiles(groupBundle.Assets.Where(asset => 
                        asset.IsEmbeddedResource || 
                        asset.IsLocal ||
                        asset.IsRemoteDownload).ToList()));

                    DependentFiles.AddRange(files);

                    if (renderTo.Contains("#"))
                    {
                        hashInFileName = true;
                        minifiedContent = Minifier.Minify(BeforeMinify(outputFile, files, arbitrary));
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
                        minifiedContent = minifiedContent ?? Minifier.Minify(BeforeMinify(outputFile, files, arbitrary));
                        renderer.Render(minifiedContent, outputFile);
                    }

                    if (hash == null && !string.IsNullOrEmpty(HashKeyName))
                    {
                        hash = hasher.GetHash(minifiedContent);
                    }

                    string renderedTag;
                    if (hashInFileName)
                    {
                        renderedTag = FillTemplate(groupBundle, renderToPath);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HashKeyName))
                        {
                            renderedTag = FillTemplate(groupBundle, renderToPath);
                        }
                        else if (renderToPath.Contains("?"))
                        {
                            renderedTag = FillTemplate(groupBundle, renderToPath + "&" + HashKeyName + "=" + hash);
                        }
                        else
                        {
                            renderedTag = FillTemplate(groupBundle, renderToPath + "?" + HashKeyName + "=" + hash);
                        }
                    }

                    content += String.Concat(GetFilesForRemote(remoteAssetPaths, groupBundle), renderedTag);
                }

                bundleCache.Add(key, content, DependentFiles);
            }

            return content;
        }

        /// <summary>
        /// If Debug, returns a <see cref="IList{string}"/> of file names.
        /// If Release, squishes bundle, writes squished file, returns resulting filename as only member of a <see cref="IList{string}"/>.
        ///
        /// Useful when loading files on-demand (via ajax).  
        /// Eg. 
        ///    Bundle().Javascript().Add("~/js/test1.js").Add("~/js/test2.js").SquishAndGetNames("~/js/test_#.js")
        ///      If Debug, returns "/js/test1.js,/js/test2.js"
        ///      If Release, returns "/js/test_43958ADEC93DAC8037D2471A95382DE9.js"
        /// </summary>
        /// <param name="renderTo"></param>
        /// <returns>List of names</returns>
        public IList<string> SquishAndGetNames(string renderTo)
        {
            var result = Render(renderTo);
            var scripts = new List<string>();

            var startToken = FileNameAttribute() + "=\"";
            var endToken = "\"";
            var start = result.IndexOf(startToken);
            while (start != -1)
            {
                var nameStartIndex = start + startToken.Length;
                var length = result.IndexOf(endToken, nameStartIndex) - nameStartIndex;
                scripts.Add(result.Substring(nameStartIndex, length));
                start = result.IndexOf(startToken, nameStartIndex + length);
            }

            return scripts;
        }
        private string FileNameAttribute()
        {
            // source attribute will either be "src" or "href"
            return this.Template.Contains("src=\"") ? "src" : "href";
        }

        public void ClearCache()
        {
            bundleCache.ClearTestingCache();
        }

        private void AddAttributes(Dictionary<string, string> attributes, string group = DEFAULT_GROUP, bool merge = true)
        {
            GroupBundle groupBundle;
            if (GroupBundles.TryGetValue(group, out groupBundle))
            {
                if (merge)
                {
                    foreach (var attribute in attributes)
                    {
                        groupBundle.Attributes[attribute.Key] = attribute.Value;
                    }
                }
                else
                {
                    groupBundle.Attributes = attributes;
                }
            }
            else
            {
                GroupBundles[group] = new GroupBundle(attributes);
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

        public T WithGroupAttribute(string name, string value, string group)
        {
            AddAttributes(new Dictionary<string, string> { { name, value } }, group);
            return (T)this;
        }

        public T WithGroupAttributes(Dictionary<string, string> attributes, string group, bool merge = true)
        {
            AddAttributes(attributes, group, merge);
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

        private string FillTemplate(GroupBundle groupBundle, string path)
        {
            return string.Format(Template, GetAdditionalAttributes(groupBundle), path);
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

        internal virtual Dictionary<string, GroupBundle> BeforeRenderDebug()
        {
            return GroupBundles;
        }

        protected abstract string Template { get; }
        protected abstract string CachePrefix { get; }
    }
}