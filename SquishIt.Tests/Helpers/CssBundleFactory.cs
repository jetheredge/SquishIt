using SquishIt.Framework;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests.Helpers
{
    internal class JavaScriptBundleFactory
    {
        private IDebugStatusReader debugStatusReader = new StubDebugStatusReader();
        private IFileWriterFactory fileWriterFactory = new StubFileWriterFactory();
        private IFileReaderFactory fileReaderFactory = new StubFileReaderFactory();
        private ICurrentDirectoryWrapper currentDirectoryWrapper = new StubCurrentDirectoryWrapper();
        private IHasher hasher = new StubHasher("hash");
    	private IBundleCache bundleCache = new StubBundleCache();

        public StubFileReaderFactory FileReaderFactory { get { return fileReaderFactory as StubFileReaderFactory; } }
        public StubFileWriterFactory FileWriterFactory { get { return fileWriterFactory as StubFileWriterFactory; } }

        public JavaScriptBundleFactory WithDebuggingEnabled(bool enabled)
        {
            this.debugStatusReader = new StubDebugStatusReader(enabled);
            return this;
        }

        public JavaScriptBundleFactory WithFileWriterFactory(IFileWriterFactory fileWriterFactory)
        {
            this.fileWriterFactory = fileWriterFactory;
            return this;
        }

        public JavaScriptBundleFactory WithFileReaderFactory(IFileReaderFactory fileReaderFactory)
        {
            this.fileReaderFactory = fileReaderFactory;
            return this;
        }

        private JavaScriptBundleFactory WithCurrentDirectoryWrapper(ICurrentDirectoryWrapper currentDirectoryWrapper)
        {
            this.currentDirectoryWrapper = currentDirectoryWrapper;
            return this;
        }

        public JavaScriptBundleFactory WithHasher(IHasher hasher)
        {
            this.hasher = hasher;
            return this;
        }

        public JavaScriptBundle Create()
        {
            return new JavaScriptBundle(debugStatusReader, fileWriterFactory, fileReaderFactory, currentDirectoryWrapper, hasher, bundleCache);
        }

        public JavaScriptBundleFactory WithContents(string css)
        {
            (fileReaderFactory as StubFileReaderFactory).SetContents(css);
            return this;
        }
    }
}