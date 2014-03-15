using SquishIt.Framework.Caches;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests.Helpers
{
    internal class CSSBundleFactory
    {
        IDebugStatusReader debugStatusReader = new StubDebugStatusReader();
        IFileWriterFactory fileWriterFactory = new StubFileWriterFactory();
        IFileReaderFactory fileReaderFactory = new StubFileReaderFactory();
        IDirectoryWrapper directoryWrapper = new StubDirectoryWrapper();
        IHasher hasher = new StubHasher("hash");
    	IContentCache contentCache = new StubContentCache();
        IContentCache rawContentCache = new StubContentCache();

        public StubFileReaderFactory FileReaderFactory { get { return fileReaderFactory as StubFileReaderFactory; } }
        public StubFileWriterFactory FileWriterFactory { get { return fileWriterFactory as StubFileWriterFactory; } }

        public CSSBundleFactory WithDebuggingEnabled(bool enabled)
        {
            this.debugStatusReader = new StubDebugStatusReader(enabled);
            return this;
        }

        public CSSBundleFactory WithFileWriterFactory(IFileWriterFactory fileWriterFactory)
        {
            this.fileWriterFactory = fileWriterFactory;
            return this;
        }

        public CSSBundleFactory WithFileReaderFactory(IFileReaderFactory fileReaderFactory)
        {
            this.fileReaderFactory = fileReaderFactory;
            return this;
        }

        public CSSBundleFactory WithCurrentDirectoryWrapper(IDirectoryWrapper directoryWrapper)
        {
            this.directoryWrapper = directoryWrapper;
            return this;
        }

        public CSSBundleFactory WithHasher(IHasher hasher)
        {
            this.hasher = hasher;
            return this;
        }

        public CSSBundle Create()
        {
            return new CSSBundle(debugStatusReader, fileWriterFactory, fileReaderFactory, directoryWrapper, hasher, contentCache, rawContentCache);
        }

        public CSSBundleFactory WithContents(string css)
        {
            (fileReaderFactory as StubFileReaderFactory).SetContents(css);
            return this;
        }
    }
}