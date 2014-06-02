using System;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CoffeeScriptTests
    {
        JavaScriptBundleFactory javaScriptBundleFactory;

        [SetUp]
        public void Setup()
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory();
        }

        [TestCase(typeof(MsIeCoffeeScript.CoffeeScriptPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(CoffeeScript.CoffeeScriptPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CanBundleJavascriptWithArbitraryCoffeeScript(Type preprocessorType)
        {
            var preprocessor = Activator.CreateInstance(preprocessorType) as IPreprocessor;
            Assert.NotNull(preprocessor);

            var coffee = "alert 'test' ";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(false)
                .Create()
                .WithPreprocessor(preprocessor)
                .AddString(coffee, ".coffee")
                .Render("~/brewed.js");

            var compiled = javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"brewed.js")];

            Assert.AreEqual("(function(){alert(\"test\")}).call(this);\n", compiled);
            Assert.AreEqual(@"<script type=""text/javascript"" src=""brewed.js?r=hash""></script>", tag);
        }

        [TestCase(typeof(MsIeCoffeeScript.CoffeeScriptPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(CoffeeScript.CoffeeScriptPreprocessor)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CanBundleJavascriptInDebugWithArbitraryCoffeeScript(Type preprocessorType)
        {
            var preprocessor = Activator.CreateInstance(preprocessorType) as IPreprocessor;
            Assert.NotNull(preprocessor);

            var coffee = "alert 'test' ";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithPreprocessor(preprocessor)
                .AddString(coffee, ".coffee")
                .Render("~/brewed.js");

            Assert.AreEqual("<script type=\"text/javascript\">(function() {\n  alert('test');\n\n}).call(this);\n</script>\n", TestUtilities.NormalizeLineEndings(tag));
        }
    }
}
