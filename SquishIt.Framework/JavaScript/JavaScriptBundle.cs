using System;
using System.Collections.Generic;
using System.Text;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript.Minifiers;
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
        //
        private JavaScriptMinifiers javaScriptMinifier = JavaScriptMinifiers.Ms;
        private const string scriptTemplate = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        private bool renderOnlyIfOutputFileMissing = false;

        public JavaScriptBundle(): base(new FileWriterFactory(), new FileReaderFactory(), new DebugStatusReader())
        {
        }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory): base(fileWriterFactory, fileReaderFactory, debugStatusReader)
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

        public IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier)
        {
            this.javaScriptMinifier = javaScriptMinifier;
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
            return RenderRelease(name, state.RenderTo);
        }

        public void ClearTestingCache()
        {
            debugJavaScriptFiles.Clear();
            bundleCache.ClearTestingCache();
            namedState.Clear();
        }

        string IJavaScriptBundleBuilder.Render(string renderTo)
        {
            return Render(renderTo, renderTo);
        }

        private string Render(string renderTo, string key)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {              
                string output = RenderFiles(scriptTemplate, javaScriptFiles);
                debugJavaScriptFiles[key] = output;
                return output;
            }

            return RenderRelease(key, renderTo);
        }

        private string RenderRelease(string key, string renderTo)
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
                        string identifier = MapMinifierToIdentifier(javaScriptMinifier);
                        
                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            compressedJavaScript = MinifyJavaScript(files, identifier);
                            hash = Hasher.Create(compressedJavaScript);
                            renderTo = renderTo.Replace("#", hash);
                        }

                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);

                        string minifiedJavaScript;
                        if (renderOnlyIfOutputFileMissing && FileExists(outputFile))
                        {
                            minifiedJavaScript = ReadFile(outputFile);
                        }
                        else
                        {
                            minifiedJavaScript = MinifyJavaScript(files, identifier);
                            WriteJavaScriptToFile(minifiedJavaScript, outputFile, null);
                        }
                        
                        if (hash == null)
                        {
                            hash = Hasher.Create(minifiedJavaScript);                            
                        }
                        
                        string renderedScriptTag;
                        if (hashInFileName)
                        {
                            renderedScriptTag = String.Format(scriptTemplate, ExpandAppRelativePath(renderTo));
                        }
                        else
                        {
                            string path = ExpandAppRelativePath(renderTo);
                            if (path.Contains("?"))
                            {
                                renderedScriptTag = String.Format(scriptTemplate, ExpandAppRelativePath(renderTo) + "&r=" + hash);    
                            }
                            else
                            {
                                renderedScriptTag = String.Format(scriptTemplate, ExpandAppRelativePath(renderTo) + "?r=" + hash);        
                            }
                        }
                        renderedScriptTag = String.Concat(GetFilesForRemote(), renderedScriptTag);
                        bundleCache.AddToCache(key, renderedScriptTag, files);
                    }
                }
            }
            return bundleCache.GetContent(key);
        }

        private string MapMinifierToIdentifier(JavaScriptMinifiers javaScriptMinifier)
        {
            switch (javaScriptMinifier)
            {
                case JavaScriptMinifiers.NullMinifier:
                    return NullMinifier.Identifier;
                case JavaScriptMinifiers.JsMin:
                    return JsMinMinifier.Identifier;
                case JavaScriptMinifiers.Closure:
                    return ClosureMinifier.Identifier;
                case JavaScriptMinifiers.Yui:
                    return YuiMinifier.Identifier;
                case JavaScriptMinifiers.Ms:
                    return MsMinifier.Identifier;
                default:
                    return MsMinifier.Identifier;
            }
        }

        protected void WriteJavaScriptToFile(string minifiedJavaScript, string outputFile, string gzippedOutputFile)
        {
            WriteFiles(minifiedJavaScript, outputFile);
            WriteGZippedFile(minifiedJavaScript, null);
        }

        protected string MinifyJavaScript(List<string> files, string minifierType)
        {
            IJavaScriptCompressor minifier = MinifierRegistry.Get(minifierType);
            return MinifyJavaScript(files, minifier);
        }

        private string MinifyJavaScript(List<string> files, IJavaScriptCompressor minifier)
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
                renderedJavaScriptFilesForCdn.Append(String.Format(scriptTemplate, uri));
            }
            return renderedJavaScriptFilesForCdn.ToString();
        }
    }
}