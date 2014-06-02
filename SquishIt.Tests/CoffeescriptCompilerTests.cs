using System;
using NUnit.Framework;
using SquishIt.Tests.Helpers;
using SquishIt.Framework;
using Version = SquishIt.Framework.Version;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CoffeescriptCompilerTests
    {
        [TestCase(typeof(MsIeCoffeeScript.Coffee.CoffeeScriptCompiler)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(CoffeeScript.Coffee.CoffeeScriptCompiler)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CompileWithSimpleAlertSucceeds(Type compilerType)
        {
            var compiler = Activator.CreateInstance(compilerType);
            var method = compilerType.GetMethod("Compile");
            var result = method.Invoke(compiler, new object[] {"alert 'test' "});

            Assert.AreEqual("(function() {\n  alert('test');\n\n}).call(this);\n", result);
        }

        [TestCase(typeof(MsIeCoffeeScript.Coffee.CoffeeScriptCompiler)), Platform(Exclude = "Unix, Linux, Mono")]
        [TestCase(typeof(CoffeeScript.Coffee.CoffeeScriptCompiler)), Platform(Exclude = "Unix, Linux, Mono")]
        public void CompileWithComplexScriptSucceeds(Type compilerType)
        {
            var compiler = Activator.CreateInstance(compilerType);
            var method = compilerType.GetMethod("Compile");
            
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

            var expectedResult = TestUtilities.NormalizeLineEndings(@"(function() {
  var list, math, number, opposite, race, square,
    __slice = [].slice;

  number = 42;

  opposite = true;

  if (opposite) {
    number = -42;
  }

  square = function(x) {
    return x * x;
  };

  list = [1, 2, 3, 4, 5];

  math = {
    root: Math.sqrt,
    square: square,
    cube: function(x) {
      return x * square(x);
    }
  };

  race = function() {
    var runners, winner;
    winner = arguments[0], runners = 2 <= arguments.length ? __slice.call(arguments, 1) : [];
    return print(winner, runners);
  };

  if (typeof elvis !== ""undefined"" && elvis !== null) {
    alert('I knew it!');
  }

}).call(this);
");

            var result = method.Invoke(compiler, new object[] { source });
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(typeof(MsIeCoffeeScript.Coffee.CoffeeScriptCompiler)), Platform(Include = "Unix, Linux, Mono")]
		[TestCase(typeof(CoffeeScript.Coffee.CoffeeScriptCompiler)), Platform(Include = "Unix, Linux, Mono")]
		public void CompileFailsGracefullyOnMono (Type compilerType)
		{
			var compiler = Activator.CreateInstance (compilerType);
			var method = compilerType.GetMethod ("Compile");

			string message;
			if (Platform.Mono && Platform.MonoVersion >= new Version("2.10.8")) 
			{
				var ex = Assert.Throws<System.Reflection.TargetInvocationException>(() => method.Invoke (compiler, new[] { "" }));
				message = ex.InnerException.Message;
			} 
			else 
			{
				var ex = Assert.Throws<Exception>(() => method.Invoke (compiler, new[] { "" }));
				message = ex.Message;
			}
			Assert.AreEqual("Coffeescript not yet supported for mono.", message);
		}

    }
}