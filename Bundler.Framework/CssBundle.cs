using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Bundler.Framework.CssCompressors;
using Bundler.Framework.Files;
using Bundler.Framework.Utilities;
using dotless.Core;

namespace Bundler.Framework
{
    internal class CssBundle : BundleBase, ICssBundler
    {
        private static Dictionary<string, string> renderedCssFiles = new Dictionary<string, string>();
        private List<string> cssFiles = new List<string>();

        public ICssBundler AddCss(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        public string RenderCss(string renderTo)
        {
            string cssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";
            if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled)
            {
                return RenderFiles(cssTemplate, cssFiles);
            }

            if (!renderedCssFiles.ContainsKey(renderTo))
            {
                lock (renderedCssFiles)
                {
                    if (!renderedCssFiles.ContainsKey(renderTo))
                    {
                        string outputFile = ResolveFile(renderTo);
                        string compressedCss = ProcessCssInput(GetFilePaths(cssFiles), outputFile, null, YuiCompressor.Identifier);
                        string hash = Hasher.Create(compressedCss);
                        string renderedCssTag = String.Format(cssTemplate, renderTo + "?r=" + hash);
                        renderedCssFiles.Add(renderTo, renderedCssTag);
                    }
                }
            }
            return renderedCssFiles[renderTo];
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
                    var engine = new ExtensibleEngine();                    
                    var md = new MinifierDecorator(engine);                    
                    string css = md.TransformToCss(file);
                    outputCss.Append(compressor.CompressContent(css));
                }
                else
                {
                    outputCss.Append(compressor.CompressFile(file));
                }
            }
            return outputCss;
        }
    }
}