using System;
using System.Text;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.Resolvers;
using SquishIt.Hogan;
using SquishIt.Hogan.Hogan;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;
using Version = SquishIt.Framework.Version;

namespace SquishIt.Tests
{
    public class HoganTests
    {
        JavaScriptBundleFactory javaScriptBundleFactory;

        [SetUp]
        public void Setup()
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory();
        }

        [TestCase(typeof(HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(MsIeHogan.HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CanBundleJavascriptInDebug(Type preprocessorType)
        {
            var preprocessor = Activator.CreateInstance(preprocessorType) as IPreprocessor;

            const string template = "<h1>{{message}}</h1>";
            var templateFileName = "test.hogan.html";
            var resolver = StubResolver.ForFile(TestUtilities.PrepareRelativePath(templateFileName));

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(templateFileName), template);

            var writerFactory = new StubFileWriterFactory();

            string tag;

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, resolver))
            {
                tag = javaScriptBundleFactory
                    .WithFileReaderFactory(readerFactory)
                    .WithFileWriterFactory(writerFactory)
                    .WithDebuggingEnabled(true)
                    .Create()
                    .WithPreprocessor(preprocessor)
                    .Add("~/" + templateFileName)
                    .Render("~/template.js");
            }

            var sb = new StringBuilder();
            sb.AppendLine(@"var JST = JST || {};");
            sb.AppendLine(@"JST['test'] = new Hogan.Template(function(c,p,i){var _=this;_.b(i=i||"""");_.b(""<h1>"");_.b(_.v(_.f(""message"",c,p,0)));_.b(""</h1>"");return _.fl();;},""" + template + "\",Hogan,{});");
            var compiled = sb.ToString();

            Assert.AreEqual(1, writerFactory.Files.Count);
            var expectedTag = "<script type=\"text/javascript\" src=\"test.hogan.html.squishit.debug.js\"></script>\n";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            Assert.AreEqual(compiled, writerFactory.Files[TestUtilities.PrepareRelativePath("test.hogan.html.squishit.debug.js")]);
        }

        [TestCase(typeof(HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(MsIeHogan.HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CanBundleJavascriptInDebugWithArbitraryHogan(Type preprocessorType)
        {
            var preprocessor = Activator.CreateInstance(preprocessorType) as IPreprocessor;

            const string template = "<h1>{{message}}</h1>";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithPreprocessor(preprocessor)
                .AddString(template, ".hogan.html")
                .Render("~/template.js");

            var sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"">var JST = JST || {};");
            sb.AppendLine(@"JST['dummy'] = new Hogan.Template(function(c,p,i){var _=this;_.b(i=i||"""");_.b(""<h1>"");_.b(_.v(_.f(""message"",c,p,0)));_.b(""</h1>"");return _.fl();;},""" + template + "\",Hogan,{});");
            sb.AppendLine("</script>");
            Assert.AreEqual(sb.ToString(), tag);
        }

        [TestCase(typeof(HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(MsIeHogan.HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CanBundleJavascriptInRelease(Type preprocessorType)
        {
            var preprocessor = Activator.CreateInstance(preprocessorType) as IPreprocessor;

            const string template = "<h1>{{message}}</h1>";
            var templateFileName = "test.hogan.html";
            var resolver = StubResolver.ForFile(TestUtilities.PrepareRelativePath(templateFileName));

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContentsForFile(TestUtilities.PrepareRelativePath(templateFileName), template);

            var writerFactory = new StubFileWriterFactory();

            string tag;

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, resolver))
            {
                tag = javaScriptBundleFactory
                    .WithFileReaderFactory(readerFactory)
                    .WithFileWriterFactory(writerFactory)
                    .WithDebuggingEnabled(false)
                    .Create()
                    .WithPreprocessor(preprocessor)
                    .Add("~/" + templateFileName)
                    .Render("~/template.js");
            }

            //are minifier's optimizations here OK?
            var compiled =
                @"var JST=JST||{};JST.test=new Hogan.Template(function(n,t,i){var r=this;return r.b(i=i||""""),r.b(""<h1>""),r.b(r.v(r.f(""message"",n,t,0))),r.b(""</h1>""),r.fl()},""" + template + "\",Hogan,{});";

            Assert.AreEqual(1, writerFactory.Files.Count);
            var expectedTag = "<script type=\"text/javascript\" src=\"template.js?r=hash\"></script>";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var actual = writerFactory.Files[TestUtilities.PrepareRelativePath("template.js")];
            Assert.AreEqual(compiled + "\n", actual);
        }

        [TestCase(typeof(HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(MsIeHogan.HoganPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CanBundleJavascriptInReleaseWithArbitraryHogan(Type preprocessorType)
        {
            var preprocessor = Activator.CreateInstance(preprocessorType) as IPreprocessor;

            const string template = "<h1>{{message}}</h1>";

            var writerFactory = new StubFileWriterFactory();

            string tag;

            tag = javaScriptBundleFactory
                    .WithFileWriterFactory(writerFactory)
                    .WithDebuggingEnabled(false)
                    .Create()
                    .WithPreprocessor(preprocessor)
                    .AddString(template, ".hogan.html")
                    .Render("~/template.js");

            //are minifier's optimizations here OK?
            var compiled = @"var JST=JST||{};JST.dummy=new Hogan.Template(";

            Assert.AreEqual(1, writerFactory.Files.Count);
            var expectedTag = "<script type=\"text/javascript\" src=\"template.js?r=hash\"></script>";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            var actual = writerFactory.Files[TestUtilities.PrepareRelativePath("template.js")];
            Assert.IsTrue(actual.StartsWith(compiled));
        }

        [TestCase(typeof(HoganCompiler)), Platform(Include = "Unix, Linux, Mono")]
		[TestCase(typeof(MsIeHogan.Hogan.HoganCompiler)), Platform(Include = "Unix, Linux, Mono")]
		public void CompileFailsGracefullyOnMono (Type compilerType)
		{
			var compiler = Activator.CreateInstance (compilerType);
			var method = compilerType.GetMethod ("Compile");

			string message;
			if (Platform.Mono && Platform.MonoVersion >= new Version("2.10.8")) 
			{
				var exception = Assert.Throws<System.Reflection.TargetInvocationException>(() => method.Invoke (compiler, new[] { "" }));
				message = exception.InnerException.Message;
			} 
			else 
			{
				var exception = Assert.Throws<Exception>(() => method.Invoke (compiler, new[] { "" }));
				message = exception.Message;
			}
            Assert.AreEqual("Hogan not yet supported for mono.", message);
        }
    }
}
