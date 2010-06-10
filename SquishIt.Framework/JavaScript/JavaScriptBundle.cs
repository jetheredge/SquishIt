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
        private static Dictionary<string, string> renderedJavaScriptFiles = new Dictionary<string, string>();
        private static Dictionary<string, string> debugJavaScriptFiles = new Dictionary<string, string>();
        private List<string> javaScriptFiles = new List<string>();
        //Added to support CND
        private List<string> javaScriptFilesForCdn = new List<string>();
        //
        private JavaScriptMinifiers javaScriptMinifier = JavaScriptMinifiers.Yui;
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

        IJavaScriptBundleBuilder IJavaScriptBundle.AddCdn(string javaScriptPath, string cdnUri)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                javaScriptFiles.Add(javaScriptPath);
            }
            else
            {
                javaScriptFilesForCdn.Add(cdnUri);
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

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.AddCdn(string javaScriptPath, string cdnUri)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                javaScriptFiles.Add(javaScriptPath);
            }
            else
            {
                javaScriptFilesForCdn.Add(cdnUri);
            }
            return this;
        }

        public void AsNamed(string name, string renderTo)
        {
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
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return debugJavaScriptFiles[name];
            }
            return renderedJavaScriptFiles[name];
        }

        public void ClearCache()
        {
            debugJavaScriptFiles.Clear();
            renderedJavaScriptFiles.Clear();
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

            if (!renderedJavaScriptFiles.ContainsKey(key))
            {
                lock (renderedJavaScriptFiles)
                {
                    if (!renderedJavaScriptFiles.ContainsKey(key))
                    {
                        string compressedJavaScript;
                        string hash = null;
                        bool hashInFileName = false;
                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            compressedJavaScript = MinifyJavaScript(GetFilePaths(javaScriptFiles), MapMinifierToIdentifier(javaScriptMinifier));
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
                            string identifier = MapMinifierToIdentifier(javaScriptMinifier);
                            minifiedJavaScript = MinifyJavaScript(GetFilePaths(javaScriptFiles), identifier);
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
                        renderedJavaScriptFiles.Add(key, renderedScriptTag);
                       
                    }
                }
            }
            renderedJavaScriptFiles[key] = String.Concat(GetJavascriptFilesForCdn(), renderedJavaScriptFiles[key]);
            return renderedJavaScriptFiles[key];
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
                default:
                    return JsMinMinifier.Identifier;
            }
        }

        public string MinifyJavaScript(List<InputFile> arguments, string minifierType)
        {
            List<string> files = GetFiles(arguments);
            return MinifyJavaScript(files, minifierType);
        }

        public void WriteJavaScriptToFile(string minifiedJavaScript, string outputFile, string gzippedOutputFile)
        {
            WriteFiles(minifiedJavaScript, outputFile);
            WriteGZippedFile(minifiedJavaScript, null);
        }

        public string MinifyJavaScript(List<string> files, string minifierType)
        {
            IJavaScriptCompressor minifier = MinifierRegistry.Get(minifierType);
            return MinifyJavaScript(files, minifier).ToString();
        }

        private StringBuilder MinifyJavaScript(List<string> files, IJavaScriptCompressor minifier)
        {
            var outputJavaScript = new StringBuilder();
            foreach (string file in files)
            {
                string content = ReadFile(file);
                outputJavaScript.Append(minifier.CompressContent(content));
            }
            return outputJavaScript;
        }

        private string GetJavascriptFilesForCdn()
        {
            var renderedJavaScriptFilesForCdn = new StringBuilder();
            if (javaScriptFilesForCdn.Count > 0)
            {
                foreach (var uri in javaScriptFilesForCdn)
                {
                    renderedJavaScriptFilesForCdn.AppendFormat(String.Format(scriptTemplate + "{1}", uri, "\n"));
                }
            }
            return renderedJavaScriptFilesForCdn.ToString();
        }
    }
}