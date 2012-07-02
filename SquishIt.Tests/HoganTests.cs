using System.Text;
using NUnit.Framework;
using SquishIt.Hogan;
using SquishIt.Tests.Helpers;

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
        public void CanBundleJavascriptInDebugWithArbitraryHogan()
        {
            const string tempalte = "<h1>{{message}}</h1>";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithPreprocessor(new HoganPreprocessor())
                .AddString(tempalte, ".html")
                .Render("~/template.js");
            
            var sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"">var JST = JST || {};");
            sb.AppendLine(@"JST['dummy'] = new Hogan.Template(function(c,p,i){var _=this;_.b(i=i||"""");_.b(""<h1>"");_.b(_.v(_.f(""message"",c,p,0)));_.b(""</h1>"");return _.fl();;});");
            sb.AppendLine("</script>");
            Assert.AreEqual(sb.ToString(),tag);
        }
    }
}
