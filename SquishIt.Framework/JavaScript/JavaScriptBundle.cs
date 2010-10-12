using System;
using System.Collections.Generic;
using System.Text;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript.Minifiers;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    internal class JavaScriptBundle: BundleBase, IJavaScriptBundle, IJavaScriptBundleBuilder
    {
        private static BundleCache bundleCache = new BundleCache();
        private static Dictionary<string, string> debugJavaScriptFiles = new Dictionary<string, string>();
        private static Dictionary<string, NamedState> namedState = new Dictionary<string, NamedState>();
        private List<string> javaScriptFiles = new List<string>();
        private List<string> remoteJavaScriptFiles = new List<string>();
        private List<string> embeddedResourceJavaScriptFiles = new List<string>();
        
        private IJavaScriptMinifier javaScriptMinifier = new MsMinifier();
        private const string scriptTemplate = "<script type=\"text/javascript\" {0}src=\"{1}\"></script>";
        private bool renderOnlyIfOutputFileMissing = false;
        private string cachePrefix = "js";

        public JavaScriptBundle(): base(new FileWriterFactory(), new FileReaderFactory(), new DebugStatusReader(), new CurrentDirectoryWrapper())
        {
        }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper): 
            base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper)
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
            Render(renderTo, name);
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

            string outputFile = ResolveAppRelativePathToFileSystem(state.RenderTo);
            return RenderRelease(name, state.RenderTo, new FileRenderer(fileWriterFactory));
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
            return Render(renderTo, renderTo);
        }

        private string Render(string renderTo, string key)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(key);
            }
            return RenderRelease(key, renderTo, new FileRenderer(fileWriterFactory));
        }

        public string AsCached(string name, string jsPath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return RenderDebug(name);
            }
            return RenderRelease(name, jsPath, new CacheRenderer(cachePrefix, name));
        }

        private string RenderDebug(string key)
        {
            string modifiedTemplate = FillTemplate("{0}");
            string output = RenderFiles(modifiedTemplate, javaScriptFiles);
            debugJavaScriptFiles[key] = output;
            return output;
        }

        private string RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            if (!bundleCache.ContainsKey(key))
            {
                lock (bundleCache)
                {
                    if (!bundleCache.ContainsKey(key))
                    {
                        string compressedJavaScript;
                        string hash = null;
                        bool hashInFileName = false;
                        
                        List<string> files = GetFiles(GetFilePaths(javaScriptFiles));
                        files.AddRange(GetFiles(GetEmbeddedResourcePaths(embeddedResourceJavaScriptFiles)));
                        
                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            compressedJavaScript = MinifyJavaScript(files, javaScriptMinifier);
                            hash = Hasher.Create(compressedJavaScript);
                            renderTo = renderTo.Replace("#", hash);
                        }

                        var outputFile = ResolveAppRelativePathToFileSystem(renderTo);

                        string minifiedJavaScript;
                        if (renderOnlyIfOutputFileMissing && FileExists(outputFile))
                        {
                            minifiedJavaScript = ReadFile(outputFile);
                        }
                        else
                        {
                            minifiedJavaScript = MinifyJavaScript(files, javaScriptMinifier);
                            renderer.Render(minifiedJavaScript, outputFile);
                        }
                        
                        if (hash == null)
                        {
                            hash = Hasher.Create(minifiedJavaScript);                            
                        }
                        
                        string renderedScriptTag;
                        if (hashInFileName)
                        {
                            renderedScriptTag = FillTemplate(ExpandAppRelativePath(renderTo));
                        }
                        else
                        {
                            string path = ExpandAppRelativePath(renderTo);
                            if (path.Contains("?"))
                            {
                                renderedScriptTag = FillTemplate(ExpandAppRelativePath(renderTo) + "&r=" + hash);    
                            }
                            else
                            {
                                renderedScriptTag = FillTemplate(ExpandAppRelativePath(renderTo) + "?r=" + hash);        
                            }
                        }
                        renderedScriptTag = String.Concat(GetFilesForRemote(), renderedScriptTag);
                        bundleCache.AddToCache(key, renderedScriptTag, files);
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
                    inputJavaScript.Append(ReadFile(file));
                }
                return minifier.CompressContent(inputJavaScript.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error processing: {0}", e.Message), e);
            }
        }

        private string GetFilesForRemote()
        {
            var renderedJavaScriptFilesForCdn = new StringBuilder();
            foreach (var uri in remoteJavaScriptFiles)
            {
                renderedJavaScriptFilesForCdn.Append(FillTemplate(uri));
            }
            return renderedJavaScriptFilesForCdn.ToString();
        }

        private string FillTemplate(string path)
        {
            return String.Format(scriptTemplate, GetAdditionalAttributes(), path);
        }
    }
}