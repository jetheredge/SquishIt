using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using dotless.Core;
using SquishIt.Framework.Css.Compressors;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    internal class CssBundle : BundleBase, ICssBundle, ICssBundleBuilder
    {
        private static BundleCache bundleCache = new BundleCache();
        private static Dictionary<string, string> debugCssFiles = new Dictionary<string, string>();
        private static Dictionary<string, NamedState> namedState = new Dictionary<string, NamedState>();
        private List<string> cssFiles = new List<string>();
        private List<string> remoteCssFiles = new List<string>();
        private List<string> embeddedResourceCssFiles = new List<string>();
        private List<string> dependentFiles = new List<string>();
        private ICssCompressor cssCompressorInstance = new MsCompressor();
        private bool renderOnlyIfOutputFileMissing = false;
        private bool processImports = false;
        private const string CssTemplate = "<link rel=\"stylesheet\" type=\"text/css\" {0}href=\"{1}\" />";
        private static readonly Regex importPattern = new Regex(@"@import +url\(([""']){0,1}(.*?)\1{0,1}\);", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string cachePrefix = "css";

        public CssBundle()
            : base(new FileWriterFactory(), new FileReaderFactory(), new DebugStatusReader(), new CurrentDirectoryWrapper())
        {
        }

        public CssBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper)
            : base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper)
        {
        }

        ICssBundleBuilder ICssBundleBuilder.Add(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        ICssBundleBuilder ICssBundle.AddRemote(string localPath, string remotePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssFiles.Add(localPath);
            }
            else
            {
                remoteCssFiles.Add(remotePath);
            }
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.AddRemote(string javaScriptPath, string cdnUri)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssFiles.Add(javaScriptPath);
            }
            else
            {
                remoteCssFiles.Add(cdnUri);
            }
            return this;
        }

        void AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                cssFiles.Add(localPath);
            }
            else
            {
                embeddedResourceCssFiles.Add(embeddedResourcePath);
            }
        }

        ICssBundleBuilder ICssBundle.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddEmbeddedResource(localPath, embeddedResourcePath);
            return this;
        }

        ICssBundleBuilder ICssBundle.Add(string cssScriptPath)
        {
            cssFiles.Add(cssScriptPath);
            return this;
        }

        ICssBundleBuilder ICssBundleBuilder.WithMedia(string media)
        {
            return WithAttribute("media", media);
        }

        public ICssBundleBuilder WithAttribute(string name, string value)
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

        public ICssBundleBuilder WithCompressor(CssCompressors cssCompressor)
        {
            
            this.cssCompressorInstance = MapCompressorEnumToType(cssCompressor);
            return this;
        }

        public ICssBundleBuilder WithCompressor(ICssCompressor cssCompressor)
        {
            this.cssCompressorInstance = cssCompressor;
            return this;
        }

        void ICssBundleBuilder.AsNamed(string name, string renderTo)
        {
            namedState[name] = new NamedState(debugStatusReader.IsDebuggingEnabled(), renderTo);
            Render(renderTo, name);
        }

        public ICssBundleBuilder RenderOnlyIfOutputFileMissing()
        {
            renderOnlyIfOutputFileMissing = true;
            return this;
        }

        public ICssBundleBuilder ProcessImports()
        {
            processImports = true;
            return this;
        }

        public ICssBundleBuilder ForceDebug()
        {
            debugStatusReader.ForceDebug();
            return this;
        }

        public ICssBundleBuilder ForceRelease()
        {
            debugStatusReader.ForceRelease();
            return this;
        }

        string ICssBundle.RenderNamed(string name)
        {
            NamedState state = namedState[name];
            if (state.Debug)
            {
                return debugCssFiles[name];
            }
            return RenderRelease(name, state.RenderTo, new FileRenderer(fileWriterFactory));
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
            return RenderRelease(key, renderTo, new FileRenderer(fileWriterFactory));
        }

        public string AsCached(string name, string cssPath)
        {
            if (debugStatusReader.IsDebuggingEnabled())
            {
                string result = RenderDebugCss();
                debugCssFiles[name] = result;
                return result;
            }
            return RenderRelease(name, cssPath, new CacheRenderer(cachePrefix, name));
        }

        private string RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            if (!bundleCache.ContainsKey(key))
            {
                lock (bundleCache)
                {
                    if (!bundleCache.ContainsKey(key))
                    {
                        string compressedCss;
                        string hash= null;
                        bool hashInFileName = false;

                        dependentFiles.Clear();

                        string outputFile = ResolveAppRelativePathToFileSystem(renderTo);

                        List<string> files = GetFiles(GetFilePaths(cssFiles));
                        files.AddRange(GetFiles(GetEmbeddedResourcePaths(embeddedResourceCssFiles)));
                        dependentFiles.AddRange(files);

                        if (renderTo.Contains("#"))
                        {
                            hashInFileName = true;
                            compressedCss = CompressCss(outputFile, files, cssCompressorInstance);
                            hash = Hasher.Create(compressedCss);
                            renderTo = renderTo.Replace("#", hash);
                            outputFile = outputFile.Replace("#", hash);
                        }

                        if (renderOnlyIfOutputFileMissing && FileExists(outputFile))
                        {
                            compressedCss = ReadFile(outputFile);
                        }
                        else
                        {
                            compressedCss = CompressCss(outputFile, files, cssCompressorInstance);
                            renderer.Render(compressedCss, outputFile);
                        }
                        
                        if (hash == null)
                        {
                            hash = Hasher.Create(compressedCss);
                        }

                        string renderedCssTag;
                        if (hashInFileName)
                        {
                            renderedCssTag = FillTemplate(ExpandAppRelativePath(renderTo));
                        }
                        else
                        {
                            string path = ExpandAppRelativePath(renderTo);
                            if (path.Contains("?"))
                            {
                                renderedCssTag = FillTemplate(path + "&r=" + hash);
                            }
                            else
                            {
                                renderedCssTag = FillTemplate(path + "?r=" + hash);
                            }
                        }
                        renderedCssTag = String.Concat(GetFilesForRemote(), renderedCssTag);
                        bundleCache.AddToCache(key, renderedCssTag, dependentFiles);
                    }
                }
            }
            return bundleCache.GetContent(key);
        }

        private ICssCompressor MapCompressorEnumToType(CssCompressors compressors)
        {
            string compressor;
            switch (compressors)
            {
                case CssCompressors.NullCompressor:
                    compressor = NullCompressor.Identifier;
                    break;
                case CssCompressors.YuiCompressor:
                    compressor = YuiCompressor.Identifier;
                    break;
                case CssCompressors.MsCompressor:
                    compressor = MsCompressor.Identifier;
                    break;
                default:
                    compressor = MsCompressor.Identifier;
                    break;
            }

            return CssCompressorRegistry.Get(compressor);
        }

        private string RenderDebugCss()
        {
            string modifiedCssTemplate = FillTemplate("{0}");
            var processedCssFiles = new List<string>();
            foreach (string file in cssFiles)
            {
                if (file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
                {
                    string outputFile = ResolveAppRelativePathToFileSystem(file);
                    string css = ProcessLess(outputFile);
                    outputFile += ".debug.css";
                    using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                    {
                        fileWriter.Write(css);
                    }
                    processedCssFiles.Add(file + ".debug.css");
                }
                else
                {
                    processedCssFiles.Add(file);
                }
            }

            return RenderFiles(modifiedCssTemplate, processedCssFiles);
        }

        public void ClearCache()
        {
            bundleCache.ClearTestingCache();
            debugCssFiles.Clear();
            namedState.Clear();
        }

        public string RenderCached(string name)
        {
            var cacheRenderer = new CacheRenderer(cachePrefix, name);
            return cacheRenderer.Get(name);
        }

        private string CompressCss(string outputFilePath, List<string> files, ICssCompressor compressor)
        {
            var outputCss = new StringBuilder();
            foreach (string file in files)
            {
                string css;
                if (file.ToLower().EndsWith(".less") || file.ToLower().EndsWith(".less.css"))
                {
                    css = ProcessLess(file);
                }
                else
                {
                    css = ReadFile(file);
                }
                
                if (processImports)
                {
                    css = ProcessImport(css);
                }
                css = CssPathRewriter.RewriteCssPaths(outputFilePath, file, css);
                outputCss.Append(compressor.CompressContent(css));
            }
            return outputCss.ToString();
        }

        private string ProcessLess(string file)
        {
            try
            {
                currentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                var content = ReadFile(file);
                var engineFactory = new EngineFactory();
                var engine = engineFactory.GetEngine();
                return engine.TransformToCss(content, file);
            }
            finally
            {
                currentDirectoryWrapper.Revert();
            }
        }

        private string ProcessImport(string css)
        {
            return importPattern.Replace(css, new MatchEvaluator(ApplyFileContentsToMatchedImport));
        }

        private string ApplyFileContentsToMatchedImport(Match match)
        {
            var file = ResolveAppRelativePathToFileSystem(match.Groups[2].Value);
            dependentFiles.Add(file);
            return ReadFile(file);
        }

        private string GetFilesForRemote()
        {
            var renderedCssFilesForCdn = new StringBuilder();
            foreach (var uri in remoteCssFiles)
            {
                renderedCssFilesForCdn.Append(FillTemplate(uri));
            }
            return renderedCssFilesForCdn.ToString();
        }

        private string FillTemplate(string path)
        {
            return String.Format(CssTemplate, GetAdditionalAttributes(), path);
        }
    }
}