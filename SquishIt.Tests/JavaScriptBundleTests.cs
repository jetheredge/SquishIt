using NUnit.Framework;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.JavaScript.Minifiers;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
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

        private string javaScript2 = @"function sum(a, b){
                                            return a + b;
                                       }";

        private IJavaScriptBundle javaScriptBundle;
        private IJavaScriptBundle javaScriptBundle2;
        private IJavaScriptBundle debugJavaScriptBundle;
        private IJavaScriptBundle debugJavaScriptBundle2;
        private StubFileWriterFactory fileWriterFactory;
        private StubFileReaderFactory fileReaderFactory;

        [SetUp]
        public void Setup()
        {
            var nonDebugStatusReader = new StubDebugStatusReader(false);
            var debugStatusReader = new StubDebugStatusReader(true);
            fileWriterFactory = new StubFileWriterFactory();
            fileReaderFactory = new StubFileReaderFactory();
            fileReaderFactory.SetContents(javaScript);

            javaScriptBundle = new JavaScriptBundle(nonDebugStatusReader,
                                                    fileWriterFactory,
                                                    fileReaderFactory);

            javaScriptBundle2 = new JavaScriptBundle(nonDebugStatusReader,
                                                    fileWriterFactory,
                                                    fileReaderFactory);

            debugJavaScriptBundle = new JavaScriptBundle(debugStatusReader, 
                                                        fileWriterFactory, 
                                                        fileReaderFactory);

            debugJavaScriptBundle2 = new JavaScriptBundle(debugStatusReader,
                                                        fileWriterFactory,
                                                        fileReaderFactory);
        }

        [Test]
        public void CanBundleJavaScript()
        {
            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=8E8C548F4F6300695269DE689B903BA3\"></script>", tag);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_1.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithQuerystringParameter()
        {
            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .Render("~/js/output_querystring.js?v=2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_querystring.js?v=2&r=8E8C548F4F6300695269DE689B903BA3\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            javaScriptBundle
                .Add("~/js/test.js")
                .AsNamed("Test", "~/js/output_2.js");

            var tag = javaScriptBundle.RenderNamed("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_2.js?r=8E8C548F4F6300695269DE689B903BA3\"></script>", tag);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_2.js"]);
        }
        //----------------------------------------------------
        [Test]
        public void CanBundleJavaScriptWithCdn()
        {
            var tag = javaScriptBundle
                .AddCdn("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                .Add("~/js/test.js")
                .Render("~/js/output_1_2.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script>\n<script type=\"text/javascript\" src=\"js/output_1_2.js?r=8E8C548F4F6300695269DE689B903BA3\"></script>", tag);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_1_2.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithCdnAndQuerystringParameter()
        {
            var tag = javaScriptBundle
                .AddCdn("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                .Add("~/js/test.js")
                .Render("~/js/output_querystring.js?v=2_2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script>\n<script type=\"text/javascript\" src=\"js/output_querystring.js?v=2_2&r=8E8C548F4F6300695269DE689B903BA3\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithCdn()
        {
            javaScriptBundle
                .AddCdn("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                .Add("~/js/test.js")
                .AsNamed("TestCdn", "~/js/output_3_2.js");

            var tag = javaScriptBundle.RenderNamed("TestCdn");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script>\n<script type=\"text/javascript\" src=\"js/output_3_2.js?r=8E8C548F4F6300695269DE689B903BA3\"></script>", tag);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_3_2.js"]);
        }
        //-------------------------------------------------------------------------
        [Test]
        public void CanRenderDebugTags()
        {
            debugJavaScriptBundle
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .AsNamed("TestWithDebug", "~/js/output_3.js");

            var tag = debugJavaScriptBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanRenderDebugTagsTwice()
        {
            debugJavaScriptBundle
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .AsNamed("TestWithDebug", "~/js/output_4.js");

            debugJavaScriptBundle2
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .AsNamed("TestWithDebug", "~/js/output_4.js");

            var tag1 = debugJavaScriptBundle.RenderNamed("TestWithDebug");
            var tag2 = debugJavaScriptBundle2.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag1);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag2);
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            debugJavaScriptBundle
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .AsNamed("NamedWithDebug", "~/js/output_5.js");

            var tag = debugJavaScriptBundle.RenderNamed("NamedWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanCreateBundleWithNullMinifer()
        {
            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Render("~/js/output_6.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_6.js?r=F531977E16553414B260BD74C5CAB7F2\"></script>", tag);
            Assert.AreEqual(javaScript, fileWriterFactory.Files[@"C:\js\output_6.js"]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .WithMinifier(JavaScriptMinifiers.JsMin)
                .Render("~/js/output_7.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_7.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[@"C:\js\output_7.js"]);
        }

        [Test]
        public void CanCreateBundleWithClosureMinifer()
        {
            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .WithMinifier(JavaScriptMinifiers.Closure)
                .Render("~/js/output_8.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_8.js?r=00DFDFFC4078EFF6DFCC6244EAB77420\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b};\r\n", fileWriterFactory.Files[@"C:\js\output_8.js"]);
        }

        [Test]
        public void CanRenderOnlyIfFileMissing()
        {
            fileReaderFactory.SetFileExists(false);

            javaScriptBundle
                .Add("~/js/test.js")
                .RenderOnlyIfOutputFileMissing()
                .Render("~/js/output_9.js");

            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_9.js"]);

            fileReaderFactory.SetContents(javaScript2);
            fileReaderFactory.SetFileExists(true);
            javaScriptBundle.ClearCache();

            javaScriptBundle
                .Add("~/js/test.js")
                .RenderOnlyIfOutputFileMissing()
                .Render("~/js/output_9.js");

            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_9.js"]);
        }

        [Test]
        public void CanRerenderFiles()
        {
            fileReaderFactory.SetFileExists(false);

            javaScriptBundle
                .Add("~/js/test.js")
                .Render("~/js/output_10.js");

            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_10.js"]);

            fileReaderFactory.SetContents(javaScript2);
            fileReaderFactory.SetFileExists(true);
            fileWriterFactory.Files.Clear();
            javaScriptBundle.ClearCache();

            javaScriptBundle2
                .Add("~/js/test.js")
                .Render("~/js/output_10.js");

            Assert.AreEqual("function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_10.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithHashInFilename()
        {
            var tag = javaScriptBundle
                .Add("~/js/test.js")
                .Render("~/js/output_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_8E8C548F4F6300695269DE689B903BA3.js\"></script>", tag);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\output_8E8C548F4F6300695269DE689B903BA3.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithUnderscoresInName()
        {
            var tag = javaScriptBundle
                .Add("~/js/test_file.js")
                .Render("~/js/outputunder_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/outputunder_8E8C548F4F6300695269DE689B903BA3.js\"></script>", tag);
            Assert.AreEqual("function product(d,c){return d*c}function sum(d,c){return d+c};", fileWriterFactory.Files[@"C:\js\outputunder_8E8C548F4F6300695269DE689B903BA3.js"]);
        }
    }
}