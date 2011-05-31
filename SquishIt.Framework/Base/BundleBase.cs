using System;
using System.Collections.Generic;
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
		private static Dictionary<string, string> renderPathCache = new Dictionary<string, string>();
		
		private const string DEFAULT_GROUP = "default";
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected ICurrentDirectoryWrapper currentDirectoryWrapper;
        protected IHasher hasher;
		protected abstract IMinifier<T> DefaultMinifier { get; }
    	private IMinifier<T> minifier;
    	protected IMinifier<T> Minifier
    	{
    		get
    		{
    			if (minifier == null)
    				minifier = DefaultMinifier;
				return minifier;
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
 
            foreach (InputFile file in inputFiles)
            {
                resolvedFilePaths.AddRange(file.Resolver.TryResolve(file.FilePath));
            }

            return resolvedFilePaths;
        }

		/// FIX-----------------------------------------------------
		private InputFile GetInputFile(Asset asset)
		{
			if (!asset.IsEmbeddedResource)
			{
				return String.IsNullOrEmpty(asset.RemotePath) ? GetFileSystemPath(asset.LocalPath) : GetHttpPath(asset.RemotePath);
			}
			else
			{
				return GetEmbeddedResourcePath(asset.RemotePath);
			}
		}
		/// FIX-----------------------------------------------------
		
        private List<InputFile> GetInputFiles(List<Asset> assets)
        {
            var inputFiles = new List<InputFile>();
            foreach (var asset in assets)
            {
                if (asset.RemotePath == null)
                {
                    inputFiles.Add(GetFileSystemPath(asset.LocalPath));
                }
                else if (asset.IsEmbeddedResource)
                {
                    inputFiles.Add(GetEmbeddedResourcePath(asset.RemotePath));
                }
            }
            return inputFiles;
        }

        private InputFile GetFileSystemPath(string localPath)
        {
            string mappedPath = FileSystem.ResolveAppRelativePathToFileSystem(localPath);
            return new InputFile(mappedPath, ResolverFactory.Get<FileResolver>());
        }

		/// FIX-----------------------------------------------------
		private InputFile GetHttpPath(string remotePath)
		{
 			return new InputFile(remotePath, ResolverFactory.Get<HttpResolver>());
		}
		/// FIX-----------------------------------------------------
        
        private InputFile GetEmbeddedResourcePath(string resourcePath)
        {
            return new InputFile(resourcePath, ResolverFactory.Get<EmbeddedResourceResolver>());
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

        public T Add(string filePath)
        {
            AddAsset(new Asset(filePath));
            return (T)this;
        }

        public T Add(string filePath, string group)
        {
            AddAsset(new Asset(filePath), group);
            return (T)this;
        }

        public T AddRemote(string localPath, string remotePath)
        {
            AddAsset(new Asset(localPath, remotePath));
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

        public string Render(string renderTo)
        {
            string key = renderTo + GroupBundles.GetHashCode();
            return Render(renderTo, key);
        }

        private string Render(string renderTo, string key, bool isNamed = false)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(isNamed ? key : null);
            }
            return RenderRelease(key, renderTo, new FileRenderer(fileWriterFactory));
        }

        public string RenderNamed(string name)
        {
            return bundleCache.GetContent(name);
        }

        public string RenderCached(string name)
        {
            return CacheRenderer.Get(CachePrefix, name);
        }

		public string RenderCachedAssetTag(string name)
		{
			if (debugStatusReader.IsDebuggingEnabled())
			{
				return RenderDebug(name);
			}
			return RenderRelease(name, null, new CacheRenderer(CachePrefix, name));
		}

        public void AsNamed(string name, string renderTo)
        {
            Render(renderTo, name, true);
        }

        public string AsCached(string name, string filePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(name);
            }
            return RenderRelease(name, filePath, new CacheRenderer(CachePrefix, name));
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
						string processedFile = ExpandAppRelativePath(asset.LocalPath);
						/// FIX-----------------------------------------------------
						if (!String.IsNullOrEmpty(asset.RemotePath))
						{
							IEnumerable<String> files = null;
							var inputFile = GetInputFile(asset);
							files = inputFile.Resolver.TryResolve(asset.IsEmbeddedResource ? inputFile.FilePath : asset.RemotePath);
							var debugContent = String.Empty;
							var tsb = new StringBuilder();
							foreach(var fn in files)
							{
								tsb.Append(ReadFile(fn) + "\n\n\n");
							}
							var renderer = new FileRenderer(fileWriterFactory);
							renderer.Render(tsb.ToString(), FileSystem.ResolveAppRelativePathToFileSystem((processedFile)));
						}
						/// FIX-----------------------------------------------------
                        sb.Append(FillTemplate(groupBundle, processedFile));
                    }
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

                    var localAssetPaths = new List<string>();
                    var remoteAssetPaths = new List<string>();
                    var embeddedAssetPaths = new List<string>();
                    foreach (var asset in groupBundle.Assets)
                    {
                        if (asset.RemotePath == null)
                        {
                            localAssetPaths.Add(asset.LocalPath);
                        }
                        else if (!asset.IsEmbeddedResource)
                        {
                            remoteAssetPaths.Add(asset.RemotePath);
                        }
                        else if (asset.IsEmbeddedResource)
                        {
                            embeddedAssetPaths.Add(asset.RemotePath);
                        }
                    }

                    files.AddRange(GetFiles(groupBundle.Assets));
                    DependentFiles.AddRange(files);

                    if (renderTo.Contains("#"))
                    {
                        hashInFileName = true;
                        minifiedContent = Minifier.Minify(BeforeMinify(outputFile, files));
                        hash = hasher.GetHash(minifiedContent);
                        renderTo = renderTo.Replace("#", hash);
                    	renderToPath = renderToPath.Replace("#", hash);
                        outputFile = outputFile.Replace("#", hash);
                    }

                    if (ShouldRenderOnlyIfOutputFileIsMissing && FileExists(outputFile) && minifiedContent == null)
                    {
                        minifiedContent = ReadFile(outputFile);
                    }
                    else
                    {
                        minifiedContent = minifiedContent ?? Minifier.Minify(BeforeMinify(outputFile, files));
                        renderer.Render(minifiedContent, outputFile);
                    }

                    if (hash == null)
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
                        if (renderToPath.Contains("?"))
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

        protected virtual string BeforeMinify(string outputFile, List<string> files)
        {
			var sb = new StringBuilder();
			foreach (var file in files)
			{
                sb.Append(ReadFile(file) + "\n");
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