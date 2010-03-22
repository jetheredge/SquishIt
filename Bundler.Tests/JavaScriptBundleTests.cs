using Bundler.Framework;
using Bundler.Framework.Tests.Mocks;
using NUnit.Framework;

namespace Bundler.Tests
{
    [TestFixture]
    public class JavaScriptBundleTests
    {
        private string javaScript = @"
                                        function product(a, b)
                                        {
                                            return a * b;
                                        }

                                        function sum(a, b){
                                            return a + b;
                                        }";

        [Test]
        public void CanBundleJavaScript()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(javaScript);

            IJavaScriptBundle javaScriptBundle = new JavaScriptBundle(mockDebugStatusReader,
                                                                        mockFileWriterFactory,
                                                                        mockFileReaderFactory);

            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .Render("~/js/output.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", mockFileWriterFactory.Files["~/js/output.js"]);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(javaScript);

            IJavaScriptBundle javaScriptBundle = new JavaScriptBundle(mockDebugStatusReader,
                                                                        mockFileWriterFactory,
                                                                        mockFileReaderFactory);

            javaScriptBundle
                .Add("~/js/test.js")
                .AsNamed("Test", "~/js/output.js");

            var tag = javaScriptBundle.RenderNamed("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", mockFileWriterFactory.Files["~/js/output.js"]);
        }

        [Test]
        public void CanRenderDebugTags()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(javaScript);

            IJavaScriptBundle javaScriptBundle = new JavaScriptBundle(mockDebugStatusReader,
                                                                        mockFileWriterFactory,
                                                                        mockFileReaderFactory);

            javaScriptBundle
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .AsNamed("TestWithDebug", "~/js/output.js");

            var tag = javaScriptBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(javaScript);

            IJavaScriptBundle javaScriptBundle = new JavaScriptBundle(mockDebugStatusReader,
                                                                        mockFileWriterFactory,
                                                                        mockFileReaderFactory);

            javaScriptBundle
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .AsNamed("NamedWithDebug", "~/js/output.js");

            var tag = javaScriptBundle.RenderNamed("NamedWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);            
        }
    }
}