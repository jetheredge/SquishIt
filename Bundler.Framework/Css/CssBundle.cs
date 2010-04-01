using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bundler.Framework.Css.Compressors;
using Bundler.Framework.Files;
using Bundler.Framework.Utilities;
using dotless.Core;

namespace Bundler.Framework.Css
{
    internal class CssBundle : BundleBase, ICssBundle, ICssBundleBuilder
    {
        private static Dictionary<string, string> renderedCssFiles = new Dictionary<string, string>();
        private static Dictionary<string, string> debugCssFiles = new Dictionary<string, string>();
        private List<string> cssFiles = new List<string>();
        private string mediaTag = "";
        private CssCompressors cssCompressor = CssCompressors.YuiCompressor;
        private const string CssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" {0} href=\"{1}\" />";

        public CssBundle()
            : base(new FileWriterFactory(), new FileReaderFactory(), new DebugStatusReader())
        {
        }

        public CssBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory)
            : base(fileWriterFactory, fileReaderFactory, debugStatusReader)
        {
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

        public ICssBundleBuilder WithCompressor(CssCompressors cssCompressor)
        {
            this.cssCompressor = cssCompressor;
            return this;
        }

        void ICssBundleBuilder.AsNamed(string name, string renderTo)
        {
            Render(renderTo, name);
        }

        string ICssBundle.RenderNamed(string name)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                return debugCssFiles[name];
            }
            
            return renderedCssFiles[name];
        }

        string ICssBundleBuilder.Render(string renderTo)
        {
            return Render(renderTo, renderTo);
        }

        private string Render(string renderTo, string key)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                string result = RenderDebugCss();
                debugCssFiles[key] = result;
                return result;
            }

            if (!renderedCssFiles.ContainsKey(key))
            {
                lock (renderedCssFiles)
                {
                    if (!renderedCssFiles.ContainsKey(key))
                    {
                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);

                        string compressedCss = ProcessCssInput(GetFilePaths(cssFiles), outputFile, null, MapCompressorToIdentifier(cssCompressor));
                        string hash = Hasher.Create(compressedCss);
                        string renderedCssTag = String.Format(CssTemplate, mediaTag, ExpandAppRelativePath(renderTo) + "?r=" + hash);
                        renderedCssFiles.Add(key, renderedCssTag);
                    }
                }
            }
            return renderedCssFiles[key];
        }

        private string MapCompressorToIdentifier(CssCompressors compressors)
        {
            switch (compressors)
            {
                case CssCompressors.NullCompressor:
                    return NullCompressor.Identifier;
                case CssCompressors.YuiCompressor:
                    return YuiCompressor.Identifier;
                default:
                    return YuiCompressor.Identifier;
            }
        }

        private string RenderDebugCss()
        {
            string modifiedCssTemplate = String.Format(CssTemplate, mediaTag, "{0}");

            var processedCssFiles = new List<string>();
            foreach (string file in cssFiles)
            {                    
                if (Path.GetExtension(file).ToLower() == ".less")
                {
                    string outputFile = ResolveAppRelativePathToFileSystem(file);
                    string css = ProcessLess(outputFile);
                    outputFile = outputFile.Substring(0, outputFile.Length - 5);
                    using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                    {
                        fileWriter.Write(css);
                    }
                    processedCssFiles.Add(file.Substring(0, file.Length - 5));
                }
                else
                {
                    processedCssFiles.Add(file);
                }
            }

            return RenderFiles(modifiedCssTemplate, processedCssFiles);
        }

        public string ProcessCssInput(List<InputFile> arguments, string outputFile, string gzippedOutputFile, string compressorType)
        {
            List<string> files = GetFiles(arguments);
            string compressedCss = CompressCss(files, compressorType);
            WriteFiles(compressedCss, outputFile);
            WriteGZippedFile(compressedCss, null);
            return compressedCss;
        }

        public string CompressCss(List<string> files, string compressorType)
        {
            ICssCompressor compressor = CssCompressorRegistry.Get(compressorType);
            return CompressCss(files, compressor).ToString();
        }

        public void ClearCache()
        {
            renderedCssFiles.Clear();
            debugCssFiles.Clear();
        }

        private StringBuilder CompressCss(List<string> files, ICssCompressor compressor)
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
                    string css = ReadFile(file);
                    outputCss.Append(compressor.CompressContent(css));
                }
            }
            return outputCss;
        }        

        private string ProcessLess(string file)
        {
            var content = ReadFile(file);
            var engine = new ExtensibleEngine();
            var md = new MinifierDecorator(engine);
            var lso = new LessSourceObject();
            lso.Cacheable = false;
            lso.Content = content;
            lso.Key = file;
            return md.TransformToCss(lso);
        }
    }
}