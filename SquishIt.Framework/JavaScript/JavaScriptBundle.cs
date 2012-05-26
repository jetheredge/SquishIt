using System.Collections.Generic;
using System.IO;
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
        private const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\" defer></script>";
        private const string TAG_FORMAT = "<script type=\"text/javascript\">{0}</script>";

        private const string CACHE_PREFIX = "js";

        private bool deferred;

        protected override IMinifier<JavaScriptBundle> DefaultMinifier
        {
            get { return Configuration.Instance.DefaultJsMinifier(); }
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
            get
            {
                var val = typeless ? JS_TEMPLATE.Replace("type=\"text/javascript\" ", "") : JS_TEMPLATE;
                return deferred ? val : val.Replace(" defer", "");
            }
        }

        protected override string CachePrefix {
            get { return CACHE_PREFIX; }
        }

        protected  override string ProcessFile(string file, string outputFile) 
        {
            var preprocessors = FindPreprocessors(file);
            if (preprocessors != null) 
            {
                return PreprocessFile(file, preprocessors);
            }
            return ReadFile(file);
        }

        public JavaScriptBundle WithDeferredLoad()
        {
            deferred = true;
            return this;
        }
    }
}