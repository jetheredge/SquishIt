using System;
using System.IO;
using NUnit.Framework;
using SquishIt.Framework.Files;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Stubs;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class JavaScriptBundleTests
    {
        private string javaScript = TestUtilities.NormalizeLineEndings(@"
																				function product(a, b)
																				{
																						return a * b;
																				}

																				function sum(a, b){
																						return a + b;
																				}");

        private string javaScript2 = TestUtilities.NormalizeLineEndings(@"function sum(a, b){
																						return a + b;
																			 }");

        private JavaScriptBundle javaScriptBundle;
        private JavaScriptBundle javaScriptBundle2;
        private JavaScriptBundle debugJavaScriptBundle;
        private JavaScriptBundle debugJavaScriptBundle2;
        private StubFileWriterFactory fileWriterFactory;
        private StubFileReaderFactory fileReaderFactory;
        private StubCurrentDirectoryWrapper currentDirectoryWrapper;
        private IHasher hasher;
        private StubBundleCache stubBundleCache;

        [SetUp]
        public void Setup()
        {
            var nonDebugStatusReader = new StubDebugStatusReader(false);
            var debugStatusReader = new StubDebugStatusReader(true);
            fileWriterFactory = new StubFileWriterFactory();
            fileReaderFactory = new StubFileReaderFactory();
            currentDirectoryWrapper = new StubCurrentDirectoryWrapper();
            stubBundleCache = new StubBundleCache();

            var retryableFileOpener = new RetryableFileOpener();
            hasher = new Hasher(retryableFileOpener);

            fileReaderFactory.SetContents(javaScript);

            javaScriptBundle = new JavaScriptBundle(nonDebugStatusReader,
                                                        fileWriterFactory,
                                                        fileReaderFactory,
                                                        currentDirectoryWrapper,
                                                        hasher,
                                                        stubBundleCache);

            javaScriptBundle2 = new JavaScriptBundle(nonDebugStatusReader,
                                                        fileWriterFactory,
                                                        fileReaderFactory,
                                                        currentDirectoryWrapper,
                                                        hasher,
                                                        stubBundleCache);

            debugJavaScriptBundle = new JavaScriptBundle(debugStatusReader,
                                                        fileWriterFactory,
                                                        fileReaderFactory,
                                                        currentDirectoryWrapper,
                                                        hasher,
                                                        stubBundleCache);

            debugJavaScriptBundle2 = new JavaScriptBundle(debugStatusReader,
                                                        fileWriterFactory,
                                                        fileReaderFactory,
                                                        currentDirectoryWrapper,
                                                        hasher,
                                                        stubBundleCache);
        }

        [Test]
        public void CanBundleJavaScript()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath (@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithQuerystringParameter()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .Render("~/js/output_querystring.js?v=2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_querystring.js?v=2&r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsNamed("TestNamed", "~/js/output_namedbundle.js");

            var tag = javaScriptBundle.RenderNamed("TestNamed");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_namedbundle.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_namedbundle.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithRemote()
        {
            var tag = javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .Render("~/js/output_1_2.js");

            Assert.AreEqual ("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_1_2.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1_2.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithRemoteAndQuerystringParameter()
        {
            var tag = javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .Render("~/js/output_querystring.js?v=2_2");

            Assert.AreEqual ("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_querystring.js?v=2_2&r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithRemote()
        {
            javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .AsNamed("TestCdn", "~/js/output_3_2.js");

            var tag = javaScriptBundle.RenderNamed("TestCdn");

            Assert.AreEqual ("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_3_2.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_3_2.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithEmbeddedResource()
        {
            var tag = javaScriptBundle
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .Render("~/js/output_Embedded.js");

            Assert.AreEqual ("<script type=\"text/javascript\" src=\"js/output_Embedded.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_Embedded.js")]);
        }

        [Test]
        public void CanDebugBundleJavaScriptWithEmbeddedResource()
        {
            var tag = debugJavaScriptBundle
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .Render("~/js/output_Embedded.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanRenderDebugTags()
        {
            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamed("TestWithDebug", "~/js/output_3.js");

            var tag = debugJavaScriptBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
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

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag1));
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag2));
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamed("NamedWithDebug", "~/js/output_5.js");

            var tag = debugJavaScriptBundle.RenderNamed("NamedWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateBundleWithNullMinifer()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithMinifier<NullMinifier>()
                    .Render("~/js/output_6.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_6.js?r=1C5788F076B8F8FB10AF9A76E7B822CB\"></script>", tag);
            Assert.AreEqual(javaScript + "\n", fileWriterFactory.Files[TestUtilities.PreparePath(Environment.CurrentDirectory + @"\js\output_6.js")]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithMinifier<JsMinMinifier>()
                    .Render("~/js/output_7.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_7.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual ("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_7.js")]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMiniferByPassingInstance()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithMinifier(new JsMinMinifier())
                    .Render("~/js/output_jsmininstance.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_jsmininstance.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual ("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_jsmininstance.js")]);
        }

        [Test]
        public void CanCreateEmbeddedBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundle
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .WithMinifier<JsMinMinifier>()
                    .Render("~/js/output_embedded7.js");

            Assert.AreEqual ("<script type=\"text/javascript\" src=\"js/output_embedded7.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual ("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_embedded7.js")]);
        }

        /*[Test]
        public void CanCreateBundleWithClosureMinifer()
        {
                var tag = javaScriptBundle
                        .Add("~/js/test.js")
                        .WithMinifier(JavaScriptMinifiers.Closure)
                        .Render("~/js/output_8.js");

                Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_8.js?r=00DFDFFC4078EFF6DFCC6244EAB77420\"></script>", tag);
                Assert.AreEqual("function product(n,t){return n*t}function sum(n,t){return n+t};\r\n", fileWriterFactory.Files[@"C:\js\output_8.js"]);
        }*/

        [Test]
        public void CanRenderOnlyIfFileMissing()
        {
            fileReaderFactory.SetFileExists(false);

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderOnlyIfOutputFileMissing()
                    .Render("~/js/output_9.js");

            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_9.js")]);

            fileReaderFactory.SetContents(javaScript2);
            fileReaderFactory.SetFileExists(true);
            javaScriptBundle.ClearCache();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderOnlyIfOutputFileMissing()
                    .Render("~/js/output_9.js");

            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_9.js")]);
        }

        [Test]
        public void CanRerenderFiles()
        {
            fileReaderFactory.SetFileExists(false);

            javaScriptBundle
                    .Add("~/js/test.js")
                    .Render("~/js/output_10.js");

            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_10.js")]);

            fileReaderFactory.SetContents(javaScript2);
            fileReaderFactory.SetFileExists(true);
            fileWriterFactory.Files.Clear();
            javaScriptBundle.ClearCache();

            javaScriptBundle2
                    .Add("~/js/test.js")
                    .Render("~/js/output_10.js");

            Assert.AreEqual ("function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_10.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithHashInFilename()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .Render("~/js/output_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_36286D0CEA57C5ED24B868EB0D2898E9.js\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_36286D0CEA57C5ED24B868EB0D2898E9.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithUnderscoresInName()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test_file.js")
                    .Render("~/js/outputunder_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/outputunder_36286D0CEA57C5ED24B868EB0D2898E9.js\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\outputunder_36286D0CEA57C5ED24B868EB0D2898E9.js")]);
        }

        [Test]
        public void CanCreateNamedBundleWithForcedRelease()
        {
            debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .ForceRelease()
                    .AsNamed("ForceRelease", "~/js/output_forcerelease.js");

            var tag = javaScriptBundle.RenderNamed("ForceRelease");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_forcerelease.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_forcerelease.js")]);
        }

        [Test]
        public void CanBundleJavaScriptWithSingleAttribute()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithAttribute("charset", "utf-8")
                    .Render("~/js/output_att.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/output_att.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
        }

        [Test]
        public void CanBundleJavaScriptWithSingleMultipleAttributes()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithAttribute("charset", "utf-8")
                    .WithAttribute("other", "value")
                    .Render("~/js/output_att2.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" other=\"value\" src=\"js/output_att2.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
        }

        [Test]
        public void CanDebugBundleWithAttribute()
        {
            string tag = debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .WithAttribute("charset", "utf-8")
                    .Render("~/js/output_debugattr.js");
            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/test1.js\"></script>\n<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/test2.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateCachedBundle()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/js/output_2.js");

            var content = javaScriptBundle.RenderCached("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_2.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual("function product(n,t){return n*t}function sum(n,t){return n+t}", content);
        }

        [Test]
        public void CanCreateCachedBundleAssetTag()
        {
            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/assets/js/main");

            var content = javaScriptBundle.RenderCached("Test");
            javaScriptBundle.ClearCache();
            var tag = javaScriptBundle.RenderCachedAssetTag("Test");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"assets/js/main?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual("function product(n,t){return n*t}function sum(n,t){return n+t}", content);
        }

        [Test]
        public void CanCreateCachedBundleWithDebug()
        {
            var tag = debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .AsCached("Test", "~/js/output_2.js");
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>\n", TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanCreateCachedBundleWithForceRelease()
        {
            var tag1 = debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .ForceRelease()
                    .AsCached("Test", "~/assets/js/main");

            var content = debugJavaScriptBundle.RenderCached("Test");
            javaScriptBundle.ClearCache();
            var tag2 = debugJavaScriptBundle.RenderCachedAssetTag("Test");

            Assert.AreEqual(tag1, tag2);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"assets/js/main?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag1);
            Assert.AreEqual("function product(n,t){return n*t}function sum(n,t){return n+t}", content);
        }

        [Test]
        public void WithoutTypeAttribute()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithoutTypeAttribute()
                    .Render("~/js/output_1.js");

            Assert.AreEqual("<script src=\"js/output_1.js?r=36286D0CEA57C5ED24B868EB0D2898E9\"></script>", tag);
            Assert.AreEqual ("function product(n,t){return n*t}function sum(n,t){return n+t}", fileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"js\output_1.js")]);
        }

        [Test]
        public void CanBundleDirectoryContentsInDebug()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PreparePath (Environment.CurrentDirectory + "\\" + path + "\\file1.js");
            var file2 = TestUtilities.PreparePath(Environment.CurrentDirectory + "\\" + path + "\\file2.js");

            using (new ResolverFactoryScope(typeof(SquishIt.Framework.Resolvers.FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
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

                var expectedTag = string.Format("<script type=\"text/javascript\" src=\"/{0}/file1.js\"></script>\n<script type=\"text/javascript\" src=\"/{0}/file2.js\"></script>\n", path);
                Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
            }
        }

        [Test]
        public void CanBundleDirectoryContentsInRelease()
        {
            var path = Guid.NewGuid().ToString();
            var file1 = TestUtilities.PrepareRelativePath(path + "\\file1.js");
            var file2 = TestUtilities.PrepareRelativePath(path + "\\file2.js");

            using (new ResolverFactoryScope(typeof(SquishIt.Framework.Resolvers.FileSystemResolver).FullName, StubResolver.ForDirectory(new[] { file1, file2 })))
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

                var combined = "function replace(n,t){return n+t}function product(n,t){return n*t}function sum(n,t){return n+t}";
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
                .AddString(js2Format, subtract, divide)
                .Render("doesn't matter where...");

            var expectedTag = string.Format("<script type=\"text/javascript\">{0}</script>\n<script type=\"text/javascript\">{1}</script>\n", javaScript, string.Format(js2Format, subtract, divide));
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));
        }

        [Test]
        public void CanRenderArbitraryStringsInDebugWithoutType () 
        {
            var js2Format = "{0}{1}";

            var subtract = "function sub(a,b){return a-b}";
            var divide = "function div(a,b){return a/b}";

            var tag = new JavaScriptBundleFactory ()
                .WithDebuggingEnabled (true)
                .Create ()
                .AddString (javaScript)
                .AddString (js2Format, subtract, divide)
                .WithoutTypeAttribute ()
                .Render ("doesn't matter where...");

            var expectedTag = string.Format ("<script>{0}</script>\n<script>{1}</script>\n", javaScript, string.Format (js2Format, subtract, divide));
            Assert.AreEqual (expectedTag, TestUtilities.NormalizeLineEndings (tag));
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
                    .AddString(js2Format, subtract, divide)
                    .Render("~/output.js");

            var expectedTag = "<script type=\"text/javascript\" src=\"output.js?r=hashy\"></script>";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var minifiedScript = "function product(n,t){return n*t}function sum(n,t){return n+t}function sub(n,t){return n-t}function div(n,t){return n/t}";
            Assert.AreEqual(minifiedScript, writerFactory.Files[TestUtilities.PrepareRelativePath(@"output.js")]);
        }
    }
}