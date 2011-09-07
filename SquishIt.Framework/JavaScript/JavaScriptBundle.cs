using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript {
    public class JavaScriptBundle : BundleBase<JavaScriptBundle> 
    {
        private const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\"></script>";
        bool typeless;

        private const string CACHE_PREFIX = "js";

        protected override IMinifier<JavaScriptBundle> DefaultMinifier 
        {
            get { return new MsMinifier (); }
        }

        public JavaScriptBundle ()
            : base (new FileWriterFactory (new RetryableFileOpener (), 5), new FileReaderFactory (new RetryableFileOpener (), 5), new DebugStatusReader (), new CurrentDirectoryWrapper (), new Hasher (new RetryableFileOpener ()), new BundleCache ()) {}

        public JavaScriptBundle (IDebugStatusReader debugStatusReader)
            : base (new FileWriterFactory (new RetryableFileOpener (), 5), new FileReaderFactory (new RetryableFileOpener (), 5), debugStatusReader, new CurrentDirectoryWrapper (), new Hasher (new RetryableFileOpener ()), new BundleCache ()) {}

        public JavaScriptBundle (IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache) :
            base (fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher, bundleCache) {}

        public JavaScriptBundle WithoutTypeAttribute () 
        {
            this.typeless = true;
            return this;
        }

        protected override string Template 
        {
            get { return typeless ? JS_TEMPLATE.Replace ("type=\"text/javascript\" ", "") : JS_TEMPLATE; }
        }

        protected override string CachePrefix 
        {
            get { return CACHE_PREFIX; }
        }
    }
}