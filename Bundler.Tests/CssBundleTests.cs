using Bundler.Framework;
using Bundler.Framework.Tests.Mocks;
using NUnit.Framework;

namespace Bundler.Tests
{
    [TestFixture]
    public class CssBundleTests
    {
        private string css = @" li {
                                    margin-bottom:0.1em;
                                    margin-left:0;
                                    margin-top:0.1em;
                                }

                                th {
                                    font-weight:normal;
                                    vertical-align:bottom;
                                }

                                .FloatRight {
                                    float:right;
                                }
                                
                                .FloatLeft {
                                    float:left;
                                }";


        private string cssLess =
                                    @"@brand_color: #4D926F;

                                    #header {
                                        color: @brand_color;
                                    }
 
                                    h2 {
                                        color: @brand_color;
                                    }";

        [Test]
        public void CanBundleCss()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/output.css?r=AE4C10DB94E5420AD54BD0A0BE9F02C2\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", mockFileWriterFactory.Files["/css/output.css"]);
        }

        [Test]
        public void CanBundleCssWithMediaAttribute()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithMedia("screen")
                            .Render("/css/css_with_media_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/css_with_media_output.css?r=AE4C10DB94E5420AD54BD0A0BE9F02C2\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", mockFileWriterFactory.Files["/css/css_with_media_output.css"]);
        }

        [Test]
        public void CanBundleCssWithLess()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(cssLess);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                .Add("~/css/test.less")
                .Render("~/css/output.css");

            string contents = mockFileWriterFactory.Files["~/css/output.css"];

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/output.css?r=350F6DC1A87E2503EE6D4AE414C4A479\" />", tag);   
            Assert.AreEqual("#header,h2{color:#4d926f;}", contents);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                    .Add("~/css/temp.css")
                    .AsNamed("Test", "~/css/output.css");

            string tag = cssBundle.RenderNamed("Test");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", mockFileWriterFactory.Files["~/css/output.css"]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/output.css?r=A757BD759BA228D91481798C2C49A8DC\" />", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                    .Add("~/css/temp1.css")
                    .Add("~/css/temp2.css")
                    .AsNamed("TestWithDebug", "~/css/output.css");

            string tag = cssBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/temp1.css\" /><link rel=\"stylesheet\" type=\"text/css\"  href=\"css/temp2.css\" />", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithMediaAttribute()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                    .Add("~/css/temp.css")
                    .WithMedia("screen")
                    .AsNamed("TestWithMedia", "~/css/output.css");

            string tag = cssBundle.RenderNamed("TestWithMedia");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", mockFileWriterFactory.Files["~/css/output.css"]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"css/output.css?r=A757BD759BA228D91481798C2C49A8DC\" />", tag);
        }

        [Test]
        public void CanRenderDebugTags()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                .Add("/css/first.css")
                .Add("/css/second.css")
                .Render("/css/output.css");

            Assert.AreEqual(tag, "<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/first.css\" /><link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/second.css\" />");
        }

        [Test]
        public void CanRenderDebugTagsWithMediaAttribute()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                .Add("/css/first.css")
                .Add("/css/second.css")
                .WithMedia("screen")
                .Render("/css/output.css");

            Assert.AreEqual(tag, "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/first.css\" /><link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/second.css\" />");
        }
    }
}