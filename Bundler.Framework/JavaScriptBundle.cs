using System;
using System.Collections.Generic;
using System.Text;
using Bundler.Framework.Files;
using Bundler.Framework.Minifiers;
using Bundler.Framework.Utilities;

namespace Bundler.Framework
{
    internal class JavaScriptBundle: BundleBase, IJavaScriptBundle, IJavaScriptBundleBuilder
    {
        private static Dictionary<string, string> renderedJavaScriptFiles = new Dictionary<string, string>();
        private static Dictionary<string, string> debugJavaScriptFiles = new Dictionary<string, string>();
        private List<string> javaScriptFiles = new List<string>();
        private const string scriptTemplate = "<script type=\"text/javascript\" src=\"{0}\"></script>";

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

        IJavaScriptBundleBuilder IJavaScriptBundleBuilder.Add(string javaScriptPath)
        {
            javaScriptFiles.Add(javaScriptPath);
            return this;
        }

        public void AsNamed(string name, string renderTo)
        {
            Render(renderTo, name);
        }

        string IJavaScriptBundle.RenderNamed(string name)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return debugJavaScriptFiles[name];
            }
            return renderedJavaScriptFiles[name];
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
                debugJavaScriptFiles.Add(key, output);
                return output;
            }

            if (!renderedJavaScriptFiles.ContainsKey(key))
            {
                lock (renderedJavaScriptFiles)
                {
                    if (!renderedJavaScriptFiles.ContainsKey(key))
                    {
                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);
                        string minifiedJavaScript = ProcessJavaScriptInput(GetFilePaths(javaScriptFiles), outputFile, null, JsMinMinifier.Identifier);
                        string hash = Hasher.Create(minifiedJavaScript);
                        string renderedScriptTag = String.Format(scriptTemplate, ExpandAppRelativePath(renderTo) + "?r=" + hash);
                        renderedJavaScriptFiles.Add(key, renderedScriptTag);
                    }
                }
            }
            return renderedJavaScriptFiles[key];
        }

        public string ProcessJavaScriptInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string minifierType)
        {
            List<string> files = GetFiles(arguments);
            string minifiedJavaScript = MinifyJavaScript(files, minifierType);
            WriteFiles(minifiedJavaScript, outputFile);
            WriteGZippedFile(minifiedJavaScript, null);
            return minifiedJavaScript;
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
    }
}