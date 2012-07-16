using System;
using NUnit.Framework;
using SquishIt.CoffeeScript;
using SquishIt.CoffeeScript.Coffee;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture, Platform(Exclude = "Unix, Linux, Mono")]
    public class CoffeeScriptTests
    {
        //TODO: should probably have more tests here
        JavaScriptBundleFactory javaScriptBundleFactory;

        [SetUp]
        public void Setup()
        {
            javaScriptBundleFactory = new JavaScriptBundleFactory();
        }

        [Test]
        public void CanBundleJavascriptWithArbitraryCoffeeScript()
        {
            var coffee = "alert 'test' ";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(false)
                .Create()
                .WithPreprocessor(new CoffeeScriptPreprocessor())
                .AddString(coffee, ".coffee")
                .Render("~/brewed.js");

            var compiled = javaScriptBundleFactory.FileWriterFactory.Files[TestUtilities.PrepareRelativePath(@"brewed.js")];

            Assert.AreEqual(@"(function(){alert(""test"")}).call(this);", compiled);
            Assert.AreEqual(@"<script type=""text/javascript"" src=""brewed.js?r=hash""></script>", tag);
        }

        [Test]
        public void CanBundleJavascriptInDebugWithArbitraryCoffeeScript()
        {
            var coffee = "alert 'test' ";

            var tag = javaScriptBundleFactory
                .WithDebuggingEnabled(true)
                .Create()
                .WithPreprocessor(new CoffeeScriptPreprocessor())
                .AddString(coffee, ".coffee")
                .Render("~/brewed.js");

            Assert.AreEqual("<script type=\"text/javascript\">(function() {\n  alert('test');\n}).call(this);\n</script>\r\n", tag);
        }
    }

    [TestFixture]
    public class CoffeescriptCompilerTests
    {
        [Test, Platform(Exclude = "Unix, Linux, Mono")]
        public void CompileWithSimpleAlertSucceeds()
        {
            var compiler = new CoffeeScriptCompiler();

            string result = compiler.Compile("alert 'test' ");

            Assert.AreEqual("(function() {\n  alert('test');\n}).call(this);\n", result);
        }

        [Test, Platform(Exclude = "Unix, Linux, Mono")]
        public void CompileWithComplexScriptSucceeds()
        {
            string source = @"# Assignment:
number   = 42
opposite = true

# Conditions:
number = -42 if opposite

# Functions:
square = (x) -> x * x

# Arrays:
list = [1, 2, 3, 4, 5]

# Objects:
math =
  root:   Math.sqrt
  square: square
  cube:   (x) -> x * square x

# Splats:
race = (winner, runners...) ->
  print winner, runners

# Existence:
alert 'I knew it!' if elvis?";

            var compiler = new CoffeeScriptCompiler();
            compiler.Compile(source);
        }

        [Test, Platform(Include = "Unix, Linux, Mono")]
        public void CompileFailsGracefullyOnMono()
        {
            var compiler = new CoffeeScriptCompiler();
            var exception = Assert.Throws(typeof(NotSupportedException), () => compiler.Compile(""));
            Assert.AreEqual("Coffeescript not yet supported for mono.", exception.Message);
        }

    }
}
