using SquishIt.Framework;
using NUnit.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CssBundleTests
    {
        private string css = TestUtilities.NormalizeLineEndings(@" li {
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
                                }");

        private string css2 = TestUtilities.NormalizeLineEndings(@" li {
                                    margin-bottom:0.1em;
                                    margin-left:0;
                                    margin-top:0.1em;
                                }

                                th {
                                    font-weight:normal;
                                    vertical-align:bottom;
                                }");


        private string cssLess = TestUtilities.NormalizeLineEndings(@"@brand_color: #4D926F;

                                    #header {
                                        color: @brand_color;
                                    }
 
                                    h2 {
                                        color: @brand_color;
                                    }");

        private CssBundleFactory cssBundleFactory;
        private IHasher hasher;

        [SetUp]
        public void Setup()
        {
            cssBundleFactory = new CssBundleFactory();
            var retryableFileOpener = new RetryableFileOpener();
            hasher = new Hasher(retryableFileOpener);
        }

        [Test]
        public void CanAddMultiplePathFiles()
        {
            var cssBundle1 = cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            var cssBundle2 = cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            cssBundle1.Add("/css/first.css", "/css/second.css");
            cssBundle2.Add("/css/first.css").Add("/css/second.css");

            var cssBundle1Assets = cssBundle1.GroupBundles["default"].Assets;
            var cssBundle2Assets = cssBundle1.GroupBundles["default"].Assets;

            Assert.AreEqual(cssBundle1Assets.Count, cssBundle2Assets.Count);
            for (var i = 0; i < cssBundle1Assets.Count; i++)
            {
                var assetBundle1 = cssBundle1Assets[i];
                var assetBundle2 = cssBundle2Assets[i];
                Assert.AreEqual(assetBundle1.LocalPath, assetBundle2.LocalPath);
                Assert.AreEqual(assetBundle1.Order, assetBundle2.Order);
            }
        }

        [Test]
        public void CanAddMultiplePathFilesToGroup()
        {
            var myGroup = "myGroup";
            var cssBundle1 = cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            var cssBundle2 = cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            cssBundle1.AddToGroup(myGroup, "/css/first.css", "/css/second.css");
            cssBundle2.AddToGroup(myGroup, "/css/first.css").AddToGroup(myGroup, "/css/second.css");

            var cssBundle1Assets = cssBundle1.GroupBundles[myGroup].Assets;
            var cssBundle2Assets = cssBundle1.GroupBundles[myGroup].Assets;

            Assert.AreEqual(cssBundle1Assets.Count, cssBundle2Assets.Count);
            for (var i = 0; i < cssBundle1Assets.Count; i++)
            {
                var assetBundle1 = cssBundle1Assets[i];
                var assetBundle2 = cssBundle2Assets[i];
                Assert.AreEqual(assetBundle1.LocalPath, assetBundle2.LocalPath);
                Assert.AreEqual(assetBundle1.Order, assetBundle2.Order);
            }
        }
        
        [Test]
        public void CanBundleCss()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(css);

            string tag = cssBundle

                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output.css")]);
        }

        [Test]
        public void CanBundleCssSpecifyingOutputLinkPath()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(css);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithOutputBaseHref("http//subdomain.domain.com")
                            .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"http//subdomain.domain.com/css/output.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output.css")]);
        }

        [Test]
        public void CanBundleCssWithQueryStringParameter()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithContents(css)
                .WithDebuggingEnabled(false)
                .Create();

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .Render("/css/output_querystring.css?v=1");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_querystring.css?v=1&r=C33D1225DED9D889876CEE87754EE305\" />", tag);
        }

        [Test]
        public void CanBundleCssWithMediaAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithAttribute("media", "screen")
                            .Render("/css/css_with_media_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/css_with_media_output.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\css_with_media_output.css")]);
        }

        [Test]
        public void CanBundleCssWithRemote()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                            .AddRemote("/css/first.css", "http://www.someurl.com/css/first.css")
                            .Add("/css/second.css")
                            .Render("/css/output_remote.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.someurl.com/css/first.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_remote.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output_remote.css")]);
        }

        [Test]
        public void CanBundleCssWithEmbedded()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                            .AddEmbeddedResource("/css/first.css", "SquishIt.Tests://EmbeddedResource.Embedded.css")
                            .Render("/css/output_embedded.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_embedded.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output_embedded.css")]);
        }

        [Test]
        public void CanDebugBundleCssWithEmbedded()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                            .AddEmbeddedResource("/css/first.css", "SquishIt.Tests://EmbeddedResource.Embedded.css")
                            .Render("/css/output_embedded.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
        }

        [Test]
        public void CanBundleCssWithLess()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(cssLess)
                .Create();

            string tag = cssBundle
                .Add("~/css/test.less")
                .Render("~/css/output.css");

            string contents = cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output.css")];

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />", tag);
            Assert.AreEqual("#header{color:#4d926f}h2{color:#4d926f}", contents);
        }

        [Test]
        public void CanBundleCssWithLessAndPathRewrites()
        {
            string css =
                    @"@brand_color: #4D926F;
                        #header {
                            color: @brand_color;
                            background-image: url(../image/mygif.gif);
                        }
                    ";

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                .Add("~/css/something/test.less")
                .Render("~/css/output_less_with_rewrites.css");

            string contents = cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output_less_with_rewrites.css")];

            Assert.AreEqual("#header{color:#4d926f;background-image:url(image/mygif.gif)}", contents);
        }

        [Test]
        public void CanBundleCssWithLessWithLessDotCssFileExtension()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(cssLess)
                .Create();

            string tag = cssBundle
                .Add("~/css/test.less.css")
                .Render("~/css/output_less_dot_css.css");

            string contents = cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output_less_dot_css.css")];

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output_less_dot_css.css?r=15D3D9555DEFACE69D6AB9E7FD972638\" />", tag);
            Assert.AreEqual("#header{color:#4d926f}h2{color:#4d926f}", contents);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            cssBundle
                    .Add("~/css/temp.css")
                    .AsNamed("Test", "~/css/output.css");

            string tag = cssBundle.RenderNamed("Test");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output.css")]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/output.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            cssBundle
                    .Add("~/css/temp1.css")
                    .Add("~/css/temp2.css")
                    .AsNamed("TestWithDebug", "~/css/output.css");

            string tag = cssBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/temp1.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"css/temp2.css\" />\n", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithMediaAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            cssBundle
                    .Add("~/css/temp.css")
                    .WithAttribute("media", "screen")
                    .AsNamed("TestWithMedia", "~/css/output.css");

            string tag = cssBundle.RenderNamed("TestWithMedia");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output.css")]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"css/output.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
        }

        [Test]
        public void CanRenderDebugTags()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                .Add("/css/first.css")
                .Add("/css/second.css")
                .Render("/css/output.css");

            Assert.AreEqual(tag, "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/second.css\" />\n");
        }

        [Test]
        public void CanRenderDebugTagsTwice()
        {
            CSSBundle cssBundle1 = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            CSSBundle cssBundle2 = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            string tag1 = cssBundle1
                .Add("/css/first.css")
                .Add("/css/second.css")
                .Render("/css/output.css");

            string tag2 = cssBundle2
                .Add("/css/first.css")
                .Add("/css/second.css")
                .Render("/css/output.css");

            Assert.AreEqual(tag1, "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/second.css\" />\n");
            Assert.AreEqual(tag2, "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/second.css\" />\n");
        }

        [Test]
        public void CanRenderDebugTagsWithMediaAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                .Add("/css/first.css")
                .Add("/css/second.css")
                .WithAttribute("media", "screen")
                .Render("/css/output.css");

            Assert.AreEqual(tag, "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/second.css\" />\n");
        }

        [Test]
        public void CanBundleCssWithCompressorAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                 .WithDebuggingEnabled(false)
                 .WithContents(css)
                 .Create();

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithMinifier<YuiCompressor>()
                            .Render("/css/css_with_compressor_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/css_with_compressor_output.css?r=1D0C7C68EDD1B4BD490CCE557E427268\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual(" li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:400;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:400;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\css_with_compressor_output.css")]);
        }

        [Test]
        public void CanBundleCssWithNullCompressorAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithMinifier<NullCompressor>()
                            .Render("/css/css_with_null_compressor_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/css_with_null_compressor_output.css?r=54F52AC95333FEFD5243AC373F573A07\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual(css + "\n" + css + "\n", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\css_with_null_compressor_output.css")]);
        }

        [Test]
        public void CanBundleCssWithCompressorInstance()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(css);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithMinifier<MsCompressor>()
                            .Render("/css/compressor_instance.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/compressor_instance.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\compressor_instance.css")]);
        }

        [Test]
        public void CanRenderOnlyIfFileMissing()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            cssBundleFactory.FileReaderFactory.SetFileExists(false);

            cssBundle
                .Add("/css/first.css")
                .Render("~/css/can_render_only_if_file_missing.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\can_render_only_if_file_missing.css")]);

            cssBundleFactory.FileReaderFactory.SetContents(css2);
            cssBundleFactory.FileReaderFactory.SetFileExists(true);
            cssBundle.ClearCache();

            cssBundle
                .Add("/css/first.css")
                .RenderOnlyIfOutputFileMissing()
                .Render("~/css/can_render_only_if_file_missing.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\can_render_only_if_file_missing.css")]);
        }

        [Test]
        public void CanRerenderFiles()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            cssBundleFactory.FileReaderFactory.SetFileExists(false);

            cssBundle.ClearCache();
            cssBundle
                .Add("/css/first.css")
                .Render("~/css/can_rerender_files.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\can_rerender_files.css")]);

            CSSBundle cssBundle2 = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(css2)
                .Create();

            cssBundleFactory.FileReaderFactory.SetFileExists(true);
            cssBundleFactory.FileWriterFactory.Files.Clear();
            cssBundle.ClearCache();

            cssBundle2
                .Add("/css/first.css")
                .Render("~/css/can_rerender_files.css");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\can_rerender_files.css")]);
        }

        [Test]
        public void CanRenderCssFileWithHashInFileName()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .Render("/css/output_#.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_C33D1225DED9D889876CEE87754EE305.css\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\output_C33D1225DED9D889876CEE87754EE305.css")]);
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

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(importCss);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(@"C:\css\other.css", "#footer{color:#ffffff}");

            cssBundle
                            .Add("/css/first.css")
                            .Render("/css/unprocessed_import.css");

            Assert.AreEqual(@"@import url(""/css/other.css"");#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\unprocessed_import.css")]);
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


            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(importCss);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\other.css"), "#footer{color:#ffffff}");

            string tag = cssBundle
                            .Add("/css/first.css")
                            .ProcessImports()
                            .Render("/css/processed_import.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\processed_import.css")]);
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

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\other.css"), "#footer{color:#ffffff}");

            string tag = cssBundle
                            .Add("/css/first.css")
                            .ProcessImports()
                            .Render("/css/processed_import_noquotes.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\processed_import_noquotes.css")]);
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

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\other.css"), "#footer{color:#ffffff}");

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import_singlequotes.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\processed_import_singlequotes.css")]);
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

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\other.css"), "#footer{color:#ffffff}");

            string tag = cssBundle
                            .Add("/css/first.css")
                            .ProcessImports()
                            .Render("/css/processed_import_uppercase.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\processed_import_uppercase.css")]);
        }

        [Test]
        public void CanCreateNamedBundleWithForceRelease()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            cssBundle
                    .Add("~/css/temp.css")
                    .ForceRelease()
                    .AsNamed("TestForce", "~/css/named_withforce.css");

            string tag = cssBundle.RenderNamed("TestForce");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePathRelativeToWorkingDirectory(@"C:\css\named_withforce.css")]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/named_withforce.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
        }

        [Test]
        public void CanBundleCssWithArbitraryAttributes()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents(css)
                    .Create();

            string tag = cssBundle
                                            .Add("/css/first.css")
                                            .Add("/css/second.css")
                                            .WithAttribute("media", "screen")
                                            .WithAttribute("test", "other")
                                            .Render("/css/css_with_attribute_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" test=\"other\" href=\"/css/css_with_attribute_output.css?r=C33D1225DED9D889876CEE87754EE305\" />", tag);
        }

        [Test]
        public void CanBundleDebugCssWithArbitraryAttributes()
        {
            CSSBundle cssBundle = cssBundleFactory
                    .WithDebuggingEnabled(true)
                    .WithContents(css)
                    .Create();

            string tag = cssBundle
                .Add("/css/first.css")
                .Add("/css/second.css")
                .WithAttribute("media", "screen")
                .WithAttribute("test", "other")
                .Render("/css/css_with_debugattribute_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" test=\"other\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" test=\"other\" href=\"/css/second.css\" />\n", tag);
        }

        [Test]
        public void CanCreateCachedBundle()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                .Add("~/css/temp.css")
                .AsCached("TestCached", "~/static/css/TestCached.css");

            string contents = cssBundle.RenderCached("TestCached");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", contents);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"static/css/TestCached.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
        }

        [Test]
        public void CanCreateCachedBundleAssetTag()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            cssBundle
                .Add("~/css/temp.css")
                .AsCached("TestCached", "~/static/css/TestCached.css");

            string contents = cssBundle.RenderCached("TestCached");
            cssBundle.ClearCache();
            string tag = cssBundle.RenderCachedAssetTag("TestCached");

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}", contents);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"static/css/TestCached.css?r=67F81278D746D60E6F711B5A29747388\" />", tag);
        }

        [Test]
        public void CanCreateCachedBundleInDebugMode()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                .Add("~/css/temp.css")
                .AsCached("TestCached", "~/static/css/TestCached.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/temp.css\" />\n", tag);
        }
    }
}