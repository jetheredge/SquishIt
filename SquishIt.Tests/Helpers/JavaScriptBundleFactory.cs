using SquishIt.Framework.Caches;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests.Helpers
{
    internal class JavaScriptBundleFactory
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

        JavaScriptBundleFactory WithCurrentDirectoryWrapper(IDirectoryWrapper directoryWrapper)
        {
            this.directoryWrapper = directoryWrapper;
            return this;
        }

        public JavaScriptBundleFactory WithHasher(IHasher hasher)
        {
            this.hasher = hasher;
            return this;
        }

        public JavaScriptBundle Create()
        {
            return new JavaScriptBundle(debugStatusReader, fileWriterFactory, fileReaderFactory, directoryWrapper, hasher, contentCache, rawContentCache);
        }

        public JavaScriptBundleFactory WithContents(string css)
        {
            (fileReaderFactory as StubFileReaderFactory).SetContents(css);
            return this;
        }
    }
}