using SquishIt.Framework;
using NUnit.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Css.Compressors;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
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

        private string css2 = @" li {
                                    margin-bottom:0.1em;
                                    margin-left:0;
                                    margin-top:0.1em;
                                }

                                th {
                                    font-weight:normal;
                                    vertical-align:bottom;
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/output.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\output.css"]);
        }

        [Test]
        public void CanBundleCssWithQueryStringParameter()
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
                            .Render("/css/output_querystring.css?v=1");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/output_querystring.css?v=1&r=C33D1225DED9D889876CEE87754EE305\" />", tag);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/css_with_media_output.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\css_with_media_output.css"]);
        }

        [Test]
        public void CanBundleCssWithRemote()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .AddRemote("/css/first.css", "http://www.someurl.com/css/first.css")
                            .Add("/css/second.css")
                            .Render("/css/output_remote.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"http://www.someurl.com/css/first.css\" /><link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/output_remote.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\output_remote.css"]);
        }

        [Test]
        public void CanBundleCssWithEmbedded()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .AddEmbeddedResource("/css/first.css", "SquishIt.Tests://SquishIt.Tests.EmbeddedResource.Embedded.css")
                            .Render("/css/output_embedded.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/output_embedded.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\output_embedded.css"]);
        }

        [Test]
        public void CanDebugBundleCssWithEmbedded()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .AddEmbeddedResource("/css/first.css", "SquishIt.Tests://SquishIt.Tests.EmbeddedResource.Embedded.css")
                            .Render("/css/output_embedded.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/first.css\" />", tag);
            Assert.AreEqual(0, mockFileWriterFactory.Files.Count);
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

            string contents = mockFileWriterFactory.Files[@"C:\css\output.css"];

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/output.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />", tag);
            Assert.AreEqual("#header{color:#4d926f}h2{color:#4d926f}", contents);
        }

        [Test]
        public void CanBundleCssWithLessAndPathRewrites()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();

            string css =
                    @"@brand_color: #4D926F;
                        #header {
                            color: @brand_color;
                            background-image: url(../image/mygif.gif);
                        }
                    ";
            
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                .Add("~/css/something/test.less")
                .Render("~/css/output_less_with_rewrites.css");

            string contents = mockFileWriterFactory.Files[@"C:\css\output_less_with_rewrites.css"];

            Assert.AreEqual("#header{color:#4d926f;background-image:url(image/mygif.gif)}", contents);
        }

        [Test]
        public void CanBundleCssWithLessWithLessDotCssFileExtension()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(cssLess);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                .Add("~/css/test.less.css")
                .Render("~/css/output_less_dot_css.css");

            string contents = mockFileWriterFactory.Files[@"C:\css\output_less_dot_css.css"];

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/output_less_dot_css.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />", tag);
            Assert.AreEqual("#header{color:#4d926f}h2{color:#4d926f}", contents);
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

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\output.css"]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/output.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
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

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\output.css"]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"css/output.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
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
        public void CanRenderDebugTagsTwice()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle1 = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            ICssBundle cssBundle2 = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag1 = cssBundle1
                .Add("/css/first.css")
                .Add("/css/second.css")
                .Render("/css/output.css");

            string tag2 = cssBundle2
                .Add("/css/first.css")
                .Add("/css/second.css")
                .Render("/css/output.css");

            Assert.AreEqual(tag1, "<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/first.css\" /><link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/second.css\" />");
            Assert.AreEqual(tag2, "<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/first.css\" /><link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/second.css\" />");
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

        [Test]
        public void CanBundleCssWithCompressorAttribute()
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
                            .WithCompressor(CssCompressors.YuiCompressor)
                            .Render("/css/css_with_compressor_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/css_with_compressor_output.css?r=AE4C10DB94E5420AD54BD0A0BE9F02C2\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}li{margin-bottom:.1em;margin-left:0;margin-top:.1em;}th{font-weight:normal;vertical-align:bottom;}.FloatRight{float:right;}.FloatLeft{float:left;}", mockFileWriterFactory.Files[@"C:\css\css_with_compressor_output.css"]);
        }

        [Test]
        public void CanBundleCssWithNullCompressorAttribute()
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
                            .WithCompressor(CssCompressors.NullCompressor)
                            .Render("/css/css_with_null_compressor_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/css_with_null_compressor_output.css?r=9650CBE3E753DF5F9146A2AF738A8272\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual(css + css, mockFileWriterFactory.Files[@"C:\css\css_with_null_compressor_output.css"]);
        }

        [Test]
        public void CanRenderOnlyIfFileMissing()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            mockFileReaderFactory.SetFileExists(false);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                .Add("/css/first.css")
                .RenderOnlyIfOutputFileMissing()
                .Render("~/css/can_render_only_if_file_missing.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\can_render_only_if_file_missing.css"]);

            mockFileReaderFactory.SetContents(css2);
            mockFileReaderFactory.SetFileExists(true);
            cssBundle.ClearCache();

            cssBundle
                .Add("/css/first.css")
                .RenderOnlyIfOutputFileMissing()
                .Render("~/css/can_render_only_if_file_missing.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\can_render_only_if_file_missing.css"]);
        }

        [Test]
        public void CanRerenderFiles()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            mockFileReaderFactory.SetFileExists(false);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);
            cssBundle.ClearCache();
            cssBundle
                .Add("/css/first.css")
                .Render("~/css/can_rerender_files.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\can_rerender_files.css"]);

            mockFileReaderFactory.SetContents(css2);
            mockFileReaderFactory.SetFileExists(true);
            mockFileWriterFactory.Files.Clear();
            cssBundle.ClearCache();

            ICssBundle cssBundle2 = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle2
                .Add("/css/first.css")
                .Render("~/css/can_rerender_files.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}", mockFileWriterFactory.Files[@"C:\css\can_rerender_files.css"]);
        }

        [Test]
        public void CanRenderCssFileWithHashInFileName()
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
                            .Render("/css/output_#.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"/css/output_C33D1225DED9D889876CEE87754EE305.css\" />", tag);
            Assert.AreEqual(1, mockFileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\output_C33D1225DED9D889876CEE87754EE305.css"]);
        }
  
        [Test]
        public void CanRenderCssFileWithUnprocessedImportStatement()
        {
            string importCss =
                                    @"
                                    @import url(""/css/other.css"");
                                    #header {
                                        color: #4D926F;
                                    }";

            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(importCss);
            mockFileReaderFactory.SetContentsForFile(@"C:\css\other.css", "#footer{color:#ffffff}");

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                            .Add("/css/first.css")
                            .Render("/css/unprocessed_import.css");

            Assert.AreEqual(@"@import url(""/css/other.css"");#header{color:#4d926f}", mockFileWriterFactory.Files[@"C:\css\unprocessed_import.css"]);
        }

        [Test]
        public void CanRenderCssFileWithImportStatement()
        {
            string importCss =
                                    @"
                                    @import url(""/css/other.css"");
                                    #header {
                                        color: #4D926F;
                                    }";

            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(importCss);
            mockFileReaderFactory.SetContentsForFile(@"C:\css\other.css", "#footer{color:#ffffff}");

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .ProcessImports()
                            .Render("/css/processed_import.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", mockFileWriterFactory.Files[@"C:\css\processed_import.css"]);
        }

        [Test]
        public void CanRenderCssFileWithImportStatementNoQuotes()
        {
            string importCss =
                                    @"
                                    @import url(/css/other.css);
                                    #header {
                                        color: #4D926F;
                                    }";

            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(importCss);
            mockFileReaderFactory.SetContentsForFile(@"C:\css\other.css", "#footer{color:#ffffff}");

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .ProcessImports()
                            .Render("/css/processed_import_noquotes.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", mockFileWriterFactory.Files[@"C:\css\processed_import_noquotes.css"]);
        }

        [Test]
        public void CanRenderCssFileWithImportStatementSingleQuotes()
        {
            string importCss =
                                    @"
                                    @import url('/css/other.css');
                                    #header {
                                        color: #4D926F;
                                    }";

            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(importCss);
            mockFileReaderFactory.SetContentsForFile(@"C:\css\other.css", "#footer{color:#ffffff}");

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import_singlequotes.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", mockFileWriterFactory.Files[@"C:\css\processed_import_singlequotes.css"]);
        }

        [Test]
        public void CanRenderCssFileWithImportStatementUppercase()
        {
            string importCss =
                                    @"
                                    @IMPORT URL(/css/other.css);
                                    #header {
                                        color: #4D926F;
                                    }";

            var mockDebugStatusReader = new StubDebugStatusReader(false);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(importCss);
            mockFileReaderFactory.SetContentsForFile(@"C:\css\other.css", "#footer{color:#ffffff}");

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .ProcessImports()
                            .Render("/css/processed_import_uppercase.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", mockFileWriterFactory.Files[@"C:\css\processed_import_uppercase.css"]);
        }

        [Test]
        public void CanCreateNamedBundleWithForceRelease()
        {
            var mockDebugStatusReader = new StubDebugStatusReader(true);
            var mockFileWriterFactory = new StubFileWriterFactory();
            var mockFileReaderFactory = new StubFileReaderFactory();
            mockFileReaderFactory.SetContents(css);

            ICssBundle cssBundle = new CssBundle(mockDebugStatusReader,
                                                 mockFileWriterFactory,
                                                 mockFileReaderFactory);

            cssBundle
                    .Add("~/css/temp.css")
                    .ForceRelease()
                    .AsNamed("TestForce", "~/css/named_withforce.css");

            string tag = cssBundle.RenderNamed("TestForce");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", mockFileWriterFactory.Files[@"C:\css\named_withforce.css"]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\"  href=\"css/named_withforce.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
        }
    }
}