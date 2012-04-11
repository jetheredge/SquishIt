using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    public class JavaScriptBundle : BundleBase<JavaScriptBundle>
    {
        private const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\"></script>";
        private const string TAG_FORMAT = "<script type=\"text/javascript\">{0}</script>";

        private const string CACHE_PREFIX = "js";

        protected override IMinifier<JavaScriptBundle> DefaultMinifier
        {
            get { return Configuration.DefaultJsMinifier(); }
        }

        protected override IEnumerable<string> allowedExtensions
        {
            get { return instanceAllowedExtensions.Union(Bundle.AllowedGlobalExtensions.Union(Bundle.AllowedScriptExtensions)); }
        }

        protected override IEnumerable<string> disallowedExtensions
        {
            get { return Bundle.AllowedStyleExtensions; }
        }

        protected override string defaultExtension
        {
            get { return ".JS"; }
        }

        protected override string tagFormat
        {
            get { return typeless ? TAG_FORMAT.Replace(" type=\"text/javascript\"", "") : TAG_FORMAT; }
        }

        public JavaScriptBundle()
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), new DebugStatusReader(), new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()), new BundleCache()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader)
            : base(new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), debugStatusReader, new CurrentDirectoryWrapper(), new Hasher(new RetryableFileOpener()), new BundleCache()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache) :
            base(fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher, bundleCache) { }

        protected override string Template
        {
            get { return typeless ? JS_TEMPLATE.Replace("type=\"text/javascript\" ", "") : JS_TEMPLATE; }
        }

        protected override string CachePrefix
        {
            get { return CACHE_PREFIX; }
        }

        internal override void BeforeRenderDebug()
        {
            foreach(var asset in bundleState.Assets)
            {
                var localPath = asset.LocalPath;
                var preprocessors = FindPreprocessors(localPath);
                if(preprocessors != null && preprocessors.Count() > 0)
                {
                    string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(localPath);
                    string javascript = PreprocessJavascriptFile(outputFile, preprocessors);
                    outputFile += ".debug.js";
                    using(var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                    {
                        fileWriter.Write(javascript);
                    }

                    asset.LocalPath = localPath + ".debug.js";
                }
            }
        }

        protected override string BeforeMinify(string outputFile, List<string> files, IEnumerable<ArbitraryContent> arbitraryContent)
        {
            //TODO: refactor so that this is common code and ProcessCSSFile/ProcessJavascriptFile are a single abstract method called here
            var sb = new StringBuilder();

            files.Select(ProcessJavascriptFile)
                .Concat(arbitraryContent.Select(ac => {
                    var filename = "dummy." + ac.Extension;
                    var preprocessors = FindPreprocessors(filename);
                    return PreprocessContent(filename, preprocessors, ac.Content);
                }))
                .Aggregate(sb, (builder, val) => builder.Append(val + "\n"));

            return sb.ToString();
        }

        private string ProcessJavascriptFile(string file)
        {
            var preprocessors = FindPreprocessors(file);
            if(preprocessors != null)
            {
                return PreprocessJavascriptFile(file, preprocessors);
            }
            return ReadFile(file);
        }

        private string PreprocessJavascriptFile(string file, IEnumerable<IPreprocessor> preprocessors)
        {
            lock(typeof(JavaScriptBundle))
            {
                return PreprocessFile(file, preprocessors);
            }
        }
    }
}