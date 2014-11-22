using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Moq;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;
using SquishIt.Tests.Helpers;
using HttpContext = SquishIt.Framework.HttpContext;

namespace SquishIt.Tests
{
    [TestFixture]
    public class JavaScriptBundleTests
    {
        string javaScript = TestUtilities.NormalizeLineEndings(@"
																				function product(a, b)
																				{
																						return a * b;
																				}

																				function sum(a, b){
																						return a + b;
																				}");
        string minifiedJavaScript = "function product(n,t){return n*t}function sum(n,t){return n+t};\n";

        string javaScriptPreMinified = "(function() { alert('should end with parens') })();\n";

        string javaScript2 = TestUtilities.NormalizeLineEndings(@"function sum(a, b){
																						return a + b;
																			 }");
        string minifiedJavaScript2 = "function sum(n,t){return n+t};\n";

        JavaScriptBundleFactory javaScriptBundleFactory;

        [SetUp]
        public void Setup()
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory()
                .WithDebuggingEnabled(false)
                .WithHasher(new StubHasher("hash"))
                .WithContents(javaScript);
            //javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript);
        }

        [Test]
        public void CanRenderEmptyBundle_WithHashInFilename()
        {
            javaScriptBundleFactory.Create().Render("~/js/output_#.js");
        }

        [Test]
        public void CanBundleJavaScript()
        {
            var tag = javaScriptBundleFactory
                    .WithDebuggingEnabled(false)
                    .Create()
                    .Add("~/js/test.js")
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleJavascriptWithMinifiedFiles()
        {
            var firstPath = "first.js";
            var secondPath = "second.js";

            javaScriptBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), javaScriptPreMinified);
            javaScriptBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), javaScript2);

            var tag = javaScriptBundleFactory
                .Create()
                .AddMinified(firstPath)
                .Add(secondPath)
                .Render("script.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"script.js?r=hash\"></script>", tag);

            Assert.AreEqual(1, javaScriptBundleFactory.FileWriterFactory.Files.Count);
            var output = TestUtilities.NormalizeLineEndings(javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath("script.js")]);
            Assert.True(output.StartsWith(javaScriptPreMinified));
            Assert.True(output.EndsWith(minifiedJavaScript2));
        }

        [Test]
        public void CanBundleJavascriptWithMinifiedStrings()
        {
            var tag = javaScriptBundleFactory
                .Create()
                .AddMinifiedString(javaScriptPreMinified)
                .AddString(javaScript2)
                .Render("script.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"script.js?r=hash\"></script>", tag);

            Assert.AreEqual(1, javaScriptBundleFactory.FileWriterFactory.Files.Count);
            var output = TestUtilities.NormalizeLineEndings(javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath("script.js")]);
            Assert.True(output.StartsWith(javaScriptPreMinified));
            Assert.True(output.EndsWith(minifiedJavaScript2));
        }

        [Test]
        public void CanBundleJavaScriptWithMinifiedDirectories()
        {
            var path = Guid.NewGuid().ToString();
            var path2 = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.js");
            var file2 = TestUtilities.PrepareRelativePath(path2 + "\\file2.js");

            var resolver = new Mock<IResolver>(MockBehavior.Strict);
            resolver.Setup(r => r.IsDirectory(It.IsAny<string>())).Returns(true);

            resolver.Setup(r =>
                r.ResolveFolder(TestUtilities.PrepareRelativePath(path), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new[] { file1 });

            resolver.Setup(r =>
                r.ResolveFolder(TestUtilities.PrepareRelativePath(path2), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new[] { file2 });

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, resolver.Object))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, javaScript);
                frf.SetContentsForFile(file2, javaScriptPreMinified);

                var writerFactory = new StubFileWriterFactory();

                var tag = javaScriptBundleFactory
                    .WithDebuggingEnabled(false)
                    .WithFileReaderFactory(frf)
                    .WithFileWriterFactory(writerFactory)
                    .Create()
                    .AddDirectory(path)
                    .AddMinifiedDirectory(path2)
                    .Render("~/output.js");

                Assert.AreEqual("<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>", tag);

                var content = writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.js")];
                Assert.True(content.StartsWith(minifiedJavaScript));
                Assert.True(content.EndsWith(javaScriptPreMinified));
            }
        }

        [Test]
        public void CanBundleJsVaryingOutputBaseHrefRendersIndependentUrl()
        {
            var firstPath = "first.js";
            var secondPath = "second.js";

            javaScriptBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), javaScript);
            javaScriptBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), javaScript2);

            string tag = javaScriptBundleFactory
                            .Create()
                            .Add(firstPath)
                            .Add(secondPath)
                            .WithOutputBaseHref("http://subdomain.domain.com")
                            .Render("/js/output.js");

            string tagNoBaseHref = javaScriptBundleFactory
                            .Create()
                            .Add(firstPath)
                            .Add(secondPath)
                            .Render("/js/output.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://subdomain.domain.com/js/output.js?r=hash\"></script>", tag);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"/js/output.js?r=hash\"></script>", tagNoBaseHref);
        }

        [Test]
        public void RenderNamedUsesOutputBaseHref()
        {
            var firstPath = "first.js";
            var secondPath = "second.js";

            javaScriptBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(firstPath), javaScript);
            javaScriptBundleFactory.FileReaderFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(secondPath), javaScript2);

            javaScriptBundleFactory
                .Create()
                .Add(firstPath)
                .Add(secondPath)
                .WithOutputBaseHref("http://subdomain.domain.com")
                .AsNamed("leBundle", "/js/output.js");

            var tag = javaScriptBundleFactory
                            .Create()
                .WithOutputBaseHref("http://subdomain.domain.com")
                .RenderNamed("leBundle");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://subdomain.domain.com/js/output.js?r=hash\"></script>", tag);
        }

        [Test]
        public void CanBundleJavaScriptWithQuerystringParameter()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .Render("~/js/output_querystring.js?v=2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_querystring.js?v=2&r=hash\"></script>", tag);
        }

        [Test]
        public void CanBundleJavaScriptWithoutRevisionHash()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .WithoutRevisionHash()
                    .Render("~/js/output_querystring.js?v=2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_querystring.js?v=2\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .AsNamed("TestNamed", "~/js/output_namedbundle.js");

            var tag = javaScriptBundleFactory
                            .Create()
                            .RenderNamed("TestNamed");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_namedbundle.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_namedbundle.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithRemote()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .Render("~/js/output_1_2.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_1_2.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1_2.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithRemoteAndQuerystringParameter()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .Render("~/js/output_querystring.js?v=2_2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_querystring.js?v=2_2&r=hash\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithRemote()
        {
            javaScriptBundleFactory
                    .Create()
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .AsNamed("TestCdn", "~/js/output_3_2.js");

            var tag = javaScriptBundleFactory
                            .Create()
                            .RenderNamed("TestCdn");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_3_2.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_3_2.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithEmbeddedResource()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .Render("~/js/output_Embedded.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_Embedded.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_Embedded.js")]);
            Assert.AreEqual(1, javaScriptBundleFactory.FileWriterFactory.Files.Count);
        }

        [Test]
        public void CanBundleJavaScriptWithRootEmbeddedResource()
        {
            //this only tests that the resource can be resolved
            var tag = javaScriptBundleFactory
                    .WithDebuggingEnabled(false)
                    .Create()
                    .AddRootEmbeddedResource("~/js/test.js", "SquishIt.Tests://RootEmbedded.js")
                    .Render("~/js/output_Embedded.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_Embedded.js?r=hash\"></script>", tag);
            Assert.AreEqual(1, javaScriptBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_Embedded.js")]);
        }

        [Test]
        public void CanDebugBundleJavaScriptWithEmbeddedResource()
        {
            var tag = javaScriptBundleFactory
                    .WithDebuggingEnabled(true)
                    .Create()
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .Render("~/js/output_Embedded.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
            Assert.AreEqual(1, javaScriptBundleFactory.FileWriterFactory.Files.Count);
        }

        [Test]
        public void CanRenderDebugTags()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamed("TestWithDebug", "~/js/output_3.js");

            var tag = debugJavaScriptBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanRenderPreprocessedDebugTags()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            using(new ScriptPreprocessorScope<StubScriptPreprocessor>(new StubScriptPreprocessor()))
            {
                string tag = debugJavaScriptBundle
                    .Add("~/first.script.js")
                    .Render("output.js");

                Assert.AreEqual(
                    "<script type=\"text/javascript\" src=\"first.script.js.squishit.debug.js\"></script>\n",
                    TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void CanRenderDebugTagsTwice()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            var debugJavaScriptBundle2 = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

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

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag1));
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag2));
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamed("NamedWithDebug", "~/js/output_5.js");

            var tag = debugJavaScriptBundle.RenderNamed("NamedWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateBundleWithNullMinifier()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .WithMinifier<NullMinifier>()
                    .Render("~/js/output_6.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_6.js?r=hash\"></script>", tag);
            Assert.AreEqual(javaScript + ";\n", javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PreparePath(Environment.CurrentDirectory + @"\js\output_6.js")]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .WithMinifier<JsMinMinifier>()
                    .Render("~/js/output_7.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_7.js?r=hash\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;};\n", javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_7.js")]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMiniferByPassingInstance()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .WithMinifier(new JsMinMinifier())
                    .Render("~/js/output_jsmininstance.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_jsmininstance.js?r=hash\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;};\n", javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_jsmininstance.js")]);
        }

        [Test]
        public void CanCreateEmbeddedBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .WithMinifier<JsMinMinifier>()
                    .Render("~/js/output_embedded7.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_embedded7.js?r=hash\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;};\n", javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_embedded7.js")]);
        }

        [Test]
        public void CanRenderOnlyIfFileMissing()
        {
            javaScriptBundleFactory.FileReaderFactory.SetFileExists(false);

            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderOnlyIfOutputFileMissing()
                    .Render("~/js/output_9.js");

            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_9.js")]);

            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript2);
            javaScriptBundleFactory.FileReaderFactory.SetFileExists(true);
            javaScriptBundle.ClearCache();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderOnlyIfOutputFileMissing()
                    .Render("~/js/output_9.js");

            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_9.js")]);
        }

        [Test]
        public void CanRerenderFiles()
        {
            javaScriptBundleFactory.FileReaderFactory.SetFileExists(false);

            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            var javaScriptBundle2 = javaScriptBundleFactory
                .Create();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .Render("~/js/output_10.js");

            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_10.js")]);

            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript2);
            javaScriptBundleFactory.FileReaderFactory.SetFileExists(true);
            javaScriptBundleFactory.FileWriterFactory.Files.Clear();
            javaScriptBundle.ClearCache();

            javaScriptBundle2
                    .Add("~/js/test.js")
                    .Render("~/js/output_10.js");

            Assert.AreEqual("function sum(n,t){return n+t};\n", javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_10.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithHashInFilename()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .Render("~/js/output_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_hash.js\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_hash.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithUnderscoresInName()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test_file.js")
                    .Render("~/js/outputunder_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/outputunder_hash.js\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\outputunder_hash.js")]);
        }

        [Test]
        public void CanCreateNamedBundleWithForcedRelease()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .ForceRelease()
                    .AsNamed("ForceRelease", "~/js/output_forcerelease.js");

            var tag = javaScriptBundleFactory
                            .Create()
                            .RenderNamed("ForceRelease");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_forcerelease.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_forcerelease.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithSingleAttribute()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .WithAttribute("charset", "utf-8")
                    .Render("~/js/output_att.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/output_att.js?r=hash\"></script>", tag);
        }

        [Test]
        public void CanBundleJavaScriptWithSingleMultipleAttributes()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .Add("~/js/test.js")
                    .WithAttribute("charset", "utf-8")
                    .WithAttribute("other", "value")
                    .Render("~/js/output_att2.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" other=\"value\" src=\"js/output_att2.js?r=hash\"></script>", tag);
        }

        [Test]
        public void CanDebugBundleWithAttribute()
        {
            string tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .Add("~/js/test1.js")
                .Add("~/js/test2.js")
                .WithAttribute("charset", "utf-8")
                .Render("~/js/output_debugattr.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateCachedBundle()
        {
            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/js/output_2.js");

            var content = javaScriptBundle.RenderCached("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_2.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, content);
        }

        [Test]
        public void CanCreateCachedBundleAssetTag()
        {
            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/assets/js/main");

            var content = javaScriptBundle.RenderCached("Test");
            javaScriptBundle.ClearCache();
            var tag = javaScriptBundle.RenderCachedAssetTag("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"assets/js/main?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, content);
        }

        [Test]
        public void CanCreateCachedBundleAssetTag_When_Debugging()
        {
            var javaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/assets/js/main");

            var tag = javaScriptBundle.RenderCachedAssetTag("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateCachedBundleWithDebug()
        {
            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/js/output_2.js");
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateCachedBundleWithForceRelease()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            var tag1 = debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .ForceRelease()
                    .AsCached("Test", "~/assets/js/main");

            var content = debugJavaScriptBundle.RenderCached("Test");
            javaScriptBundle.ClearCache();
            var tag2 = debugJavaScriptBundle.RenderCachedAssetTag("Test");

            Assert.AreEqual(tag1, tag2);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"assets/js/main?r=hash\"></script>", tag1);
            Assert.AreEqual(minifiedJavaScript, content);
        }

        [Test]
        public void WithoutTypeAttribute()
        {
            var tag = javaScriptBundleFactory
                            .Create()
                    .Add("~/js/test.js")
                    .WithoutTypeAttribute()
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script src=\"js/output_1.js?r=hash\"></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PreparePath(Environment.CurrentDirectory + "\\" + path + "\\file1.js");
            var file2 = TestUtilities.PreparePath(Environment.CurrentDirectory + "\\" + path + "\\file2.js");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, javaScript2.Replace("sum", "replace"));
                frf.SetContentsForFile(file2, javaScript);

                var writerFactory = new StubFileWriterFactory();

                var tag = new JavaScriptBundleFactory()
                        .WithDebuggingEnabled(true)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.js");

                var expectedTag = string.Format("<script type=\"text/javascript\" src=\"{0}/file1.js\"></script>\n<script type=\"text/javascript\" src=\"{0}/file2.js\"></script>\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug_Ignores_Duplicates()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PreparePath(Environment.CurrentDirectory + "\\" + path + "\\file1.js");
            var file2 = TestUtilities.PreparePath(Environment.CurrentDirectory + "\\" + path + "\\file2.js");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, javaScript2.Replace("sum", "replace"));
                frf.SetContentsForFile(file2, javaScript);

                var writerFactory = new StubFileWriterFactory();

                var tag = new JavaScriptBundleFactory()
                        .WithDebuggingEnabled(true)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Add(file1)
                        .Render("~/output.js");

                var expectedTag = string.Format("<script type=\"text/javascript\" src=\"{0}/file1.js\"></script>\n<script type=\"text/javascript\" src=\"{0}/file2.js\"></script>\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug_Writes_And_Ignores_Preprocessed_Debug_Files()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.script.js");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file1.script.js.squishit.debug.js");
            var content = "some stuffs";

            var preprocessor = new StubScriptPreprocessor();

            using(new ScriptPreprocessorScope<StubScriptPreprocessor>(preprocessor))
            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, content);

                var writerFactory = new StubFileWriterFactory();

                var tag = new JavaScriptBundleFactory()
                        .WithDebuggingEnabled(true)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.js");

                var expectedTag = string.Format("<script type=\"text/javascript\" src=\"{0}/file1.script.js.squishit.debug.js\"></script>\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
                Assert.AreEqual(content, preprocessor.CalledWith);
                Assert.AreEqual(1, writerFactory.Files.Count);
                Assert.AreEqual("scripty", writerFactory.Files[file1 + ".squishit.debug.js"]);
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInRelease()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.js");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.js");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, javaScript2.Replace("sum", "replace"));
                frf.SetContentsForFile(file2, javaScript);

                var writerFactory = new StubFileWriterFactory();

                var tag = new JavaScriptBundleFactory()
                        .WithDebuggingEnabled(false)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Render("~/output.js");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hashy\"></script>";
                Assert.AreEqual(expectedTag, tag);

                var combined = "function replace(n,t){return n+t};\nfunction product(n,t){return n*t}function sum(n,t){return n+t};\n";
                Assert.AreEqual(combined, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.js")]);
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInRelease_Ignores_Duplicates()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.js");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.js");

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
            {
                var frf = new StubFileReaderFactory();
                frf.SetContentsForFile(file1, javaScript2.Replace("sum", "replace"));
                frf.SetContentsForFile(file2, javaScript);

                var writerFactory = new StubFileWriterFactory();

                var tag = new JavaScriptBundleFactory()
                        .WithDebuggingEnabled(false)
                        .WithFileReaderFactory(frf)
                        .WithFileWriterFactory(writerFactory)
                        .WithHasher(new StubHasher("hashy"))
                        .Create()
                        .Add(path)
                        .Add(file1)
                        .Render("~/output.js");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hashy\"></script>";
                Assert.AreEqual(expectedTag, tag);

                var combined = "function replace(n,t){return n+t};\nfunction product(n,t){return n*t}function sum(n,t){return n+t};\n";
                Assert.AreEqual(combined, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.js")]);
            }
        }

        [Test]
        public void CanRenderArbitraryStringsInDebug()
        {
            var js2Format = "{0}{1}";

            var subtract = "function sub(a,b){return a-b}";
            var divide = "function div(a,b){return a/b}";

            var tag = new JavaScriptBundleFactory()
                .WithDebuggingEnabled(true)
                .Create()
                .AddString(javaScript)
                .AddString(js2Format, new[] { subtract, divide })
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<script type=\"text/javascript\">{0}</script>\n<script type=\"text/javascript\">{1}</script>\n", javaScript, string.Format(js2Format, subtract, divide));
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanMaintainOrderBetweenArbitraryAndFileAssetsInRelease()
        {
            var file1 = "somefile.js";
            var file2 = "anotherfile.js";

            var subtract = "function sub(a,b){return a-b}";
            var minifiedSubtract = "function sub(n,t){return n-t};";

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file1), javaScript);
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file2), javaScript2);

            var writerFactory = new StubFileWriterFactory();

            var tag = new JavaScriptBundleFactory()
                .WithFileReaderFactory(readerFactory)
                .WithFileWriterFactory(writerFactory)
                .WithDebuggingEnabled(false)
                .Create()
                .Add(file1)
                .AddString(subtract)
                .Add(file2)
                .Render("test.js");

            var expectedTag = string.Format("<script type=\"text/javascript\" src=\"test.js?r=hash\"></script>");
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var combined = minifiedJavaScript + minifiedSubtract + "\n" + minifiedJavaScript2;
            Assert.AreEqual(combined, writerFactory.Files[TestUtilities.PrepareRelativePath(@"test.js")]);
        }

        [Test]
        public void CanMaintainOrderBetweenArbitraryAndFileAssetsInDebug()
        {
            var file1 = "somefile.js";
            var file2 = "anotherfile.js";

            var subtract = "function sub(a,b){return a-b}";

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file1), javaScript);
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(file2), javaScript2);

            var writerFactory = new StubFileWriterFactory();

            var tag = new JavaScriptBundleFactory()
                .WithFileReaderFactory(readerFactory)
                .WithFileWriterFactory(writerFactory)
                .WithDebuggingEnabled(true)
                .Create()
                .Add(file1)
                .AddString(subtract)
                .Add(file2)
                .Render("test.js");

            var expectedTag = string.Format("<script type=\"text/javascript\" src=\"somefile.js\"></script>\n<script type=\"text/javascript\">{0}</script>\n<script type=\"text/javascript\" src=\"anotherfile.js\"></script>\n"
                , subtract);
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanRenderArbitraryStringsInDebugAsCached()
        {
            var debugJavaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            var content = debugJavaScriptBundle
                .AddString(javaScript)
                .Add("~/js/test.js")
                .AsCached("Test3", "~/js/output_2.js");

            var cachedContent = debugJavaScriptBundle.RenderCached("Test3");

            debugJavaScriptBundle.ClearCache();

            var generatedContent = debugJavaScriptBundle.RenderCached("Test3");

            Assert.AreEqual(cachedContent, content);
            Assert.AreEqual(cachedContent, generatedContent);
        }

        [Test]
        public void CanRenderArbitraryStringsInDebugWithoutType()
        {
            var js2Format = "{0}{1}";

            var subtract = "function sub(a,b){return a-b}";
            var divide = "function div(a,b){return a/b}";

            var tag = new JavaScriptBundleFactory()
                .WithDebuggingEnabled(true)
                .Create()
                .AddString(javaScript)
                .AddString(js2Format, new[] { subtract, divide })
                .WithoutTypeAttribute()
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<script>{0}</script>\n<script>{1}</script>\n", javaScript, string.Format(js2Format, subtract, divide));
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void DoesNotRenderDuplicateArbitraryStringsInDebug()
        {
            var tag = new JavaScriptBundleFactory()
                .WithDebuggingEnabled(true)
                .Create()
                .AddString(javaScript)
                .AddString(javaScript)
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<script type=\"text/javascript\">{0}</script>\n", javaScript);
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanBundleArbitraryContentsInRelease()
        {
            var js2Format = "{0}{1}";

            var subtract = "function sub(a,b){return a-b}";
            var divide = "function div(a,b){return a/b}";

            var writerFactory = new StubFileWriterFactory();

            var tag = new JavaScriptBundleFactory()
                    .WithDebuggingEnabled(false)
                    .WithFileWriterFactory(writerFactory)
                    .WithHasher(new StubHasher("hashy"))
                    .Create()
                    .AddString(javaScript)
                    .AddString(js2Format, new[] { subtract, divide })
                    .Render("~/output.js");

            var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hashy\"></script>";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var minifiedScript = "function product(n,t){return n*t}function sum(n,t){return n+t};\nfunction sub(n,t){return n-t}function div(n,t){return n/t};\n";
            Assert.AreEqual(minifiedScript, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithDeferredLoad()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .WithDeferredLoad()
                    .Add("~/js/test.js")
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=hash\" defer></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithDeferredLoadLast()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .WithAsyncLoad()
                    .WithDeferredLoad()
                    .Add("~/js/test.js")
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=hash\" defer></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithAsyncLoad()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .WithAsyncLoad()
                    .Add("~/js/test.js")
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=hash\" async></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithAsyncLoadLast()
        {
            var tag = javaScriptBundleFactory
                    .Create()
                    .WithDeferredLoad()
                    .WithAsyncLoad()
                    .Add("~/js/test.js")
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=hash\" async></script>", tag);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanUseArbitraryReleaseFileRenderer()
        {
            var renderer = new Mock<IRenderer>();

            var content = "content";

            javaScriptBundleFactory
                .Create()
                .WithReleaseFileRenderer(renderer.Object)
                .AddString(content)
                .ForceRelease()
                .Render("test.js");

            renderer.Verify(r => r.Render(content + ";\n", TestUtilities.PrepareRelativePath("test.js")));
        }

        [Test]
        public void CanIgnoreArbitraryReleaseFileRendererIfDebugging()
        {
            var renderer = new Mock<IRenderer>(MockBehavior.Strict);

            var content = "content";

            javaScriptBundleFactory
                .Create()
                .WithReleaseFileRenderer(renderer.Object)
                .AddString(content)
                .ForceDebug()
                .Render("test.js");

            renderer.VerifyAll();
        }

        [Test]
        public void CanIgnoreArbitraryReleaseRendererInDebug()
        {
            var renderer = new Mock<IRenderer>();

            var content = "content";

            javaScriptBundleFactory
                .Create()
                .WithReleaseFileRenderer(renderer.Object)
                .AddString(content)
                .Render("test.js");

            renderer.VerifyAll();
        }

        [Test]
        public void CanIncludeDynamicContentInDebug()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(r => r.Url).Returns(new Uri("http://example.com"));
            context.SetupGet(c => c.Request).Returns(request.Object);

            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            using(new HttpContextScope(context.Object))
            {
                javaScriptBundle
                    .ForceDebug()
                    .AddDynamic("/some/dynamic/js");
            }

            var tag = javaScriptBundle
                .Render("~/combined_#.js");

            Assert.AreEqual(0, javaScriptBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"/some/dynamic/js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanIncludeDynamicContentInRelease()
        {
            //this doesn't really test the nitty-gritty details (http resolver, download etc...) but its a start
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(r => r.Url).Returns(new Uri("http://example.com"));
            context.SetupGet(c => c.Request).Returns(request.Object);

            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            using(new HttpContextScope(context.Object))
            {
                //some/dynamic/js started returning 404s
                javaScriptBundle
                    .ForceRelease()
                    .AddDynamic("/");
            }

            var tag = javaScriptBundle
                .Render("~/combined.js");

            Assert.AreEqual(1, javaScriptBundleFactory.FileWriterFactory.Files.Count);
            Assert.AreEqual(minifiedJavaScript, javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath("combined.js")]);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"combined.js?r=hash\"></script>", tag);
        }

        [Test]
        public void RenderRelease_OmitsRenderedTag_IfOnlyRemoteAssets()
        {
            //this is rendering tag correctly but incorrectly(?) merging both files
            using(new ResolverFactoryScope(typeof(HttpResolver).FullName, StubResolver.ForFile("http://www.someurl.com/css/first.css")))
            {
                string tag = javaScriptBundleFactory
                    .Create()
                    .ForceRelease()
                    .AddRemote("/css/first.js", "http://www.someurl.com/js/first.js")
                    .Render("/css/output_remote.js");

                Assert.AreEqual("<script type=\"text/javascript\" src=\"http://www.someurl.com/js/first.js\"></script>", tag);
                Assert.AreEqual(0, javaScriptBundleFactory.FileWriterFactory.Files.Count);
            }
        }

        [Test]
        public void CanRenderDistinctBundlesIfSameOutputButDifferentFileNames()
        {
            var hasher = new Hasher(new RetryableFileOpener());

            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript);

            var tag = javaScriptBundleFactory
                        .WithHasher(hasher)
                        .Create()
                        .Add("~/js/test.js")
                        .Render("~/js/output#.js");

            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript2);

            var tag2 = javaScriptBundleFactory
                        .WithHasher(hasher)
                        .Create()
                        .Add("~/js/test2.js")
                        .Render("~/js/output#.js");

            Assert.AreNotEqual(tag, tag2);
        }

        [Test]
        public void CanRenderDistinctBundlesIfSameOutputButDifferentArbitrary()
        {
            var hasher = new Hasher(new RetryableFileOpener());

            var tag = javaScriptBundleFactory
                    .WithHasher(hasher)
                    .Create()
                    .AddString(javaScript)
                    .Render("~/js/output#.js");

            var tag2 = javaScriptBundleFactory
                    .WithHasher(hasher)
                    .Create()
                    .AddString(javaScript2)
                    .Render("~/js/output#.js");

            Assert.AreNotEqual(tag, tag2);
        }

        [Test]
        public void ForceDebugIf()
        {
            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript);

            var file1 = "test.js";
            var file2 = "anothertest.js";
            Func<bool> queryStringPredicate = () => HttpContext.Current.Request.QueryString.AllKeys.Contains("debug") && HttpContext.Current.Request.QueryString["debug"] == "true";

            var nonDebugContext = new Mock<HttpContextBase>();
            nonDebugContext.Setup(hcb => hcb.Request.QueryString).Returns(new NameValueCollection());
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/" + file1)).Returns(Path.Combine(Environment.CurrentDirectory, file1));
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/" + file2)).Returns(Path.Combine(Environment.CurrentDirectory, file2));
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/output.js")).Returns(Path.Combine(Environment.CurrentDirectory, "output.js"));

            JavaScriptBundle bundle;

            using(new HttpContextScope(nonDebugContext.Object))
            {
                bundle = javaScriptBundleFactory
                            .WithDebuggingEnabled(false)
                            .Create()
                            .Add(file1)
                            .Add(file2)
                            .ForceDebugIf(queryStringPredicate);

                var tag = bundle.Render("~/output.js");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }

            var debugContext = new Mock<HttpContextBase>();
            debugContext.Setup(hcb => hcb.Request.QueryString).Returns(new NameValueCollection { { "debug", "true" } });
            debugContext.Setup(hcb => hcb.Server.MapPath("/" + file1)).Returns(Path.Combine(Environment.CurrentDirectory, file1));
            debugContext.Setup(hcb => hcb.Server.MapPath("/" + file2)).Returns(Path.Combine(Environment.CurrentDirectory, file2));
            debugContext.Setup(hcb => hcb.Server.MapPath("/output.js")).Returns(Path.Combine(Environment.CurrentDirectory, "output.js"));

            using(new HttpContextScope(debugContext.Object))
            {
                var tag = bundle.Render("~/output.js");

                var expectedTag = "<script type=\"text/javascript\" src=\"test.js\"></script>\n<script type=\"text/javascript\" src=\"anothertest.js\"></script>\n";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }

            using(new HttpContextScope(nonDebugContext.Object))
            {
                var tag = bundle.Render("~/output.js");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void ForceDebugIf_Named()
        {
            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript);

            var file1 = "test.js";
            var file2 = "anothertest.js";
            Func<bool> queryStringPredicate = () => HttpContext.Current.Request.QueryString.AllKeys.Contains("debug") && HttpContext.Current.Request.QueryString["debug"] == "true";

            var debugContext = new Mock<HttpContextBase>();
            debugContext.Setup(hcb => hcb.Request.QueryString).Returns(new NameValueCollection { { "debug", "true" } });
            debugContext.Setup(hcb => hcb.Server.MapPath("/" + file1)).Returns(Path.Combine(Environment.CurrentDirectory, file1));
            debugContext.Setup(hcb => hcb.Server.MapPath("/" + file2)).Returns(Path.Combine(Environment.CurrentDirectory, file2));
            debugContext.Setup(hcb => hcb.Server.MapPath("/output.js")).Returns(Path.Combine(Environment.CurrentDirectory, "output.js"));

            var nonDebugContext = new Mock<HttpContextBase>();
            nonDebugContext.Setup(hcb => hcb.Request.QueryString).Returns(new NameValueCollection());
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/" + file1)).Returns(Path.Combine(Environment.CurrentDirectory, file1));
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/" + file2)).Returns(Path.Combine(Environment.CurrentDirectory, file2));
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/output.js")).Returns(Path.Combine(Environment.CurrentDirectory, "output.js"));

            using(new HttpContextScope(nonDebugContext.Object))
            {
                javaScriptBundleFactory
                    .WithDebuggingEnabled(false)
                    .Create()
                    .Add(file1)
                    .Add(file2)
                    .ForceDebugIf(queryStringPredicate)
                    .AsNamed("test", "~/output.js");

                var tag = javaScriptBundleFactory
                    .Create()
                    .RenderNamed("test");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }

            using(new HttpContextScope(debugContext.Object))
            {
                var tag = javaScriptBundleFactory
                    .Create()
                    .RenderNamed("test");

                var expectedTag = "<script type=\"text/javascript\" src=\"test.js\"></script>\n<script type=\"text/javascript\" src=\"anothertest.js\"></script>\n";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }

            using(new HttpContextScope(nonDebugContext.Object))
            {
                var tag = javaScriptBundleFactory
                    .Create()
                    .RenderNamed("test");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void ForceDebugIf_Cached()
        {
            javaScriptBundleFactory.FileReaderFactory.SetContents(javaScript);

            var file1 = "test.js";
            var file2 = "anothertest.js";
            Func<bool> queryStringPredicate = () => HttpContext.Current.Request.QueryString.AllKeys.Contains("debug") && HttpContext.Current.Request.QueryString["debug"] == "true";

            var debugContext = new Mock<HttpContextBase>();
            debugContext.Setup(hcb => hcb.Request.QueryString).Returns(new NameValueCollection { { "debug", "true" } });
            debugContext.Setup(hcb => hcb.Server.MapPath("/" + file1)).Returns(Path.Combine(Environment.CurrentDirectory, file1));
            debugContext.Setup(hcb => hcb.Server.MapPath("/" + file2)).Returns(Path.Combine(Environment.CurrentDirectory, file2));
            debugContext.Setup(hcb => hcb.Server.MapPath("/output.js")).Returns(Path.Combine(Environment.CurrentDirectory, "output.js"));

            var nonDebugContext = new Mock<HttpContextBase>();
            nonDebugContext.Setup(hcb => hcb.Request.QueryString).Returns(new NameValueCollection());
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/" + file1)).Returns(Path.Combine(Environment.CurrentDirectory, file1));
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/" + file2)).Returns(Path.Combine(Environment.CurrentDirectory, file2));
            nonDebugContext.Setup(hcb => hcb.Server.MapPath("/output.js")).Returns(Path.Combine(Environment.CurrentDirectory, "output.js"));

            using(new HttpContextScope(nonDebugContext.Object))
            {
                javaScriptBundleFactory
                    .WithDebuggingEnabled(false)
                    .Create()
                    .Add(file1)
                    .Add(file2)
                    .ForceDebugIf(queryStringPredicate)
                    .AsCached("test", "~/output.js");

                var tag = javaScriptBundleFactory
                    .Create()
                    .RenderNamed("test");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }

            using(new HttpContextScope(debugContext.Object))
            {
                var tag = javaScriptBundleFactory
                    .Create()
                    .RenderCachedAssetTag("test");

                var expectedTag = "<script type=\"text/javascript\" src=\"test.js\"></script>\n<script type=\"text/javascript\" src=\"anothertest.js\"></script>\n";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }

            using(new HttpContextScope(nonDebugContext.Object))
            {
                var tag = javaScriptBundleFactory
                    .Create()
                    .RenderCachedAssetTag("test");

                var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hash\"></script>";

                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void Includes_Semicolon_And_Newline_Between_Scripts()
        {
            var writerFactory = new StubFileWriterFactory();

            var tag = new JavaScriptBundleFactory()
                    .WithDebuggingEnabled(false)
                    .WithFileWriterFactory(writerFactory)
                    .WithHasher(new StubHasher("hashy"))
                    .Create()
                    .AddString("first")
                    .AddString("second")
                    .Render("~/output.js");

            var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hashy\"></script>";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var minifiedScript = "first;\nsecond;\n";
            Assert.AreEqual(minifiedScript, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.js")]);
        }

        [Test]
        public void CanRenderContentOnly_Release()
        {
            var javaScriptBundle = javaScriptBundleFactory
                .Create();

            var content = javaScriptBundle
                    .Add("~/js/test.js").RenderRawContent("testrelease");

            Assert.AreEqual(minifiedJavaScript, content);


            Assert.AreEqual(content, javaScriptBundleFactory.Create().RenderCachedRawContent("testrelease"));
        }

        [Test]
        public void CanRenderContentOnly_Debug()
        {
            var javaScriptBundle = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create();

            var content = javaScriptBundle
                    .Add("~/js/test.js").RenderRawContent("testdebug");

            Assert.AreEqual(javaScript + ";\n", content);

            Assert.AreEqual(content, javaScriptBundleFactory.Create().RenderCachedRawContent("testdebug"));
        }
    }
}