using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript
{
    public class JavaScriptBundle : BundleBase<JavaScriptBundle>
    {
        const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\" defer></script>";
        const string TAG_FORMAT = "<script type=\"text/javascript\">{0}</script>";

        const string CACHE_PREFIX = "js";

        bool deferred;

        protected override IMinifier<JavaScriptBundle> DefaultMinifier
        {
            get { return Configuration.Instance.DefaultJsMinifier(); }
        }

        protected override IEnumerable<string> allowedExtensions
        {
            get { return bundleState.AllowedExtensions.Union(Bundle.AllowedGlobalExtensions.Union(Bundle.AllowedScriptExtensions)); }
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
            get { return bundleState.Typeless ? TAG_FORMAT.Replace(" type=\"text/javascript\"", "") : TAG_FORMAT; }
        }

        public JavaScriptBundle()
            : this(new DebugStatusReader()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader)
            : this(debugStatusReader, new FileWriterFactory(new RetryableFileOpener(), 5), new FileReaderFactory(new RetryableFileOpener(), 5), new DirectoryWrapper(), Configuration.Instance.DefaultHasher(), new BundleCache()) { }

        public JavaScriptBundle(IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDirectoryWrapper directoryWrapper, IHasher hasher, IBundleCache bundleCache) :
            base(fileWriterFactory, fileReaderFactory, debugStatusReader, directoryWrapper, hasher, bundleCache) { }

        protected override string Template
        {
            get
            {
                var val = bundleState.Typeless ? JS_TEMPLATE.Replace("type=\"text/javascript\" ", "") : JS_TEMPLATE;
                return deferred ? val : val.Replace(" defer", "");
            }
        }

        protected override string CachePrefix
        {
            get { return CACHE_PREFIX; }
        }

        protected override string ProcessFile(string file, string outputFile, Asset originalAsset)
        {
            var preprocessors = FindPreprocessors(file);
            return MinifyIfNeeded(preprocessors.NullSafeAny() 
                ? PreprocessFile(file, preprocessors) 
                : ReadFile(file), originalAsset.Minify);
        }

        protected override void AggregateContent(List<Asset> assets, StringBuilder sb, string outputFile)
        {
            assets.SelectMany(a => a.IsArbitrary
                                       ? new[] { PreprocessArbitrary(a) }.AsEnumerable()
                                       : GetFilesForSingleAsset(a).Select(f => ProcessFile(f, outputFile, a)))
                .ToList()
                .Distinct()
                .Aggregate(sb, (b, s) =>
                {
                    b.Append(s);
                    return b;
                });
        }

        const string MINIFIED_FILE_SEPARATOR = ";";
        
        protected override string AppendFileClosure(string content)
        {
            if (!(content.Trim().EndsWith(MINIFIED_FILE_SEPARATOR)))
            {
                content += MINIFIED_FILE_SEPARATOR;
            }
            return content;
        }

        public JavaScriptBundle WithDeferredLoad()
        {
            deferred = true;
            return this;
        }
    }
}