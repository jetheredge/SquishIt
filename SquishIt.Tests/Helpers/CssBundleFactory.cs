using SquishIt.Framework;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Files;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;
using System;

namespace SquishIt.Tests.Helpers
{
    internal class BundleFactoryDetails
    {
        public IDebugStatusReader DebugStatusReader { get; set; }
        public IFileWriterFactory FileWriterFactory { get; set; }
        public IFileReaderFactory FileReaderFactory { get; set; }
        public ICurrentDirectoryWrapper CurrentDirectoryWrapper { get; set; }
        public IHasher Hasher { get; set; }
        public IBundleCache BundleCache { get; set; }
    }

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
            return this.Create<JavaScriptBundle>((details) => new JavaScriptBundle(details.DebugStatusReader, details.FileWriterFactory, details.FileReaderFactory, details.CurrentDirectoryWrapper, details.Hasher, details.BundleCache));
        }       

        public JavaScriptBundle Create<TBundle>(Func<BundleFactoryDetails, TBundle> bundleCreator)
            where TBundle : JavaScriptBundle
        {
            return bundleCreator(this.GetDetails());
        }

        public JavaScriptBundle Create<TBundle>(Func<TBundle> bundleCreator)
            where TBundle : JavaScriptBundle
        {
            return bundleCreator();
        }

        public JavaScriptBundleFactory WithContents(string css)
        {
            (fileReaderFactory as StubFileReaderFactory).SetContents(css);
            return this;
        }

        private BundleFactoryDetails GetDetails()
        {
            return new BundleFactoryDetails
            {
                BundleCache = this.bundleCache,
                CurrentDirectoryWrapper = this.currentDirectoryWrapper,
                DebugStatusReader = this.debugStatusReader,
                FileReaderFactory = this.fileReaderFactory,
                FileWriterFactory = this.fileWriterFactory,
                Hasher = this.hasher
            };
        }
    }
}