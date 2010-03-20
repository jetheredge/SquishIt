using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bundler.Framework.CssCompressors;
using Bundler.Framework.Files;
using Bundler.Framework.Utilities;
using dotless.Core;

namespace Bundler.Framework
{
    internal class CssBundle : BundleBase, ICssBundle, ICssBundleBuilder
    {
        private readonly IDebugStatusReader debugStatusReader;
        private static Dictionary<string, string> renderedCssFiles = new Dictionary<string, string>();
        private List<string> cssFiles = new List<string>();
        private string mediaTag = "";

        public CssBundle()
        {
            debugStatusReader = new DebugStatusReader();
        }

        public CssBundle(IDebugStatusReader debugStatusReader)
        {
            this.debugStatusReader = debugStatusReader;
        }

        ICssBundleBuilder ICssBundleBuilder.Add(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        ICssBundleBuilder ICssBundle.Add(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.WithMedia(string media)
        {
            mediaTag = "media=\"" + media + "\"";
            return this;
        }

        void ICssBundleBuilder.AsNamed(string name, string renderTo)
        {
            Render(renderTo, name);
        }

        string ICssBundle.RenderNamed(string name)
        {
            return renderedCssFiles[name];
        }

        string ICssBundleBuilder.Render(string renderTo)
        {
            return Render(renderTo, renderTo);
        }

        private string Render(string renderTo, string key)
        {
            string cssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" {0} href=\"{1}\" />";
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssTemplate = String.Format(cssTemplate, mediaTag, "{0}");

                var processedCssFiles = new List<string>();
                foreach (string file in cssFiles)
                {                    
                    if (Path.GetExtension(file).ToLower() == ".less")
                    {
                        string outputFile = ResolveAppRelativePathToFileSystem(file);
                        string css = ProcessLess(outputFile);
                        outputFile = outputFile.Substring(0, outputFile.Length - 5);
                        using (var sr = new StreamWriter(outputFile, false))
                        {
                            sr.Write(css);
                        }
                        processedCssFiles.Add(file.Substring(0, file.Length - 5));
                    }
                    else
                    {
                        processedCssFiles.Add(file);
                    }
                }

                return RenderFiles(cssTemplate, processedCssFiles);
            }

            if (!renderedCssFiles.ContainsKey(key))
            {
                lock (renderedCssFiles)
                {
                    if (!renderedCssFiles.ContainsKey(key))
                    {
                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);
                        string compressedCss = ProcessCssInput(GetFilePaths(cssFiles), outputFile, null, YuiCompressor.Identifier);
                        string hash = Hasher.Create(compressedCss);
                        string renderedCssTag = String.Format(cssTemplate, mediaTag, ExpandAppRelativePath(renderTo) + "?r=" + hash);
                        renderedCssFiles.Add(key, renderedCssTag);
                    }
                }
            }
            return renderedCssFiles[key];
        }

        public static string ProcessCssInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string compressorType)
        {
            List<string> files = GetFiles(arguments);
            string compressedCss = CompressCss(files, compressorType);
            WriteFiles(compressedCss, outputFile);
            WriteGZippedFile(compressedCss, null);
            return compressedCss;
        }

        public static string CompressCss(List<string> files, string compressorType)
        {
            ICssCompressor compressor = CssCompressorRegistry.Get(compressorType);
            return CompressCss(files, compressor).ToString();
        }

        private static StringBuilder CompressCss(List<string> files, ICssCompressor compressor)
        {
            var outputCss = new StringBuilder();
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".less")
                {
                    string css = ProcessLess(file);
                    outputCss.Append(compressor.CompressContent(css));
                }
                else
                {
                    outputCss.Append(compressor.CompressFile(file));
                }
            }
            return outputCss;
        }

        private static string ProcessLess(string file)
        {
            var engine = new ExtensibleEngine();                    
            var md = new MinifierDecorator(engine);                    
            return md.TransformToCss(file);                    
        }
    }
}