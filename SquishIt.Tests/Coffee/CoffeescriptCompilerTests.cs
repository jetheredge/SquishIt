using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SquishIt.Framework.Coffee;

namespace SquishIt.Tests.Coffee
{
	[TestFixture]
	public class CoffeescriptCompilerTests
	{
		[Test, Platform (Exclude = "Unix, Linux, Mono")]
		public void CompileWithSimpleAlertSucceeds ()
		{
			var compiler = new CoffeescriptCompiler ();

			string result = compiler.Compile ("alert 'test' ");

			Assert.AreEqual ("(function() {\n  alert('test');\n}).call(this);\n", result);
		}

		[Test, Platform (Exclude = "Unix, Linux, Mono")]
		public void CompileWithComplexScriptSucceeds ()
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

			var compiler = new CoffeescriptCompiler ();
			string result = compiler.Compile (source);
		}

		[Test, Platform (Include = "Unix, Linux, Mono")]
		public void CompileFailsGracefullyOnMono ()
		{
			var compiler = new CoffeescriptCompiler();
			var exception = Assert.Throws(typeof (NotSupportedException), () => compiler.Compile(""));
			Assert.AreEqual("Coffeescript not yet supported for mono.", exception.Message);
		}

	}
}
