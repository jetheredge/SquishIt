using SquishIt.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Files;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests.Helpers
{
    internal class CssBundleFactory
    {
        private IDebugStatusReader debugStatusReader = new StubDebugStatusReader();
        private IFileWriterFactory fileWriterFactory = new StubFileWriterFactory();
        private IFileReaderFactory fileReaderFactory = new StubFileReaderFactory();
        private ICurrentDirectoryWrapper currentDirectoryWrapper = new StubCurrentDirectoryWrapper();
        private IHasher hasher = new StubHasher("hash");
    	private IBundleCache bundleCache = new StubBundleCache();

        public StubFileReaderFactory FileReaderFactory { get { return fileReaderFactory as StubFileReaderFactory; } }
        public StubFileWriterFactory FileWriterFactory { get { return fileWriterFactory as StubFileWriterFactory; } }

        public CssBundleFactory WithDebuggingEnabled(bool enabled)
        {
            this.debugStatusReader = new StubDebugStatusReader(enabled);
            return this;
        }

        private CssBundleFactory WithFileWriterFactory(IFileWriterFactory fileWriterFactory)
        {
            this.fileWriterFactory = fileWriterFactory;
            return this;
        }

        private CssBundleFactory WithFileReaderFactory(IFileReaderFactory fileReaderFactory)
        {
            this.fileReaderFactory = fileReaderFactory;
            return this;
        }

        private CssBundleFactory WithCurrentDirectoryWrapper(ICurrentDirectoryWrapper currentDirectoryWrapper)
        {
            this.currentDirectoryWrapper = currentDirectoryWrapper;
            return this;
        }

        public CssBundleFactory WithHasher(IHasher hasher)
        {
            this.hasher = hasher;
            return this;
        }

        public CSSBundle Create()
        {
            return new CSSBundle(debugStatusReader, fileWriterFactory, fileReaderFactory, currentDirectoryWrapper, hasher, bundleCache);
        }

        public CssBundleFactory WithContents(string css)
        {
            (fileReaderFactory as StubFileReaderFactory).SetContents(css);
            return this;
        }
    }
}