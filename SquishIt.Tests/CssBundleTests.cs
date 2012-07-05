using System;
using System.Web;
using Moq;
using NUnit.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Files;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;
using SquishIt.Framework;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CssBundleTests
    {
        string css = TestUtilities.NormalizeLineEndings(@" li {
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
        string minifiedCss = "li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}";

        string css2 = TestUtilities.NormalizeLineEndings(@" li {
                                    margin-bottom:0.1em;
                                    margin-left:0;
                                    margin-top:0.1em;
                                }

                                th {
                                    font-weight:normal;
                                    vertical-align:bottom;
                                }");

        string minifiedCss2 =
            "li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}";



        CssBundleFactory cssBundleFactory;
        IHasher hasher;

        [SetUp]
        public void Setup()
        {
            cssBundleFactory = new CssBundleFactory();
            var retryableFileOpener = new RetryableFileOpener();
            hasher = new Hasher(retryableFileOpener);
        }

        [Test]
        public void CanBundleCss()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")]);
        }

        [Test]
        public void CanBundleCssSpecifyingOutputLinkPath()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithOutputBaseHref("http//subdomain.domain.com")
                            .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"http//subdomain.domain.com/css/output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")]);
        }

        [Test]
        public void CanBundleCssVaryingOutputBaseHrefRendersIndependentUrl()
        {
            //Verify that depending on basehref, we get independantly cached and returned URLs
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithOutputBaseHref("http://subdomain.domain.com")
                            .Render("/css/output.css");

            CSSBundle cssBundleNoBaseHref = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            string tagNoBaseHref = cssBundleNoBaseHref
                            .Add(firstPath)
                            .Add(secondPath)
                            .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"http://subdomain.domain.com/css/output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tagNoBaseHref);
        }

        [Test]
        public void RenderNamedUsesOutputBaseHref()
        {
            //Verify that depending on basehref, we get independantly cached and returned URLs
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(css);

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            cssBundle
                .Add(firstPath)
                .Add(secondPath)
                .WithOutputBaseHref("http://subdomain.domain.com")
                .AsNamed("leBundle", "/css/output.css");

            var tag = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create()
                .WithOutputBaseHref("http://subdomain.domain.com")
                .RenderNamed("leBundle");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"http://subdomain.domain.com/css/output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
        }

        [Test]
        public void CanBundleCssWithQueryStringParameter()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithContents(css)
                .WithDebuggingEnabled(false)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .Render("/css/output_querystring.css?v=1");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_querystring.css?v=1&r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
        }

        [Test]
        public void CanBundleCssWithoutRevisionHash()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithContents(css)
                .WithDebuggingEnabled(false)
                .Create();

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Add("/css/second.css")
                            .WithoutRevisionHash()
                            .Render("/css/output_querystring.css?v=1");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_querystring.css?v=1\" />", tag);
        }

        [Test]
        public void CanBundleCssWithMediaAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithAttribute("media", "screen")
                            .Render("/css/css_with_media_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/css_with_media_output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\css_with_media_output.css")]);
        }

        [Test]
        public void CanBundleCssWithRemote()
        {
            //this is rendering tag correctly but incorrectly(?) merging both files
            using(new ResolverFactoryScope(typeof(Framework.Resolvers.HttpResolver).FullName, StubResolver.ForFile("http://www.someurl.com/css/first.css")))
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
                Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output_remote.css")]);
            }
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
            Assert.AreEqual(minifiedCss
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output_embedded.css")]);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
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

            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")]);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/temp1.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"css/temp2.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
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

            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output.css")]);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/second.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanRenderPreprocessedDebugTags()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .WithContents(css)
                .Create();

            string tag = cssBundle
                .WithPreprocessor(new StubStylePreprocessor())
                .Add("~/first.style.css")
                .Render("/css/output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"first.style.css.squishit.debug.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/second.css\" />\n", TestUtilities.NormalizeLineEndings(tag1));
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/second.css\" />\n", TestUtilities.NormalizeLineEndings(tag2));
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/css/second.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanBundleCssWithCompressorAttribute()
        {
            var cssBundle = cssBundleFactory
                .WithHasher(hasher)
                 .WithDebuggingEnabled(false)
                 .WithContents(css)
                 .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            var tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithMinifier<YuiCompressor>()
                            .Render("/css/css_with_compressor_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/css_with_compressor_output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\css_with_compressor_output.css")]);
        }

        [Test]
        public void CanBundleCssWithNullCompressorAttribute()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithMinifier<NullCompressor>()
                            .Render("/css/css_with_null_compressor_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/css_with_null_compressor_output.css?r=703408685CD7BEBA456CEBDC6271B02F\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual(css + "\n" + css2 + "\n", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\css_with_null_compressor_output.css")]);
        }

        [Test]
        public void CanBundleCssWithCompressorInstance()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithMinifier<MsCompressor>()
                            .Render("/css/compressor_instance.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/compressor_instance.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}"
                            , cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\compressor_instance.css")]);
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

            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\can_render_only_if_file_missing.css")]);

            cssBundleFactory.FileReaderFactory.SetContents(css2);
            cssBundleFactory.FileReaderFactory.SetFileExists(true);
            cssBundle.ClearCache();

            cssBundle
                .Add("/css/first.css")
                .RenderOnlyIfOutputFileMissing()
                .Render("~/css/can_render_only_if_file_missing.css");

            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\can_render_only_if_file_missing.css")]);
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

            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\can_rerender_files.css")]);

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

            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\can_rerender_files.css")]);
        }

        [Test]
        public void CanRenderCssFileWithHashInFileName()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                            .Add(firstPath)
                            .Add(secondPath)
                            .Render("/css/output_#.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/output_E621107093FBAB71C1FC4A4200B71AD4.css\" />", tag);
            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\output_E621107093FBAB71C1FC4A4200B71AD4.css")]);
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

            Assert.AreEqual(@"@import url(""/css/other.css"");#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\unprocessed_import.css")]);
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
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css\other.css"), "#footer{color:#ffffff}");

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\processed_import.css")]);
        }

        [Test]
        public void CanRenderCssFileWithRelativeImportStatement()
        {
            string importCss =
                                    @"
                                    @import url(""other.css"");
                                    #header {
                                        color: #4D926F;
                                    }";


            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(importCss)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(importCss);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css\other.css"), "#footer{color:#ffffff}");

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\processed_import.css")]);
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

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css\other.css"), "#footer{color:#ffffff}");

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import_noquotes.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\processed_import_noquotes.css")]);
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

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css\other.css"), "#footer{color:#ffffff}");

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import_singlequotes.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\processed_import_singlequotes.css")]);
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

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(@"css\other.css"), "#footer{color:#ffffff}");

            cssBundle
                .Add("/css/first.css")
                .ProcessImports()
                .Render("/css/processed_import_uppercase.css");

            Assert.AreEqual("#footer{color:#fff}#header{color:#4d926f}", cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\processed_import_uppercase.css")]);
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

            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"css\named_withforce.css")]);
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

            var firstPath = "first.css";
            var secondPath = "second.css";

            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), css);
            cssBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), css2);

            string tag = cssBundle
                                            .Add(firstPath)
                                            .Add(secondPath)
                                            .WithAttribute("media", "screen")
                                            .WithAttribute("test", "other")
                                            .Render("/css/css_with_attribute_output.css");

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" test=\"other\" href=\"/css/css_with_attribute_output.css?r=E621107093FBAB71C1FC4A4200B71AD4\" />", tag);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" test=\"other\" href=\"/css/first.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" test=\"other\" href=\"/css/second.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
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

            Assert.AreEqual(minifiedCss, contents);
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

            Assert.AreEqual(minifiedCss, contents);
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

            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/temp.css\" />\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.css");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.css");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, css2);
                frf.SetContentsForFile(file2, css);

                var writerFactory = new StubFileWriterFactory();

                var tag = cssBundleFactory.WithDebuggingEnabled(true)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.css");

                var expectedTag = string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}/file1.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}/file2.css\" />\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug_Ignores_Duplicates()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.css");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.css");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, css2);
                frf.SetContentsForFile(file2, css);

                var writerFactory = new StubFileWriterFactory();

                var tag = cssBundleFactory.WithDebuggingEnabled(true)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.css");

                var expectedTag = string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}/file1.css\" />\n<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}/file2.css\" />\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug_Writes_And_Ignores_Preprocessed_Debug_Files()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.style.css");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file1.style.css.squishit.debug.css");
            var content = "some stuffs";

            var preprocessor = new StubStylePreprocessor();

            using(new ScriptPreprocessorScope<StubStylePreprocessor>(preprocessor))
            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, content);

                var writerFactory = new StubFileWriterFactory();

                var tag = cssBundleFactory.WithDebuggingEnabled(true)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.css");

                var expectedTag = string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}/file1.style.css.squishit.debug.css\" />\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
                Assert.AreEqual(content, preprocessor.CalledWith);
                Assert.AreEqual(1, writerFactory.Files.Count);
                Assert.AreEqual("styley", writerFactory.Files[file1 + ".squishit.debug.css"]);
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInRelease()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.css");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.css");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, css2);
                frf.SetContentsForFile(file2, css);

                var writerFactory = new StubFileWriterFactory();

                var tag = cssBundleFactory.WithDebuggingEnabled(false)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.css");

                var expectedTag = "<link rel=\"stylesheet\" type=\"text/css\" href=\"output.css?r=hashy\" />";
                Assert.AreEqual(expectedTag, tag);

                var combined = "li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}";
                Assert.AreEqual(combined, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.css")]);
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInRelease_Ignores_Duplicates()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.css");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.css");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, css2);
                frf.SetContentsForFile(file2, css);

                var writerFactory = new StubFileWriterFactory();

                var tag = cssBundleFactory.WithDebuggingEnabled(false)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Add(file1)
                        .Render("~/output.css");

                var expectedTag = "<link rel=\"stylesheet\" type=\"text/css\" href=\"output.css?r=hashy\" />";
                Assert.AreEqual(expectedTag, tag);

                var combined = "li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}";
                Assert.AreEqual(combined, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.css")]);
            }
        }

        [Test]
        public void CanRenderArbitraryStringsInDebug()
        {
            var css2Format = "{0}{1}";

            var hrColor = "hr {color:sienna;}";
            var p = "p {margin-left:20px;}";

            var tag = new CssBundleFactory()
                .WithDebuggingEnabled(true)
                .Create()
                .AddString(css)
                .AddString(css2Format, new[] { hrColor, p })
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<style type=\"text/css\">{0}</style>\n<style type=\"text/css\">{1}</style>\n", css, string.Format(css2Format, hrColor, p));
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanMaintainOrderBetweenArbitraryAndFileAssetsInRelease()
        {
            var file1 = "somefile.css";
            var file2 = "anotherfile.css";

            var arbitraryCss = ".someClass { color:red }";
            var minifiedArbitraryCss = ".someClass{color:red}";

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file1), css);
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file2), css2);

            var writerFactory = new StubFileWriterFactory();

            var tag = new CssBundleFactory()
                .WithFileReaderFactory(readerFactory)
                .WithFileWriterFactory(writerFactory)
                .WithDebuggingEnabled(false)
                .Create()
                .Add(file1)
                .AddString(arbitraryCss)
                .Add(file2)
                .Render("test.css");

            var expectedTag = string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"test.css?r=hash\" />");
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var combined = minifiedCss + minifiedArbitraryCss + minifiedCss2;
            Assert.AreEqual(combined, writerFactory.Files[TestUtilities.PrepareRelativePath(@"test.css")]);
        }

        [Test]
        public void CanMaintainOrderBetweenArbitraryAndFileAssetsInDebug()
        {
            var file1 = "somefile.css";
            var file2 = "anotherfile.css";

            var arbitraryCss = ".someClass { color:red }";

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file1), css);
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file2), css2);

            var writerFactory = new StubFileWriterFactory();

            var tag = new CssBundleFactory()
                .WithFileReaderFactory(readerFactory)
                .WithFileWriterFactory(writerFactory)
                .WithDebuggingEnabled(true)
                .Create()
                .Add(file1)
                .AddString(arbitraryCss)
                .Add(file2)
                .Render("test.css");

            var expectedTag = string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"somefile.css\" />\n<style type=\"text/css\">{0}</style>\n<link rel=\"stylesheet\" type=\"text/css\" href=\"anotherfile.css\" />\n"
                , arbitraryCss);
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanRenderArbitraryStringsInDebugWithoutType()
        {
            var css2Format = "{0}{1}";

            var hrColor = "hr {color:sienna;}";
            var p = "p {margin-left:20px;}";

            var tag = new CssBundleFactory()
                .WithDebuggingEnabled(true)
                .Create()
                .AddString(css)
                .AddString(css2Format, new[] { hrColor, p })
                .WithoutTypeAttribute()
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<style>{0}</style>\n<style>{1}</style>\n", css, string.Format(css2Format, hrColor, p));
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void DoesNotRenderDuplicateArbitraryStringsInDebug()
        {
            var tag = new CssBundleFactory()
                .WithDebuggingEnabled(true)
                .Create()
                .AddString(css)
                .AddString(css)
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<style type=\"text/css\">{0}</style>\n", css);
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanBundleArbitraryContentsInRelease()
        {
            var css2Format = "{0}{1}";

            var hrColor = "hr {color:sienna;}";
            var p = "p {margin-left:20px;}";

            var writerFactory = new StubFileWriterFactory();

            var tag = new CssBundleFactory()
                    .WithDebuggingEnabled(false)
                    .WithFileWriterFactory(writerFactory)
                    .WithHasher(new StubHasher("hashy"))
                    .Create()
                    .AddString(css)
                    .AddString(css2Format, new[] { hrColor, p })
                    .Render("~/output.css");

            var expectedTag = "<link rel=\"stylesheet\" type=\"text/css\" href=\"output.css?r=hashy\" />";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var minifiedScript = "li{margin-bottom:.1em;margin-left:0;margin-top:.1em}th{font-weight:normal;vertical-align:bottom}.FloatRight{float:right}.FloatLeft{float:left}hr{color:#a0522d}p{margin-left:20px}";
            Assert.AreEqual(minifiedScript, writerFactory.Files[TestUtilities.PrepareRelativePath("output.css")]);
        }

        [Test]
        public void PathRewritingDoesNotAffectClassesNamedUrl()
        {
            string css =
                @"
                        a.url {
                            color: #4D926F;
                        }
                    ";

            CSSBundle cssBundle = cssBundleFactory
                .WithDebuggingEnabled(false)
                .WithContents(css)
                .Create();

            cssBundle
                .Add("~/css/something/test.css")
                .Render("~/css/output_rewriting_url.css");

            string contents =
                cssBundleFactory.FileWriterFactory.Files[
                    TestUtilities.PrepareRelativePath(@"css\output_rewriting_url.css")];

            Assert.AreEqual("a.url{color:#4d926f}", contents);
        }

        [Test]
        public void CanUseArbitraryReleaseFileRenderer()
        {
            var renderer = new Mock<IRenderer>();

            var content = "content";

            cssBundleFactory
                .WithDebuggingEnabled(false)
                .Create()
                .WithReleaseFileRenderer(renderer.Object)
                .AddString(content)
                .Render("test.css");

            renderer.Verify(r => r.Render(content, TestUtilities.PrepareRelativePath("test.css")));
        }

        [Test]
        public void CanIgnoreArbitraryReleaseFileRendererIfDebugging()
        {
            var renderer = new Mock<IRenderer>(MockBehavior.Strict);

            var content = "content";

            cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithReleaseFileRenderer(renderer.Object)
                .AddString(content)
                .Render("test.css");

            renderer.VerifyAll();
        }

        [Test]
        public void CanIgnoreArbitraryReleaseRendererInDebug()
        {
            var renderer = new Mock<IRenderer>();

            var content = "content";

            cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithReleaseFileRenderer(renderer.Object)
                .AddString(content)
                .Render("test.css");

            renderer.VerifyAll();
        }


        [Test]
        public void CanIncludeDynamicContentInDebug()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(r => r.Url).Returns(new Uri("http://example.com"));
            context.SetupGet(c => c.Request).Returns(request.Object);

            var bundle = cssBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            using(new HttpContextScope(context.Object))
            {
                bundle.AddDynamic("/some/dynamic/css");
            }

            var tag = bundle.Render("~/combined_#.css");

            Assert.AreEqual(0, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"/some/dynamic/css\" />\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanIncludeDynamicContentInRelease()
        {
            //this doesn't really test the nitty-gritty details (http resolver, download etc...) but its a start
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(r => r.Url).Returns(new Uri("http://example.com"));
            context.SetupGet(c => c.Request).Returns(request.Object);

            var bundle = cssBundleFactory
                .WithContents(css)
                .WithDebuggingEnabled(false)
                .Create();

            using(new HttpContextScope(context.Object))
            {
                bundle.AddDynamic("/some/dynamic/css");
            }

            var tag = bundle.Render("~/combined.css");

            Assert.AreEqual(1, cssBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual(minifiedCss, cssBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath("combined.css")]);
            Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"combined.css?r=hash\" />", tag);
        }

        [Test]
        public void RenderRelease_OmitsRenderedTag_IfOnlyRemoteAssets()
        {
            //this is rendering tag correctly but incorrectly(?) merging both files
            using(new ResolverFactoryScope(typeof(Framework.Resolvers.HttpResolver).FullName, StubResolver.ForFile("http://www.someurl.com/css/first.css")))
            {
                CSSBundle cssBundle = cssBundleFactory
                    .WithHasher(hasher)
                    .WithDebuggingEnabled(false)
                    .WithContents(css)
                    .Create();

                string tag = cssBundle
                    .ForceRelease()
                    .AddRemote("/css/first.css", "http://www.someurl.com/css/first.css")
                    .Render("/css/output_remote.css");

                Assert.AreEqual("<link rel=\"stylesheet\" type=\"text/css\" href=\"http://www.someurl.com/css/first.css\" />", tag);
                Assert.AreEqual(0, cssBundleFactory.FileWriterFactory.Files.Count);
            }
        }

        [Test]
        public void CanRenderDistinctBundlesIfSameOutputButDifferentFileNames()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            CSSBundle cssBundle2 = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            cssBundleFactory.FileReaderFactory.SetContents(css);

            string tag = cssBundle
                            .Add("/css/first.css")
                            .Render("/css/output#.css");

            cssBundleFactory.FileReaderFactory.SetContents(css2);

            string tag2 = cssBundle2
                            .Add("/css/second.css")
                            .Render("/css/output#.css");

            Assert.AreNotEqual(tag, tag2);
        }

        [Test]
        public void CanRenderDistinctBundlesIfSameOutputButDifferentArbitrary()
        {
            CSSBundle cssBundle = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            CSSBundle cssBundle2 = cssBundleFactory
                .WithHasher(hasher)
                .WithDebuggingEnabled(false)
                .Create();

            string tag = cssBundle
                            .AddString(css)
                            .Render("/css/output#.css");

            string tag2 = cssBundle2
                            .AddString(css2)
                            .Render("/css/output#.css");

            Assert.AreNotEqual(tag, tag2);
        }
    }
}