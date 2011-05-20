using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    internal class JavaScriptBundle: BundleBase, IJavaScriptBundle, IJavaScriptBundleBuilder
    {
        private static Regex httpMatcher = new Regex(@"^https?://");
        private static BundleCache<RenderedScriptBundle> bundleCache = new BundleCache<RenderedScriptBundle>();
        private static Dictionary<string, string> debugJavaScriptFiles = new Dictionary<string, string>();
        private static Dictionary<string, NamedState> namedState = new Dictionary<string, NamedState>();
        private List<string> javaScriptFiles = new List<string>();
        private List<string> remoteJavaScriptFiles = new List<string>();
        private List<string> embeddedResourceJavaScriptFiles = new List<string>();
        
        private IJavaScriptMinifier javaScriptMinifier = new MsMinifier();
        private const string scriptTemplate = "<script type=\"text/javascript\" {0}src=\"{1}\"></script>";
        private bool renderOnlyIfOutputFileMissing = false;
        private string cachePrefix = "js";

        public JavaScriptBundle()
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), new DebugStatusReader(), new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()))
        {
        }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader)
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), debugStatusReader, new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()))
        {
        }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher): 
            base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher)
        {
        }

        IJavaScriptBundleBuilder IJavaScriptBundle.Add(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundle.AddRemote(string localPath, string remotePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                javaScriptFiles.Add(localPath);
            }
            else
            {
                remoteJavaScriptFiles.Add(remotePath);
            }
            return this;
        }

        void AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                javaScriptFiles.Add(localPath);
            }
            else
            {
                embeddedResourceJavaScriptFiles.Add(embeddedResourcePath);
            }
        }

        IJavaScriptBundleBuilder IJavaScriptBundle.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        public IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier)
        {
            this.javaScriptMinifier = MapMinifierEnumToType(javaScriptMinifier);
            return this;
        }

        public IJavaScriptBundleBuilder WithMinifier(IJavaScriptMinifier javaScriptMinifier)
        {
            this.javaScriptMinifier = javaScriptMinifier;
            return this;
        }

        public IJavaScriptBundleBuilder WithAttribute(string name, string value)
        {
            if (attributes.ContainsKey(name))
            {
                attributes[name] = value;
            }
            else
            {
                attributes.Add(name, value);
            }
            return this;
        }

        public IJavaScriptBundleBuilder RenderOnlyIfOutputFileMissing()
        {
            renderOnlyIfOutputFileMissing = true;
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.Add(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.AddRemote(string javaScriptPath, string remoteUri)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                javaScriptFiles.Add(javaScriptPath);
            }
            else
            {
                remoteJavaScriptFiles.Add(remoteUri);
            }
            return this;
        }

        public void AsNamed(string name, string renderTo)
        {
            namedState[name] = new NamedState(debugStatusReader.IsDebuggingEnabled(), renderTo);
            Render(name, renderTo, new FileRenderer(fileWriterFactory));
        }

        public IJavaScriptBundleBuilder ForceDebug()
        {
            debugStatusReader.ForceDebug();
            return this;
        }

        public IJavaScriptBundleBuilder ForceRelease()
        {
            debugStatusReader.ForceRelease();
            return this;
        }

        string IJavaScriptBundle.RenderNamed(string name)
        {
            NamedState state = namedState[name];
            if (state.Debug)
            {
                return debugJavaScriptFiles[name];
            }

            return Render(name, state.RenderTo, new FileRenderer(fileWriterFactory));
        }

        public void ClearTestingCache()
        {
            debugJavaScriptFiles.Clear();
            bundleCache.ClearTestingCache();
            namedState.Clear();
        }

        public string RenderCached(string name)
        {
            var cacheRenderer = new CacheRenderer(cachePrefix, name);
            return cacheRenderer.Get(name);
        }

        string IJavaScriptBundleBuilder.Render(string renderTo)
        {
            string key = renderTo + string.Join(",", javaScriptFiles.ToArray()).GetHashCode().ToString();
            return Render(key, renderTo, new FileRenderer(fileWriterFactory));
        }

        private string Render(string key, string renderTo, IRenderer renderer)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(key);
            }

            return RenderScriptTags(RenderRelease(key, renderTo, renderer));
        }

        private string RenderScriptTags(RenderedScriptBundle renderedScriptBundle)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var file in renderedScriptBundle.Files)
            {
                if (httpMatcher.IsMatch(file) || file.Contains(renderedScriptBundle.Hash))
                {
                    builder.Append(FillTemplate(file));
                }
                else
                {
                    if (file.Contains("?"))
                    {
                        builder.Append(FillTemplate(file + "&r=" + renderedScriptBundle.Hash));
                    }
                    else
                    {
                        builder.Append(FillTemplate(file + "?r=" + renderedScriptBundle.Hash));
                    }
                }
            }
            return builder.ToString();
        }

        public string AsCached(string name, string jsPath)
        {
            return Render(name, jsPath, new CacheRenderer(cachePrefix, name));
        }

        public IEnumerable<string> AsEnumerable(string renderTo)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return javaScriptFiles.Select(ExpandAppRelativePath);
            }
            string key = renderTo + string.Join(",", javaScriptFiles.ToArray()).GetHashCode().ToString();
            return RenderRelease(key, renderTo, new FileRenderer(fileWriterFactory)).Files;
        }

        private string RenderDebug(string key)
        {
            string modifiedTemplate = FillTemplate("{0}");
            string output = RenderFiles(modifiedTemplate, javaScriptFiles);
            debugJavaScriptFiles[key] = output;
            return output;
        }

        private string MinifyFiles(string renderTo, List<string> files, out string hash, out string minifiedJavaScript)
        {
            minifiedJavaScript = MinifyJavaScript(files, javaScriptMinifier);
            hash = hasher.GetHash(minifiedJavaScript);
            return renderTo.Replace("#", hash);
        }

        private RenderedScriptBundle RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            if (!bundleCache.ContainsKey(key))
            {
                lock (bundleCache)
                {
                    if (!bundleCache.ContainsKey(key))
                    {
                        List<string> files = GetFiles(GetFilePaths(javaScriptFiles));
                        files.AddRange(GetFiles(GetEmbeddedResourcePaths(embeddedResourceJavaScriptFiles)));

                        string minifiedJavaScript;
                        string hash;
                        var outputFile = MinifyFiles(renderTo, files, out hash, out minifiedJavaScript);

                        if (!renderOnlyIfOutputFileMissing || !FileExists(outputFile))
                        {
                            renderer.Render(minifiedJavaScript, ResolveAppRelativePathToFileSystem(outputFile));
                        }

                        var allFiles = new List<string>();
                        allFiles.AddRange(remoteJavaScriptFiles);
                        allFiles.Add(ExpandAppRelativePath(outputFile));
                        bundleCache.AddToCache(key, new RenderedScriptBundle { Files = allFiles.ToArray(), Hash = hash }, files);
                    }
                }
            }
            return bundleCache.GetContent(key);
        }

        private IJavaScriptMinifier MapMinifierEnumToType(JavaScriptMinifiers javaScriptMinifier)
        {
            string minifier;
            switch (javaScriptMinifier)
            {
                case JavaScriptMinifiers.NullMinifier:
                    minifier = NullMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.JsMin:
                    minifier = JsMinMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.Closure:
                    minifier = ClosureMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.Yui:
                    minifier = YuiMinifier.Identifier;
                    break;
                case JavaScriptMinifiers.Ms:
                    minifier = MsMinifier.Identifier;
                    break;
                default:
                    minifier = MsMinifier.Identifier;
                    break;
            }
            return MinifierRegistry.Get(minifier);
        }

        private string MinifyJavaScript(List<string> files, IJavaScriptMinifier minifier)
        {
            try
            {
                var inputJavaScript = new StringBuilder();
                foreach (var file in files)
                {
                    inputJavaScript.Append(ReadFile(file) + "\n");
                }
                return minifier.CompressContent(inputJavaScript.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error processing: {0}", e.Message), e);
            }
        }

        private string FillTemplate(string path)
        {
            return String.Format(scriptTemplate, GetAdditionalAttributes(), path);
        }
    }
}