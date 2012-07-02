using System.Text;
using NUnit.Framework;
using SquishIt.Framework.Resolvers;
using SquishIt.Hogan;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture, Platform(Exclude = "Unix, Linux, Mono")]
    public class HoganTests
    {
        JavaScriptBundleFactory javaScriptBundleFactory;

        [SetUp]
        public void Setup()
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory();
        }

        [Test]
        public void CanBundleJavascriptInDebug()
        {
            const string template = "<h1>{{message}}</h1>";
            var templateFileName = "test.html";
            var resolver = StubResolver.ForFile(templateFileName);

            var readerFactory = new StubFileReaderFactory();
            readerFactory.SetContents(template);

            var writerFactory = new StubFileWriterFactory();

            string tag;

            using(new ResolverFactoryScope(typeof(FileSystemResolver).FullName, resolver))
            {
                tag = javaScriptBundleFactory
                    .WithFileReaderFactory(readerFactory)
                    .WithFileWriterFactory(writerFactory)
                    .WithDebuggingEnabled(true)
                    .Create()
                    .WithPreprocessor(new HoganPreprocessor())
                    .Add(templateFileName)
                    .Render("~/template.js");
            }

            var sb = new StringBuilder();
            sb.Append(@"var JST = JST || {};");
            sb.Append(@"JST['dummy'] = new Hogan.Template(function(c,p,i){var _=this;_.b(i=i||"""");_.b(""<h1>"");_.b(_.v(_.f(""message"",c,p,0)));_.b(""</h1>"");return _.fl();;});");
            Assert.AreEqual(1, writerFactory.Files.Count);
            var expectedTag = "<script type=\"text/javascript\" src=\"test.html.squishit.debug.js\"></script>\n";
            Assert.AreEqual(expectedTag, TestUtilities.NormalizeLineEndings(tag));

            Assert.AreEqual(sb.ToString(), writerFactory.Files[TestUtilities.PrepareRelativePath("test.html.debug.jstest.html")]);
        }

        [Test]
        public void CanBundleJavascriptInDebugWithArbitraryHogan()
        {
            const string template = "<h1>{{message}}</h1>";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithPreprocessor(new HoganPreprocessor())
                .AddString(template, ".html")
                .Render("~/template.js");
            
            var sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"">var JST = JST || {};");
            sb.AppendLine(@"JST['dummy'] = new Hogan.Template(function(c,p,i){var _=this;_.b(i=i||"""");_.b(""<h1>"");_.b(_.v(_.f(""message"",c,p,0)));_.b(""</h1>"");return _.fl();;});");
            sb.AppendLine("</script>");
            Assert.AreEqual(sb.ToString(),tag);
        }
    }
}
